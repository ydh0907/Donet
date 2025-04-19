using Donet.Utils;

namespace Test
{
    internal class TestProgram
    {
        public static void Main()
        {
            byte[] buffer = new byte[1024];
            string message = "dohee";
            Serializer serializer = new Serializer();
            serializer.Open(NetworkSerializeMode.Serialize, new ArraySegment<byte>(buffer, 0, buffer.Length));
            serializer.Serialize(ref message);

            serializer.Open(NetworkSerializeMode.Deserialize, buffer);
            string received = "";
            serializer.Serialize(ref received);

            Console.WriteLine(received);
        }
    }
}
