using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MySql.Data.MySqlClient;

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
            
            
            string connStr = "";
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
