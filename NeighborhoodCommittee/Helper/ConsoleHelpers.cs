namespace MCCCivitasBlackTech
{
    using System;

    public class ConsoleHelpers
    {
        public static void OutPersent(double persent)
        {
            try
            {
                Console.SetCursorPosition(0, Console.CursorTop);
                Console.Write(new string(' ', Console.WindowWidth));
                Console.SetCursorPosition(0, Console.CursorTop - 1);
                int y = (int)persent / 2;
                int n = 50 - y;
                Console.Write("[");
                for (int i = 0; i < y; i++)
                {
                    Console.Write("=");
                }
                for (int i = 0; i < n; i++)
                {
                    Console.Write(" ");
                }

                Console.Write("]");
                Console.Write(" {0:0.00}%", persent);
            }
            catch
            {

            }
        }

        public static string GetInput(bool pwdArea)
        {
            string result = string.Empty;

            while (true)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(pwdArea);
                if (keyInfo.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    break;
                }

                if (pwdArea)
                {
                    Console.Write("*");
                }
                result += keyInfo.KeyChar;
            }
            
            return result;
        }
    }
}
