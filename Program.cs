using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MySql.Data.MySqlClient;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace robot_authentification
{
    public class Sites
    {
        public string url;
        public string token;
    }
    class Program
    {
        static void Main(string[] args)
        {
            
            
            string connStr = "Server=fclick.mysql.tools;Database=fclick_platform;User Id=fclick_platform;Password=m9E6cn5D1aLZ;";
            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();
            string sql = "SELECT website,token FROM `account` ";//WHERE emailverf='0'
            MySqlCommand command = new MySqlCommand(sql, conn);
            MySqlDataReader reader = command.ExecuteReader();
            try
            {

                while (reader.Read())
                {
                    Sites site = new Sites();
                    site.url = reader[0].ToString();
                    site.token = reader[1].ToString();

                    Thread myThread = new Thread(new ParameterizedThreadStart(DoWork), 1024);
                    myThread.Start(site);
                }
                reader.Close();
                conn.Close();
            }
            catch(StackOverflowException)
            {
                Console.WriteLine("STACK");
            }
            _ = Console.ReadLine();


        }
        public static void DoWork(object obj)
        {

            Stopwatch sw = Stopwatch.StartNew();
            Sites check_site = (Sites)obj;
           
            var web = new HtmlWeb();
          
            try
            {
                var doc = web.Load(check_site.url);
                string token = doc.DocumentNode.SelectSingleNode("//head/meta[@name='luplats-site-verification']").Attributes["value"].Value;
                Console.WriteLine("site: " + check_site.url + ", token: " + check_site.token);
                if (token == check_site.token)
                {
                    Console.WriteLine("SUCCESS: "+ token);
                    Console.WriteLine("Time taken: {0}ms", sw.Elapsed.TotalMilliseconds);
                }
                else
                {
                    Console.WriteLine("FAIL1");
                    Console.WriteLine("Time taken: {0}ms", sw.Elapsed.TotalMilliseconds);
                }
               
            }
            catch {
                Console.WriteLine("site: " + check_site.url + ", token: " + check_site.token);
                Console.WriteLine("FAIL2");
                Console.WriteLine("Time taken: {0}ms", sw.Elapsed.TotalMilliseconds); 
            }
            
            sw.Stop();
            Console.ReadLine();
           
        }
    }
}





//    private HtmlNode node;

//    private static async Task<HtmlNode> LoadWebsitesDocumentNode(string url, string qppr_KEY)
//    {
//        HtmlWeb web = new HtmlWeb();
//        try
//        {
//            var htmlDoc = await web.LoadFromWebAsync(url);
//            var node = htmlDoc.DocumentNode.SelectSingleNode("//head/meta[@name='luplats-site-verification']");
//            if (node.ToString() == qppr_KEY)
//            {
//                Console.WriteLine(node.Attributes["value"].Value);
//            }
//            else
//            {
//                Console.WriteLine("GOOD");
//            }
//            return node;
//        }
//        catch (HtmlAgilityPack.HtmlWebException ex)
//        {
//            return null;
//        }
//        catch (Exception Ex)
//        {
//            return null;
//        }
//    }
//    static bool RobotGo2(string user_URL, string qppr_KEY)
//    {


//        try
//        {
//            var html = user_URL;

//            HtmlWeb web = new HtmlWeb();
//            var htmlDoc = web.Load(html);



//        }
//        catch
//        {
//            Console.WriteLine("FIG");
//        }
//        return false;
//    }



//    static async Task Main(string[] args)
//    {

//        while (true)
//        {

//            int index = 0;
//            Console.WriteLine("Put 'start' to start program or 'q' to exit");
//            string start = Console.ReadLine();
//            if (start == "start")
//            {
//                Stopwatch sw = Stopwatch.StartNew();
//                List<Urls> urls = new List<Urls>();
//                List<Tokens> tokens = new List<Tokens>();
//                string connStr = "Server=fclick.mysql.tools;Database=fclick_platform;User Id=fclick_platform;Password=m9E6cn5D1aLZ;";
//                MySqlConnection conn = new MySqlConnection(connStr);
//                conn.Open();
//                string sql = "SELECT website,token FROM `account` ";//WHERE emailverf='1'
//                MySqlCommand command = new MySqlCommand(sql, conn);
//                MySqlDataReader reader = command.ExecuteReader();

//                while (reader.Read())
//                {
//                    index++;
//                    urls.Add(new Urls() { Url = reader[0].ToString() });
//                    tokens.Add(new Tokens() { Token = reader[1].ToString() });
//                    Console.WriteLine("\nurl: " + reader[0].ToString() + " token: " + reader[1].ToString());

//                    await LoadWebsitesDocumentNode(reader[0].ToString(), reader[1].ToString());
//                }
//                //await Task.Run(() => RobotGo2(reader[0].ToString(), reader[1].ToString()));


//                reader.Close();
//                conn.Close();
//                sw.Stop();
//                Console.WriteLine("Time taken: {0}ms", sw.Elapsed.TotalMilliseconds);
//                Console.WriteLine("End reader index = " + index);
//            }
//            if (start == "q")
//            {
//                Environment.Exit(0);
//            }
//        }
//    }
//}



//static void RobotGo(IWebDriver Browser0, string user_URL, string appr_KEY)
//{
//    try
//    {
//        Browser0.Navigate().GoToUrl(user_URL);
//        var meta = Browser0.FindElement(By.XPath("//meta[@name='luplats-site-verification']"));
//        if (meta.GetAttribute("content") == appr_KEY)
//        {
//            Console.WriteLine("Authentification token approved\n");
//            GC.Collect();
//            WebToBase(user_URL, appr_KEY);
//        }
//        else
//        {
//            Console.WriteLine("Token not much\n");
//            GC.Collect();

//        }
//    }
//    catch (StaleElementReferenceException)
//    {
//        Console.WriteLine("Too much request\n");
//        GC.Collect();

//    }
//    catch (NoSuchElementException)
//    {
//        Console.WriteLine("Authentification token no match\n");
//        GC.Collect();

//    }
//    catch (Exception ex)
//    {
//        Console.WriteLine("Exception: \n" + ex);
//        GC.Collect();

//    }
//}
//static void WebToBase(string user_URL, string appr_KEY)
//{
//    DateTime dateTime = DateTime.Now;
//    string connStr = "(localdb)/MSSQLLocalDB;";
//    MySqlConnection conn = new MySqlConnection(connStr);
//    conn.Open();
//    string sqlcheck = "SELECT website FROM `account` WHERE website='" + user_URL + "'";
//    try
//    {
//        Console.WriteLine("Try: ");
//        MySqlCommand commandcheck = new MySqlCommand(sqlcheck, conn);
//        // string sql = "UPDATE `authwebsites` SET Number, `website`, `auth`, `token`, `date`) VALUES ('3', '" + user_URL + "', '1', '" + appr_KEY + "', '" + dateTime.ToString("yyyy-MM-dd") + "');";
//        //MySqlCommand command = new MySqlCommand(sql, conn);
//        //command.ExecuteNonQuery();
//        //conn.Close();
//    }
//    catch (Exception ex)
//    {
//        Console.WriteLine("ex: " + ex);
//        string sql = "INSERT INTO `authwebsites` (`Number`, `website`, `auth`, `token`, `date`) VALUES ('3', '" + user_URL + "', '1', '" + appr_KEY + "', '" + dateTime.ToString("yyyy-MM-dd") + "');";
//        MySqlCommand command = new MySqlCommand(sql, conn);
//        command.ExecuteNonQuery();
//        conn.Close();
//    }
//}

