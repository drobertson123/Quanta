using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using NodaTime;
using Quanta.Core.Options.Model;
using Quanta.Core.Options.Model.Book;
using TeaTime;

namespace Quanta.Core.Benchmarks
{
    internal class Operations<T> where T : struct
    {
        private string filename = "bench";

        public Operations(string filename) => this.filename = filename;


        public static void RunAll(Runner runner)
        {
            Console.WriteLine("RunAll");
            Stopwatch sw = Stopwatch.StartNew();

            var op = new Operations<OptionBookStruct>(Path.GetFullPath(@"C:\DataTest2\testfile"));
            //op.MeasureArray(runner);
            //op.MeasureList(runner);
            //op.MeasureListPreAllocated(runner);
            //op.MeasureTeaFile(runner);
            //op.MeasureTeaFileMemoryMapped(runner);
            //op.MeasureTeaFileRawMemoryMapped(runner);
            //op.MeasureTeaFileRawMemoryMappedTick(runner);
            //op.MeasureProtobufs(runner);
            op.MeasureQuanta(runner);

            sw.Stop();

            Console.WriteLine(sw.Elapsed);
            //op.Cleanp();
            //GC.Collect();
            //GC.WaitForPendingFinalizers();
            //GC.Collect();
        }

        private void Cleanp() => File.Delete(this.filename);

        public static OptionBook AddQuotes(OptionBook book, int count)
        {
            OptionBookMap map = new OptionBookMap();

            //GOOGL180329C01052500	
            for (int i = 0; i < count; i++)
            {

                OptionQuote quote = new OptionQuote
                {
                    Symbol = $"{book.MetaData.UnderlyingSymbol}180329C{String.Format("00000000", 1000000 + i)}",
                    UnderlyingPrice = 100,
                    Right = OptionRight.Call,
                    Expiration = Instant.FromDateTimeUtc(new DateTime(2018, 3, 29, 20, 0, 0, DateTimeKind.Utc)),
                    Multiplier = 100,
                    Strike = (Decimal)((1000000 + i) / 1000),
                    Style = OptionStyle.American,
                    Bid = 3m,
                    Ask = 3.55m,
                    OpenInterest = 10000,
                    Volume = 3000
                };

                book.Add(quote);
            }

            return book;
        }

        public OptionBookMeta GetOptionBookMeta() =>
            new OptionBookMeta
            {
                UnderlyingSymbol = "GOOGL",
                SecurityType = SecurityTypes.Option,
                Interval = QuoteInterval.OneDay,
                Delay = QuoteLag.Historical,
                AsOf = Instant.FromDateTimeUtc(new DateTime(2018, 3, 24, 11, 0, 0, DateTimeKind.Utc))
            };

        public void MeasureArray(Runner runner)
        {
            long N = runner.N;
            double[] values = null;
            runner.Measure("allocate array", () =>
            {
                values = new double[N];
            }
            );
            runner.Measure("fill array", () =>
            {
                for (int i = 0; i < N; i++)
                {
                    values[i] = i;
                }
            });
            runner.Measure("sum array", () =>
            {
                double sum = 0;
                for (int i = 0; i < N; i++)
                {
                    sum += values[i];
                }
                Console.WriteLine(sum);
                if (sum != runner.TargetValue) throw new Exception("wrong result");
            });
        }

        public void MeasureList(Runner runner)
        {
            long N = runner.N;
            List<double> values = new List<double>();
            runner.Measure("fill list", () =>
            {
                for (int i = 0; i < N; i++)
                {
                    values.Add(i);
                }
            });
            runner.Measure("sum list", () =>
            {
                double sum = 0;
                for (int i = 0; i < N; i++)
                {
                    sum += values[i];
                }
                Console.WriteLine(sum);
                if (sum != runner.TargetValue) throw new Exception("wrong result");
            });
        }

        public void MeasureListPreAllocated(Runner runner)
        {
            long N = runner.N;
            List<double> values = new List<double>((int)N);
            runner.Measure("fill list pre", () =>
            {
                for (int i = 0; i < N; i++)
                {
                    values.Add(i);
                }
            });
            runner.Measure("sum list pre", () =>
            {
                double sum = 0;
                for (int i = 0; i < N; i++)
                {
                    sum += values[i];
                }
                Console.WriteLine(sum);
                if (sum != runner.TargetValue) throw new Exception("wrong result");
            });
        }

        public void MeasureTeaFile(Runner runner)
        {
            long N = runner.N;
            File.Delete(filename);
            using (var tf = TeaFile<double>.Create(filename))
            {
                runner.Measure("fill teafile<double>", () =>
                {
                    for (int i = 0; i < N; i++)
                    {
                        tf.Write(i); // conversion
                    }
                });
            }
            using (var tf = TeaFile<double>.OpenRead(filename))
            {
                runner.Measure("sum teafile<double>", () =>
                {
                    double sum = 0;
                    for (int i = 0; i < N; i++)
                    {
                        sum += tf.Read();
                    }
                    Console.WriteLine(sum);
                    if (sum != runner.TargetValue) throw new Exception("wrong result");
                });
            }
        }

