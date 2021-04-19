using System.Drawing;
using Console = Colorful.Console;

namespace DDDSJ4.Utilities
{
    public static class Logger
    {
        public static void LogInfo(string input)
        {
            Console.Write("INFO: ", Color.Blue);
            Console.WriteLine(input);
        }

        public static void LogWarn(string input)
        {
            Console.Write("WARNING: ", Color.Gold);
            Console.WriteLine(input);
        }

        public static void LogError(string input)
        {
            Console.Write("ERROR: ", Color.Red);
            Console.WriteLine(input);
        }
    }
}