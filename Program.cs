using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MySql.Data.MySqlClient;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace robot_authentification
{
    class Urls
    {
        public string Url { get; set; }
    }
    class Tokens
    {
        public string Token { get; set; }
    }
    class Program
    {
        static void WebToBase(string user_URL, string appr_KEY)
        {
            DateTime dateTime = DateTime.Now;
            string connStr = "(localdb)/MSSQLLocalDB;";
            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();
            string sqlcheck = "SELECT website FROM `account` WHERE website='" + user_URL + "'";
            try
            {
                Console.WriteLine("Try: ");
                MySqlCommand commandcheck = new MySqlCommand(sqlcheck, conn);
                // string sql = "UPDATE `authwebsites` SET Number, `website`, `auth`, `token`, `date`) VALUES ('3', '" + user_URL + "', '1', '" + appr_KEY + "', '" + dateTime.ToString("yyyy-MM-dd") + "');";
                //MySqlCommand command = new MySqlCommand(sql, conn);
                //command.ExecuteNonQuery();
                //conn.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("ex: " + ex);
                string sql = "INSERT INTO `authwebsites` (`Number`, `website`, `auth`, `token`, `date`) VALUES ('3', '" + user_URL + "', '1', '" + appr_KEY + "', '" + dateTime.ToString("yyyy-MM-dd") + "');";
                MySqlCommand command = new MySqlCommand(sql, conn);
                command.ExecuteNonQuery();
                conn.Close();
            }
        }

        static void RobotGo(IWebDriver Browser0, string user_URL, string appr_KEY)
        {
            try
            {
                Browser0.Navigate().GoToUrl(user_URL);
                var meta = Browser0.FindElement(By.XPath("//meta[@name='luplats-site-verification']"));
                if (meta.GetAttribute("content") == appr_KEY)
                {
                    Console.WriteLine("Authentification token approved\n");
                    GC.Collect();
                    WebToBase(user_URL, appr_KEY);
                }
                else
                {
                    Console.WriteLine("Token not much\n");
                    GC.Collect();

                }
            }
            catch (StaleElementReferenceException)
            {
                Console.WriteLine("Too much request\n");
                GC.Collect();

            }
            catch (NoSuchElementException)
            {
                Console.WriteLine("Authentification token no match\n");
                GC.Collect();

            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: \n" + ex);
                GC.Collect();

            }
        }
        static async Task Main(string[] args)
        {
            var html = @"http://html-agility-pack.net/";

            HtmlWeb web = new HtmlWeb();

            var htmlDoc = web.Load(html);

            var node = htmlDoc.DocumentNode.SelectSingleNode("//head/title");

            Console.WriteLine("Node Name: " + node.Name + "\n" + node.OuterHtml);
            while (true)
            {
                int index = 0;
                Console.WriteLine("Put 'start' to start program or 'q' to exit");
                string start = Console.ReadLine();
                if (start == "start")
                {

                    IWebDriver Browser0;
                    ChromeOptions headoptions = new OpenQA.Selenium.Chrome.ChromeOptions();
                    headoptions.AddArguments("--headless"); // уйти в инвиз
                    Browser0 = new ChromeDriver(headoptions);
                    List<Urls> urls = new List<Urls>();
                    List<Tokens> tokens = new List<Tokens>();
                    string connStr = "server=(localdb)/MSSQLLocalDB;";
                    MySqlConnection conn = new MySqlConnection(connStr);
                    conn.Open();
                    string sql = "SELECT website,token FROM `account` WHERE emailverf='1'";
                    MySqlCommand command = new MySqlCommand(sql, conn);
                    MySqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        index++;
                        urls.Add(new Urls() { Url = reader[0].ToString() });
                        tokens.Add(new Tokens() { Token = reader[1].ToString() });
                        Console.WriteLine("\nurl: " + reader[0].ToString() + " token: " + reader[1].ToString());
                        await Task.Run(() => RobotGo(Browser0, reader[0].ToString(), reader[1].ToString()));
                    }

                    reader.Close();
                    conn.Close();
                    Browser0.Close();
                    Browser0.Quit();
                    Console.WriteLine("End reader index = " + index);
                }
                if (start == "q")
                {
                    Environment.Exit(0);
                }
            }
        }
    }
}


