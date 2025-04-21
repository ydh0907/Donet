using Donet.Utils;

namespace Test
{
    internal class TestProgram
    {
        static Atomic<int> race = new Atomic<int>();

        interface ITest
        {
            public int Get();
            public void Set(int value);
        }

        struct Test : ITest
        {
            public int value;

            public int Get()
            {
                return value;
            }

            public void Set(int value)
            {
                this.value = value;
            }
        }

        public static void Main()
        {
            List<ITest> list = new List<ITest>();
            Test test = new Test();
            list.Add(test);
            test.Set(1);
            list.Add(test);
            test.Set(2);
            list.Add(test);

            foreach (Test t in list)
            {
                Console.WriteLine(t.Get());
            }
        }
    }
}