        public void MeasureTeaFileMemoryMapped(Runner runner)
        {
            long N = runner.N;
            ManagedMemoryMapping<double> view = null;
            runner.Measure("TeaFile<double>.OpenMemoryMapping", () =>
            {
                view = TeaFile<double>.OpenMemoryMapping(filename);
            });
            double sum = 0d;
            runner.Measure("memmap sum += view[i]", () =>
            {
                for (int i = 0; i < N; i++)
                {
                    sum += view[i];
                }
            });
            Console.WriteLine(sum);
            runner.Measure("memmap sum += view[i] 2nd run", () =>
            {
                for (int i = 0; i < N; i++)
                {
                    sum += view[i];
                }
            });
            view.Dispose();
            view = null;
        }

        public unsafe void MeasureTeaFileRawMemoryMapped(Runner runner)
        {
            long N = runner.N;
            RawMemoryMapping<double> view = null;
            //string copyfilename = "copy" + filename;
            //if (File.Exists(copyfilename)) File.Delete(copyfilename);
            //File.Copy(filename, copyfilename);
            //if(File.Exists(filename)) File.Delete(filename);
            runner.Measure("Doug - TeaFile<double>.OpenRawMemoryMapping", () =>
            {
                view = TeaFile<double>.OpenRawMemoryMapping(filename);
            });
            runner.Measure("memmap raw sum += view[i]", () =>
            {
                double sum = 0d;
                double* p = (double*)view.ItemAreaStart;
                for (int i = 0; i < N; i++)
                {
                    sum += *p;
                    p++;
                }
                Console.WriteLine(sum);
                if (sum != runner.TargetValue) throw new Exception("wrong result");
            });
            runner.Measure("memmap raw sum += view[i] 2nd run", () =>
            {
                double sum = 0d;
                double* p = (double*)view.ItemAreaStart;
                for (int i = 0; i < N; i++)
                {
                    sum += *p;
                    p++;
                }
                Console.WriteLine(sum);
                if (sum != runner.TargetValue) throw new Exception("wrong result");
            });
            runner.Measure("memmap raw sum += view[i] 3rd run", () =>
            {
                double sum = 0d;
                double* p = (double*)view.ItemAreaStart;
                for (int i = 0; i < N; i++)
                {
                    sum += *p;
                    p++;
                }
                Console.WriteLine(sum);
                if (sum != runner.TargetValue) throw new Exception("wrong result");
            });
            runner.Measure("memmap raw sum += view[i] 4th run, pointer only", () =>
            {
                double sum = 0d;
                double* p = (double*)view.ItemAreaStart;
                double* end = (double*)view.ItemAreaEnd;
                while (p < end)
                {
                    sum += *p;
                    p++;
                }
                Console.WriteLine(sum);
                if (sum != runner.TargetValue) throw new Exception("wrong result");
            });
            view.Dispose();
            view = null;
        }

