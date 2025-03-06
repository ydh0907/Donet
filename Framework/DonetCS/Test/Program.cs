using Donet;

namespace Test
{
    internal class Program
    {
        struct Data : INetworkSerializable
        {
            public int id;
            public float x;

            public bool Serialize(Serializer serializer)
            {
                return serializer.SerializeValue(ref id) && serializer.SerializeValue(ref x);
            }
        }

        class Obj
        {
            public int id;
            public int id2;
            public int id3;
            public int id4;
            public int id5;
            public float name;
        }

        static void Main(string[] args)
        {
            byte[] buffer = new byte[1024];

            Data data = new Data() { id = 4, x = 7.5f };
            ArraySegment<byte> segment = new ArraySegment<byte>(buffer, 0, buffer.Length);
            LocalSerializer.Serializer.Open(NetworkSerializeMode.Serialize, segment);
            LocalSerializer.Serializer.SerializeSerializable(ref data);
            LocalSerializer.Serializer.Close();

            Data receive = new Data();
            LocalSerializer.Serializer.Open(NetworkSerializeMode.Deserialize, segment);
            LocalSerializer.Serializer.SerializeSerializable(ref receive);
            LocalSerializer.Serializer.Close();

            Console.WriteLine(receive.id);
            Console.WriteLine(receive.x);
        }
    }
}
