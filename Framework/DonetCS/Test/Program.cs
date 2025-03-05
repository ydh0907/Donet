using Donet;

namespace Test
{
    internal class Program
    {
        struct Data : INetworkSerializable
        {
            public int id;
            public float name;

            public void Serialize(Serializer serializer)
            {
                serializer.SerializeValue(ref id);
                serializer.SerializeValue(ref name);
            }
        }

        static void Main(string[] args)
        {
            Data data = new Data();
            data.id = 10;
            data.name = "test";

            byte[] buffer = new byte[1024];
            ArraySegment<byte> segment = new ArraySegment<byte>(buffer, 0, buffer.Length);
            LocalSerializer.Serializer.Open(NetworkSerializeMode.Serialize, segment);
            LocalSerializer.Serializer.SerializeValue(ref data);
            LocalSerializer.Serializer.Close();

            Data receive = new Data();
            LocalSerializer.Serializer.Open(NetworkSerializeMode.Deserialize, segment);
            LocalSerializer.Serializer.SerializeValue(ref receive);
            LocalSerializer.Serializer.Close();

            Console.WriteLine(receive.id);
            Console.WriteLine(receive.name);
        }
    }
}
