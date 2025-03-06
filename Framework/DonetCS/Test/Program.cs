using Donet;

namespace Test
{
    internal class Program
    {
        struct Data : INetworkSerializable
        {
            public int id;
            public string name;

            public bool Serialize(Serializer serializer)
            {
                return serializer.Serialize(ref id) && serializer.Serialize(ref name);
            }

            public override string ToString()
            {
                return $"{id}: {name}";
            }
        }

        static void Main(string[] args)
        {
            byte[] buffer = new byte[1024];

            List<Data> datas = new List<Data> { new Data { id = 10, name = "철수" }, new Data { id = 20, name = "영희" } };
            ArraySegment<byte> segment = new ArraySegment<byte>(buffer, 0, buffer.Length);
            LocalSerializer.Serializer.Open(NetworkSerializeMode.Serialize, segment);
            LocalSerializer.Serializer.SerializeObject(ref datas);
            LocalSerializer.Serializer.Close();

            List<Data> results = new List<Data>();
            LocalSerializer.Serializer.Open(NetworkSerializeMode.Deserialize, segment);
            LocalSerializer.Serializer.SerializeObject(ref results);
            LocalSerializer.Serializer.Close();

            foreach (Data data in results)
                Console.WriteLine(data);

            Console.WriteLine();
            foreach (Data data in datas)
                Console.WriteLine(data);
        }
    }
}
