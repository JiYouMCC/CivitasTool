namespace MCCCivitasBlackTech
{
    using System;

    class ConsoleHelpers
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
    }
}
