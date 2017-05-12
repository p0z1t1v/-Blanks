using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VkPointChecker;
using VkPointChecker.Data;

namespace VkPointChecker
{
    public static class CurrentPointsGrabber
    {

        private static int TotalPointCounter;
        private static FirefoxDriver Driver = new FirefoxDriver();

        public static void GetCurrentPoints()
        {
            string[] customerName = { "Фудком(36)", "Фудмережа(20)", "Альвар(19)", "Фудмаркет(5)" };
            string[] customerUrl = { "https://edi.ifin.ua/Customer/Areas/36", "https://edi.ifin.ua/Customer/Areas/20",
                "https://edi.ifin.ua/Customer/Areas/19", "https://edi.ifin.ua/Customer/Areas/5" };

            Console.WriteLine("Login");

            var logSucces = Login();
            Console.WriteLine("Login success");
            Thread.Sleep(1000);

            if (logSucces)
            { 
                for (int i = 0; i < 4; i++)
                {
                    ProcessList(customerUrl[i], customerName[i]);
                }
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Processing Complete. Total Points Added: {0}", TotalPointCounter);
                Console.ResetColor();
            }
            Driver.Close();
        }
        

        private static bool Login()
        {
                try
                {
                Driver.Navigate().GoToUrl("https://edi.ifin.ua/");
                    var login = Driver.FindElementByXPath("//form[@id = 'LoginForm']/input[@id = 'UserName']");
                    var pass = Driver.FindElementByXPath("//form[@id = 'LoginForm']/input[@id = 'Password']");

                    login.SendKeys("kritsky.o.a@manager.ifin.ua");
                    pass.SendKeys("111111");

                    var loginBtn = Driver.FindElementByXPath("//form[@id = 'LoginForm']/input[@class = 'button']");
                    loginBtn.Click();

                    return true;
                }
                catch(Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                    Console.ResetColor();
                    return false;
                }
        }

        private static void ProcessList(string customerUrl, string customerName)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Start Porcessing Points. Customer: {0}", customerName);
            Console.ResetColor();

            PointDbEntities db = new PointDbEntities();

            var currentPointsList = new List<CurrentPoints>();

            try
            {
                Driver.Navigate().GoToUrl(customerUrl);

                var table = Driver.FindElementByXPath("//table");
                var rows = table.FindElements(By.TagName("td"));

                int i = 0;

                for (int z = 0; z < rows.Count;)
                {
                    CurrentPoints cPoint = new CurrentPoints();
                    for (int a = 0; a < 4; a++)
                        {
                            if (a == 0)
                            {
                                cPoint.Adress = rows[z].Text.ToString();
                            }
                            else if (a == 1)
                            {
                                cPoint.GLN = rows[z].Text.ToString();
                            }
                            else if (a == 3)
                            {
                                cPoint.Id = Guid.NewGuid();
                                cPoint.Customer = customerName;

                            currentPointsList.Add(cPoint);
                            Console.WriteLine("{0} Add point: adress {1}, Gln {2}, Customer {3}",
                                    i, cPoint.Adress, cPoint.GLN, cPoint.Customer);
                                TotalPointCounter++;
                            }
                            z++;
                        }
                        i++;
                    }
                
                    db.CurrentPoints.AddRange(currentPointsList);
            }
            catch(Exception ex)
            {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Error!!! {0}", ex.Message);
                    Console.WriteLine(ex.StackTrace);
                    Console.ResetColor();
            }
            try
            {
                db.SaveChanges();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                Exception raise = dbEx;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        string message = string.Format("{0}:{1}",
                            validationErrors.Entry.Entity.ToString(),
                            validationError.ErrorMessage);
                        // raise a new exception nesting
                        // the current instance as InnerException
                        raise = new InvalidOperationException(message, raise);
                    }
                }
                throw raise;
            }
        }
    }
}
