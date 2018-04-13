using System;
using System.Collections.Generic;
using System.Linq;
using ProtoBuf;
using Quanta.Core.Options.Model.Book;

namespace Quanta.Core.Benchmarks
{
    //public struct OptionStruct
    //{
    //    public Decimal UnderlyingPrice;
    //    public Decimal Bid;
    //    public Decimal Ask;
    //    public Int64 OpenInterest;
    //    public Int64 Volume;
    //    public Decimal ImpliedVolatility;
    //    public Decimal Delta;
    //    public Decimal Theta;
    //    public Decimal Vega;
    //    public Decimal Gamma;
    //    public Int32 EventMapID;
    //}

    //[ProtoContract]
    //public class OptionProto
    //{
    //    [ProtoMember(1)] public Decimal UnderlyingPrice { get; set; }
    //    [ProtoMember(2)] public Decimal Bid { get; set; }
    //    [ProtoMember(3)] public Decimal Ask { get; set; }
    //    [ProtoMember(4)] public Int64 OpenInterest { get; set; }
    //    [ProtoMember(5)] public Int64 Volume { get; set; }
    //    [ProtoMember(6)] public Decimal ImpliedVolatility { get; set; }
    //    [ProtoMember(7)] public Decimal Delta { get; set; }
    //    [ProtoMember(8)] public Decimal Theta { get; set; }
    //    [ProtoMember(9)] public Decimal Vega { get; set; }
    //    [ProtoMember(10)] public Decimal Gamma { get; set; }
    //    //[ProtoMember(11)] public Int32 EventMapID { get; set; }
    //}
    public static class OptionsTest
    {
        public static IEnumerable<OptionBookStruct> GetOptions(int count)
        {
            List<OptionBookStruct> options = new List<OptionBookStruct>(count);
            Random Rnd = new Random();

            for (int i = 0; i < count; i++)
            {
                options.Add(
                    new OptionBookStruct
                    {
                        //UnderlyingPrice = 55.75f,
                        Bid = (float)0.35m,
                        Ask = (float)0.45m,
                        OpenInterest = 5000,
                        Volume = 1000
                    });
            }

            return options;
        }

        //public static IEnumerable<OptionProto> GetOptionProtos(int count)
        //{
        //    List<OptionProto> options = new List<OptionProto>(count);
        //    Random Rnd = new Random();

        //    for (int i = 0; i < count; i++)
        //    {
        //        options.Add(
        //            new OptionProto
        //            {
        //                UnderlyingPrice = 55.75f,
        //                Bid = 0.35f,
        //                Ask = 0.45f,
        //                OpenInterest = 5000,
        //                Volume = 1000,
        //                ImpliedVolatility = .32f,
        //                Delta = .32f,
        //                Theta = .32f,
        //                Vega = .32f,
        //                Gamma = .32f,
        //                //EventMapID = 3
        //            });
        //    }

        //    return options;
        //}
    }
}
