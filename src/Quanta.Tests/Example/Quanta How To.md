# Quanta: How To Guide

## 1. Build Data Structures

Create the data object and the storage Struct.

Requirements:

1. The data object must have a parameter less constructor.
2. The storage Struct needs to contain **only** fixed length unmanaged variables that are Value Types.

Example of an ``OptionQuote`` data object that extends the ``QuoteBase`` Object.

```C#
    public abstract class QuoteBase : IQuote
    {
        public string Symbol { get; set; }
        public virtual SecurityTypes SecurityType { get; set; }
        public virtual QuoteInterval Interval { get; set; }
        public virtual SampleTimes SampleTime { get; set; }
        public virtual QuoteLag Delay { get; set; }
        public Instant AsOf { get; set; }	//Note: Instant is from Nodatime
    }
```

```C#
    public class OptionQuote : QuoteBase
    {
        public OptionQuote() { }
        public string UnderlyingSymbol { get; set; }
        public decimal? UnderlyingPrice { get; set; }
        public OptionRight Right { get; set; }
        public Instant Expiration { get; set; }	//Note: Instant is from Nodatime
        public int Multiplier { get; set; }
        public decimal Strike { get; set; }
        public OptionStyle Style { get; set; }
        public decimal Bid { get; set; }
        public decimal Ask { get; set; }
        public Int64 OpenInterest { get; set; }
        public Int64 Volume { get; set; }
    }
```

The Struct allows us to store the data elements in fixed size binary chunks. All fields must be fixed size unmanaged Value Types. References or variable size values will not work.

```C#
    public struct OptionStruct
    {
        public float UnderlyingPrice;
        public float Bid;
        public float Ask;
        public Int64 OpenInterest;
        public Int64 Volume;
        public Int64 AsOf;
    }
```



## 2. Define the Metadata

The metadata defines the data fields that are common to all data elements in a file. We must create a Metadata Struct or object to store that data.

Requirements:

1. The Metadata Struct or Object **must** be marked ``[Serializable]``
2. Metadata **should** be kept to a simple structure. Ideally a flat object structure.
3. You **may** implement an ISerializable interface but it **must** be compatible with Binary Serialization.

This is the ``OptionMetadata`` Struct used for the example

```C#
    [Serializable]
    public struct OptionMetadata
    {
        public string Symbol;
        public string UnderlyingSymbol;
        public OptionRight Right;               // (int)OptionRight
        public long Expiration;                 // Nodatime: instant.Ticks
        public int Multiplier;
        public decimal Strike;
        public Int32 Style;                     // (int)OptionStyle 
        public Int32 SecurityType;              // (int)SecurityTypes
        public Int32 Interval;                  // (int)QuoteInterval
        public Int32 SampleTime;                // (int)SampleTimes
        public Int32 Delay;                     // (int)QuoteLag
    }
```

### Create Formatter

The storage process needs the Metadata in a byte[] format

 To do this we Extend a ``MetaFormatter`` object like this;

```C#
    public class OptionFormatter : MetaFormatter<OptionMetadata>
    {
    }
```

This exposes two Methods of the ``MetaFormatter`` base class;

```C#
    public class MetaFormatter<T> 
    {
        public virtual T Deserialize(byte[] buffer)
        {
            MemoryStream stream = new MemoryStream(buffer);
            BinaryFormatter formatter = new BinaryFormatter();

            T output = (T)formatter.Deserialize(stream);

            return output;
        }

        public virtual byte[] Serialize(T graph)
        {
            MemoryStream stream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();

            formatter.Serialize(stream, graph);

            return stream.ToArray();
        }
    }
```

These may be overriden for more specific implementations, but in most cases of simple metadata this should be fine.

## 3. Map the Data Objects

A map between the Data Object and the Metadata & Storage Struct must be created.

The ```DataMap<TData, TStruct, TMeta>``` class is inherited and extended;

```C#
    public class DataMap<TData, TStruct, TMeta>
        where TData : new()
        where TStruct : struct
        where TMeta : struct
    {
        private Func<TStruct, TMeta, TData> _toData;
        private Func<TData, TStruct> _toStruct;

        protected void SetCreateDataFunc(Func<TStruct, TMeta, TData> toData) => 
            _toData = toData;
        protected void SetCreateStructFunc(Func<TData, TStruct> toStruct) => 
            _toStruct = toStruct;
            
        public virtual TData ToData(TStruct data, TMeta meta) => _toData(data, meta);
        public virtual TStruct FromStruct(TData data) => _toStruct(data);
    }
```

There are two critical methods that must be used to set the translation between the data objects.

``SetCreateDataFunc(Func<TStruct, TMeta, TData> toData)`` is used to set a Lambda function for the translation of the Storage Struct and Metadata to the Data Object.

``SetCreateStructFunc(Func<TData, TStruct> toStruct)`` is used to set a Lambda function for the translation of the Data  Object to the Storage Struct.

An implementation of a map between the ```OptionQuote```, ```OptionStruct``` and ```OptionMetadata``` looks like this;

