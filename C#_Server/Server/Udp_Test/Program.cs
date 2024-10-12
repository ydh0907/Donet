namespace Udp_Test
{
    internal class Program
    {
        interface IInter
        {
            public int Value { get; set; }
            public IInter GetClone() => this;
        }
        struct Cla1 : IInter
        {
            private int _value;
            public int Value { get => _value; set => _value = value; }

            public IInter GetClone()
            {
                return this;
            }
        }
        struct Cla2 : IInter
        {
            private int _value;
            public int Value { get => _value; set => _value = value; }

            public IInter GetClone()
            {
                return this;
            }
        }

        static Dictionary<int, IInter> dictionary = new();

        static void Main(string[] args)
        {
            dictionary.Add(1, new Cla1() { Value = 1 });
            dictionary.Add(2, new Cla2());

            IInter iter = GetCla(1).GetClone();
            iter.Value = 10;

            Console.WriteLine(iter == GetCla(1));
        }

        static IInter GetCla(int id)
        {
            return dictionary[id];
        }
    }
}
