using System.Text;

namespace Test
{
    internal class TestProgram
    {
        static void Main(string[] args)
        {
            var path = System.Reflection.Assembly.GetEntryAssembly().Location;
            StringBuilder sb = new StringBuilder();
            sb.Append(path);
            sb.Append('\\');
            sb.Append(DateTime.Now.Year.ToString());
            sb.Append('_');
            sb.Append(DateTime.Now.Month.ToString());
            sb.Append('_');
            sb.Append(DateTime.Now.Day.ToString());
            sb.Append('_');
            sb.Append(DateTime.Now.Hour.ToString());
            sb.Append('_');
            sb.Append(DateTime.Now.Minute.ToString());
            sb.Append('_');
            sb.Append(DateTime.Now.Second.ToString());
            sb.Append('_');
            sb.Append(DateTime.Now.Ticks.ToString());
            Console.WriteLine(sb);
        }
    }
}