```C#
    public class OptionMap : DataMap<OptionQuote, OptionStruct, OptionMetadata>
    {
        public OptionMap()
        {
            SetCreateDataFunc((data, meta) =>
                 new OptionQuote()
                 {
                     Symbol = meta.Symbol,
                     UnderlyingSymbol = meta.UnderlyingSymbol,
                     UnderlyingPrice = (Decimal)data.UnderlyingPrice,
                     Right = meta.Right,
                     Expiration = Instant.FromUnixTimeTicks(meta.Expiration),
                     Multiplier = meta.Multiplier,
                     Strike = meta.Strike,
                     Style = (OptionStyle)meta.Style,
                     Bid = (decimal)data.Bid,
                     Ask = (decimal)data.Ask,
                     OpenInterest = data.OpenInterest,
                     Volume = data.Volume,
                     SecurityType = (SecurityTypes)meta.SecurityType,
                     Interval = (QuoteInterval)meta.Interval,
                     SampleTime = (SampleTimes)meta.SampleTime,
                     Delay = (QuoteLag)meta.Delay,
                     AsOf = Instant.FromUnixTimeTicks(data.AsOf)
                 });

            SetCreateStructFunc((data) =>
                new OptionStruct
                {
                    UnderlyingPrice = (float)data.UnderlyingPrice.GetValueOrDefault(),
                    Bid = (float)data.Bid,
                    Ask = (float)data.Ask,
                    OpenInterest = data.OpenInterest,
                    Volume = data.Volume,
                    AsOf = data.AsOf.ToUnixTimeTicks()
                });
        }
    }
```

## 4. Create a Data Set or Sequence

A container for handling groups of data items is required. This can either be a Set or a Sequence.

A Set is an unordered list of data items and a Sequence is an order list of data. Both Sets and Sequences must implement an ``IMetadata`` interface. This is normally accomplished by inheriting the ```Sequence<TData, TMeta>``` or ```Set<TData, TMeta>``` class.

### Data Set Construction

Data Sets are unordered lists of data objects that is based on the [List<T> Class](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1?view=netframework-4.7.1). They should typically derive from the ```Set<TData, TMeta>``` class.

```C#
    public class Set<TData, TMeta> : List<TData>, IMetadata<TMeta>
    {
        public virtual TMeta Metadata { get; set; }
    }
```



### Data Sequence Construction

A Data Sequence is stored and retrieved as sorted data. It is based on the [SortedSet<T> Class](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.sortedset-1?view=netframework-4.7.1) and follows all the same rules and requirements.

Sequences are derived from the following class;

```C#
    public class Sequence<TData, TMeta> : SortedSet<TData>, IMetadata<TMeta>
    {
        public Sequence(IComparer<TData> comparer)
            : base(comparer) { }
        public Sequence(IEnumerable<TData> collection, IComparer<TData> comparer)
            : base(collection, comparer) { }

        public virtual TMeta Metadata { get; set; }
    }
```

 The Data Objects require a way to compare them to establish the sort order. Duplicate objects are not allowed. A default comparer can be used, but in most cases we will need to create a Comparer implementation to specify the sort order.

#### Sequence Comparer

A sequence comparer is typically a simple object that derives from the [Comparer<T>](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.comparer-1?view=netframework-4.7.1) base class where ``<T>`` would be the data class we are using. This would implement an override of the ``int Compare(<T> x, <T> y)`` method, returning an integer value representing the comparison result.

Example;

```C#
    public class OptionComparer : Comparer<OptionQuote>
    {
        public override int Compare(OptionQuote x, OptionQuote y)
        {
            return x.AsOf.CompareTo(y.AsOf);
        }
    }
```

#### Sequence 

The derived Sequence class uses the Comparer in its construction to establish the sort order of the sequence.

```C#
    public class OptionSeries : Sequence<OptionQuote, OptionMetadata>
    {
        public OptionSeries() 
            : base(new OptionComparer()) { }

        public OptionSeries(IEnumerable<OptionQuote> collection) 
            : base(collection, new OptionComparer()) { }
    }
```



## 5. Build the Router

The Router object defines the file location for the data storage and what data is placed there. Typically this would mean a fully resolved file path, but it could be used for any route definition that is a string.

The Router must implement the ``IRouter<T> `` interface. Specifically, it needs to implement and provide an output for;

```C#
string GetRoute(T meta)
```

The ``GetRoute(T meta)`` method uses the Metadata for the data being passed in to determine the Path for the data and hands it back as a string.

Normally each type of Data Object /Metadata combination requires its own Router object.

A simple way to implement the ``IRouter<T>`` interface is to use the ``Router<T>`` base class. This has an internal implementation of the ``GetRoute(T meta)`` method. This method relies on a ``Func<T,string>`` function that you set during the object construction. This function can be set using the ``void SetRouteFunc(Func<T, string> map)`` protected method.

Example Router;

```C#
    public class OptionSequenceRouter : Router<OptionSequenceMeta>
    {
        private const string DATA_ZONE = @"OptionsData\TimeSeries";
        private const string DATA_EXTENSION = ".optseq";

        public OptionSequenceRouter() : this("") { }
        public OptionSequenceRouter(string root)
        {
            Root = root;
            
            SetRouteFunc(meta =>
          {
              string symbol = meta.Symbol.Trim().ToUpper();
              string underlying = meta.UnderlyingSymbol.Trim().ToUpper();
              string subSection = $"_{underlying}";

              Instant exp = Instant.FromUnixTimeTicks(meta.Expiration);
              string expFolder =
                  exp.ToString("yyyy-MM-dd", DateTimeFormatInfo.InvariantInfo);

              string fileName = $"_{symbol}";

              string path =
                  Path.Combine(
                      Root,
                      DATA_ZONE,
                      subSection,
                      expFolder,
                      fileName,
                      DATA_EXTENSION);

              return path;
          });
        }
        public string Root { get; set; }
    }
```

