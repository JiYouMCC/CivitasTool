//-----------------------------------------------------------------------
// <copyright file="Main.cs">
//     Company copyright tag.
// </copyright>
//-----------------------------------------------------------------------
namespace Consoletest
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using MCCCivitasBlackTech;

    internal class MainClass
    {
        /// <summary>
        /// The entry point of the program, where the program control starts and ends.
        /// </summary>
        /// <param name="args">The command-line arguments.</param>
        public static void Main(string[] args)
        {
            if (args.Length < 2) 
            {
            Console.WriteLine("老兄啊，不打用户名密码怎么潜入啊……格式就是后面加上空格email空格密码");
            return;
            }

            User user = new User(args[0], args[1]); 
            if (user.Login())
            {
                user.FindSpeech ();
//                Console.WriteLine("Enter succeed!");
//                user.FindNeighbor();
//                while (!user.Ready)
//                {
//                    Console.Write(".");
//                    Thread.Sleep(500);
//                }
//
//                Console.WriteLine("\nTarget City:" + user.City);
//                foreach (Neighbor neighbor in user.Neighbors)
//                {
//                    Console.WriteLine("Target Neighbor:" + neighbor.Name);
//                    neighbor.SeeNeighbor(user);
//                }
//
//                while (!user.NeighborsReady)
//                {
//                    Thread.Sleep(500);
//                }
//
//                Console.WriteLine("\nXml File:");
//                foreach (string path in XmlOut.NeighborOut(user))
//                {
//                    Console.WriteLine(path);
//                }
//
//                Console.WriteLine("\nBOSS，你想拿哪个片区开刀，报数字，我来帮你搞定！\n");
//                foreach (Neighbor neighbor in user.Neighbors)
//                {
//                    Console.WriteLine("【" + neighbor.Name + "】" + neighbor.Id);
//                }
//
//                Console.WriteLine("【放他们一马】q（这个好像不是数字，有奇怪的东西混进来了）");
//                string c = Console.ReadLine();
//                if (c == "q" || c == "Q")
//                {
//                    Console.WriteLine("\n回老家结婚了！");
//                }
//                else
//                {
//                    Neighbor n = null;
//                    foreach (Neighbor neighbor in user.Neighbors)
//                    {
//                        if (c == neighbor.Id.ToString())
//                        {
//                            n = neighbor;
//                            TaxStandard standard=TaxStandard.LoadStandard("");
//                            n.CheckTax(standard);
//                        }
//                    }
//
//                    if (null == n)
//                    {
//                        Console.WriteLine("\nBoss，我们迷路了。。。");
//                    }
//                }
            }
            else
            {
                Console.WriteLine("潜入失败，肯定是你的打开方式不对！");
            }
        }
    }
}