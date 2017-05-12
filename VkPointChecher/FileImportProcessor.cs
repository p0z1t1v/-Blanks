using LinqToExcel;
using LinqToExcel.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VkPointChecker.Data;

namespace VkPointChecker
{
    public static class FileImportProcessor
    {
        private const string Path = @"E:\VK_GLN_Points.xlsx";
        public static int Count = 0;
        public static void ListFile()
        {
             var listItems = new ExcelQueryFactory(Path)
             {
                 DatabaseEngine = LinqToExcel.Domain.DatabaseEngine.Ace,
                 TrimSpaces = LinqToExcel.Query.TrimSpacesType.Both,
                 UsePersistentConnection = true,
                 ReadOnly = true

             };

             var pointsList = from a in listItems.Worksheet<PointItem>("Sheet1") select a;

            using(var db = new PointDbEntities())
            {
                foreach (PointItem pointItem in pointsList)
                {
                    if(pointItem.Adress != null)
                    {
                        PointListItems item = new PointListItems()
                        {
                            Id = Guid.NewGuid(),
                            Adress = pointItem.Adress,
                            GLN = pointItem.Gln

                        };

                        db.PointListItems.Add(item);
                        db.SaveChanges();
                        Count++;
                    }
                }
            }
        }

        internal class PointItem
        {
            [ExcelColumn("Adress")]
            public string Adress { get; set; }

            [ExcelColumn("GLN")]
            public string Gln { get; set; }
        }

    }
}
