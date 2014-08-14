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
                Console.WriteLine("Enter succeed!");
                user.FindNeighbor();
                while (!user.Ready)
                {
                    Console.Write(".");
                    Thread.Sleep(500);
                }

                Console.WriteLine("\nTarget City:" + user.City);
                foreach (Neighbor neighbor in user.Neighbors)
                {
                    Console.WriteLine("Target Neighbor:" + neighbor.Name);
                    neighbor.SeeNeighbor(user);
                }

                while (!user.NeighborsReady)
                {
                    Thread.Sleep(500);
                }

                Console.WriteLine("\nXml File:");
                foreach (string path in XmlOut.NeighborOut(user))
                {
                    Console.WriteLine(path);
                }

                Console.WriteLine("\nBOSS，你想拿哪个片区开刀，报数字，我来帮你搞定！\n");
                foreach (Neighbor neighbor in user.Neighbors)
                {
                    Console.WriteLine("【" + neighbor.Name + "】" + neighbor.Id);
                }

                Console.WriteLine("【放他们一马】q（这个好像不是数字，有奇怪的东西混进来了）");
                string c = Console.ReadLine();
                if (c == "q" || c == "Q")
                {
                    Console.WriteLine("\n回老家结婚了！");
                }
                else
                {
                    Neighbor n = null;
                    foreach (Neighbor neighbor in user.Neighbors)
                    {
                        if (c == neighbor.Id.ToString())
                        {
                            n = neighbor;
                            Console.WriteLine(neighbor.Name + "受死吧！");
                            List<EstateType> estateTypeList = new List<EstateType>();
                            foreach (Estate estate in neighbor.Estates)
                            {
                                if (!estateTypeList.Contains(estate.Type))
                                {
                                    estateTypeList.Add(estate.Type);
                                }
                            }

                            estateTypeList.Sort();

                            Console.WriteLine("\n收税标准：价格/面积");

                            Dictionary<EstateType, double> p = new Dictionary<EstateType, double>();
                            foreach (EstateType type in estateTypeList)
                            {
                                Console.WriteLine(type.Name);
                                Console.Write("价格：");
                                double temp1 = Convert.ToDouble(Console.ReadLine());
                                Console.Write("面积：");
                                double temp2 = Convert.ToDouble(Console.ReadLine());
                                double result = temp1 / temp2;
                                if (result > 0)
                                {
                                    p.Add(type, result);
                                }
                            }

                            foreach (KeyValuePair<EstateType, double> a in p)
                            {
                                Console.WriteLine(a.Key.Name + " " + a.Value.ToString());
                            }                          

                            Dictionary<string, double> pp = new Dictionary<string, double>();
                            foreach (Estate estate in neighbor.Estates)
                            {
                                if (p.ContainsKey(estate.Type))
                                {
                                    double ppp = 0;
                                    p.TryGetValue(estate.Type, out ppp);
                                    double rr = estate.Area * ppp;
                                    if (!pp.ContainsKey(estate.Owner))
                                    {
                                        pp.Add(estate.Owner, rr);
                                    }
                                    else
                                    {
                                        double old = 0;
                                        pp.TryGetValue(estate.Owner, out old);
                                        pp.Remove(estate.Owner);
                                        pp.Add(estate.Owner, old + rr);
                                    }
                                }
                            }

                            foreach (KeyValuePair<string, double> a in pp)
                            {
                                Console.WriteLine(a.Key + " " + a.Value.ToString());
                            }
                        }
                    }

                    if (null == n)
                    {
                        Console.WriteLine("\nBoss，我们迷路了。。。");
                    }
                }
            }
            else
            {
                Console.WriteLine("潜入失败，肯定是你的打开方式不对！");
            }
        }
    }
}