        public unsafe void MeasureTeaFileRawMemoryMappedTick(Runner runner)
        {

            long N = runner.N;

            OptionBookMap map = new OptionBookMap();
            OptionBookMeta meta = GetOptionBookMeta();
            OptionBook book = new OptionBook
            {
                MetaData = meta
            };

            book = AddQuotes(book, (int)N);


            File.Delete($"{filename}.tea");
            using (var tf = TeaFile<OptionBookStruct>.Create($"{filename}.tea"))
            {
                //List<OptionBookStruct> optionList = OptionsTest.GetOptions((int)N).ToList();
                runner.Measure($"TeaFile Write (foreach) file OptionStruct - [{N} quotes]", () =>
                {
                    foreach (OptionQuote option in book)
                    {
                        tf.Write(map.ToStruct(option));
                    }
                });
                Console.WriteLine($"TeaFile Write (foreach) file OptionStruct - [{book.Count()} quotes]");
            }

            File.Delete($"{filename}.tea");
            using (var tf = TeaFile<OptionBookStruct>.Create($"{filename}.tea"))
            {
                List<OptionBookStruct> optionList = book.Select(_ => map.ToStruct(_)).ToList();

                runner.Measure($"TeaFile Write (enumerable) file OptionStruct - [{N} quotes]", () =>
                {
                    tf.Write(optionList);
                });
                Console.WriteLine($"TeaFile Write (enumerable) file OptionStruct - [{optionList.Count()} quotes]");
            }

            OptionBook outBook = new OptionBook
            {
                MetaData = meta
            };

            runner.Measure($"TeaFile Read memmap raw using Generic of T - [{N} quotes]", () =>
            {
                using (RawMemoryMapping<OptionBookStruct> view = TeaFile<OptionBookStruct>.OpenRawMemoryMapping($"{filename}.tea"))
                {
                    int len = Unsafe.SizeOf<T>();
                    int count = 0;
                    OptionBookStruct opt;

                    byte* start = view.ItemAreaStart;
                    byte* end = view.ItemAreaEnd;

                    while (start < end)
                    {
                        opt = Unsafe.Read<OptionBookStruct>(start);
                        start += len;

                        outBook.Add(map.ToData(opt, book.MetaData));

                        count++;
                    }
                    Console.WriteLine($"TeaFile Read memmap raw using Generic of T - [{outBook.Count} quotes]");
                }
            });

            outBook.Clear();

            runner.Measure($"TeaFile Read memmap raw using OptionStruct - [{N} quotes]", () =>
            {
                using (RawMemoryMapping<OptionBookStruct> view = TeaFile<OptionBookStruct>.OpenRawMemoryMapping($"{filename}.tea"))
                {
                    //double sum = 0d;
                    int len = Unsafe.SizeOf<T>();
                    int count = 0;
                    OptionBookStruct opt;

                    OptionBookStruct* start = (OptionBookStruct*)view.ItemAreaStart;
                    OptionBookStruct* end = (OptionBookStruct*)view.ItemAreaEnd;

                    while (start < end)
                    {
                        opt = Unsafe.Read<OptionBookStruct>(start);
                        outBook.Add(map.ToData(opt, book.MetaData));
                        start++;
                        count++;
                    }
                    Console.WriteLine($"TeaFile Read memmap raw using OptionStruct - [{outBook.Count} quotes]");
                }
            });
            outBook.Clear();
            //List<T> options = new List<T>((int)N);
            //runner.Measure($"TeaFile Read memmap raw using Generic to List of T  - [{N} quotes]", () =>
            //{
            //    using (RawMemoryMapping<T> view = TeaFile<T>.OpenRawMemoryMapping($"{filename}.tea"))
            //    {
            //        double sum = 0d;
            //        int len = Unsafe.SizeOf<T>();
            //        int count = 0;
            //            //T opt;

            //            byte* start = view.ItemAreaStart;
            //        byte* end = view.ItemAreaEnd;

            //        while (start < end)
            //        {
            //            options.Add(Unsafe.Read<T>(start));
            //            start += len;
            //            count++;
            //        }
            //        Console.WriteLine($"TeaFile Read memmap raw using Generic to List of T  - [{count} quotes]");
            //    }
            //});
        }

        //public unsafe void MeasureProtobufs(Runner runner)
        //{
        //    long N = runner.N;
        //    //RawMemoryMapping<OptionStruct> view = null;
        //    File.Delete($"{filename}.proto");

        //    using (var file = File.Create($"{filename}.proto"))
        //    {
        //        List<OptionProto> options = OptionsTest.GetOptionProtos((int)N).ToList();
        //        runner.Measure($"Protobuf Write file OptionProto - [{N} quotes]", () =>
        //        {
        //            Serializer.Serialize(file, options);
        //        });
        //        Console.WriteLine($"Protobuf Write file OptionProto - [{options.Count()} quotes]");
        //    }

        //    int count = 0;
        //    runner.Measure($"Protobuf Read OptionProto - [{N} quotes]", () =>
        //    {
        //        List<OptionProto> options;
        //        using (var file = File.OpenRead($"{filename}.proto"))
        //        {
        //            options = Serializer.Deserialize<List<OptionProto>>(file);
        //        }

        //        Decimal sum = 0f;
        //        foreach (OptionProto option in options)
        //        {
        //            sum += option.Bid;
        //            count++;
        //        }
        //        Console.WriteLine($"Protobuf Read OptionProto - [{count} quotes]");
        //    });
        //}

        public unsafe void MeasureQuanta(Runner runner)
        {
            long N = runner.N;
            //RawMemoryMapping<OptionStruct> view = null;
            File.Delete($"{filename}.optbook");

            string fileName = $"{filename}.optbook";
            string mapKey = $"MAP:Tea.optbooktesting";

            OptionBookSession session = new OptionBookSession(fileName, mapKey);
            OptionBookMeta meta = GetOptionBookMeta();
            OptionBook book = new OptionBook
            {
                MetaData = meta
            };

            book = AddQuotes(book, (int)N);

            runner.Measure($"Quanta Write file  - [{N} quotes]", () =>
            {
                session.Write(book);
            });
            Console.WriteLine($"Quanta Write file  - [{book.Count()} quotes]");
            session = null;

            session = new OptionBookSession(fileName, mapKey);

            int count = 0;
            runner.Measure($"#1 Quanta Read  - [{N} quotes]", () =>
            {
                OptionBook bookOut1 = session.Read();

                Decimal sum = 0m;
                foreach (OptionQuote option in bookOut1)
                {
                    sum += option.Bid;

                }
                Console.WriteLine($"Quanta Read  - [{bookOut1.Count()} quotes]");
            });

            count = 0;
            runner.Measure($"#2 Quanta Read  - [{N} quotes]", () =>
            {
                OptionBook bookOut2 = session.Read();

                Decimal sum = 0m;
                foreach (OptionQuote option in bookOut2)
                {
                    sum += option.Bid;
                    count++;
                }
                Console.WriteLine($"Quanta Read  - [{bookOut2.Count()} quotes]");
            });
        }
    }
}

