using System;
using System.IO;
using System.Xml.Serialization;

namespace Quanta.Core.Data
{
    public static class MetaFormatter
    {
        public static T Deserialize<T>(byte[] buffer)
            where T:IMetaData
        {
            T output;
            using (MemoryStream stream = new MemoryStream(buffer))
            {
                //BinaryFormatter formatter = new BinaryFormatter();
                //output = (T)formatter.Deserialize(stream);

                XmlSerializer mySerializer = new XmlSerializer(typeof(T));
                output = (T)mySerializer.Deserialize(stream);
            }
            return output;
        }

        public static byte[] Serialize<T>(T graph)
            where T: IMetaData
        {
            byte[] output;
            using (MemoryStream stream = new MemoryStream())
            {
                //BinaryFormatter formatter = new BinaryFormatter();
                //formatter.Serialize(stream, graph);

                XmlSerializer mySerializer = new XmlSerializer(typeof(T));
                mySerializer.Serialize(stream, graph);
                output = stream.ToArray();
            }
            return output;
        }
    }
}
