using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;

namespace Serialization
{
    public class Serialization_Deserialization

    {
        public byte[] SerializeObject<T>(T obj)
        {
            DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(obj.GetType());
            using (MemoryStream memoryStream = new MemoryStream())
            {
                jsonSerializer.WriteObject(memoryStream, obj);
                return memoryStream.ToArray();
            }
        }

        public T DeserializeObject<T>(byte[] jsonData, int bytesRec)
        {
            DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(T));
            using (MemoryStream memoryStream = new MemoryStream(jsonData, 0, bytesRec))
            {
                return (T)jsonSerializer.ReadObject(memoryStream);
            }
        }


    }
}