using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VkPointChecker.Data;

namespace VkPointChecker
{
    class Program
    {
        static List<string> AllPointsList;
        static List<string> ExistingPointsList;

        static void Main(string[] args)
        {
            AllPointsList = GetAllPoints();
            ExistingPointsList = GetExistingPoints();
            
            var differents = AllPointsList.Except(ExistingPointsList);

            int x = 1;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("============= Точки которых нет на платформе =============");
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            foreach (var d in differents)
            {
                Console.WriteLine("{0}: {1}", x, d);
                x++;
            }

            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Всего не хватает {0} точек", x);

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");

            var sameEtries = AllPointsList.Except(AllPointsList.Except(ExistingPointsList));

            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("============= Одинаковые записи =============");

            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.DarkGreen;

            int y = 1;
            foreach (var s in sameEtries)
            {
                Console.WriteLine("{0}: {1}", y, s);
                y++;
            }

            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Всего одинаковых {0} точек", y);

            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");

            var rDifferents = ExistingPointsList.Except(AllPointsList);

            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("============= Точки котрорых не в общем списке =============");

            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.DarkGreen;

            int z = 1;
            foreach (var r in rDifferents)
            {
                Console.WriteLine("{0}: {1}", z, r);
                z++;
            }

            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Всего {0} точек", z);

            Console.ResetColor();
            Console.ReadLine();
        }

        static List<string> GetAllPoints()
        {
            var result = new List<string>();

            try
            {
                using (var db = new PointDbEntities())
                {
                    result.AddRange(db.PointListItems.Select(x => x.GLN));
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return result;
        }

        static List<string> GetExistingPoints()
        {
            var result = new List<string>();

            try
            {
                using (var db = new PointDbEntities())
                {
                    result.AddRange(db.CurrentPoints.Where(p => p.Customer == "Фудком(36)").Select(x => x.GLN));
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return result;
        }
    }
}
