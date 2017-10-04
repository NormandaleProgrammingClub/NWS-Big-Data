using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using MySql.Data.MySqlClient;

namespace FTPDirectory
{
    class Program
    {
        static MySqlConnection conn;
        static string myConnectionString;

        static void Main(string[] args)
        {
            myConnectionString = "server=70.99.105.198;uid=NCCCPC;pwd=NCCCPCPassword2017;";

            try
            {
                Console.WriteLine("Connecting to server...");
                conn = new MySqlConnection(myConnectionString);
                conn.Open();
                Console.WriteLine("Connected");
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Error connecting:");
                Console.WriteLine(ex.Message);
            }

            FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://ftp.ncdc.noaa.gov/pub/data/noaa/2017/");
            request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
            
            //request.Credentials = new NetworkCredential("anonymous", "janeDoe@contoso.com"); // This example assumes the FTP site uses anonymous logon.  

            FtpWebResponse response = (FtpWebResponse)request.GetResponse();

            Stream responseStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(responseStream);
            //Console.WriteLine(reader.ReadToEnd());
            
            StreamWriter writer = new StreamWriter(@"C:\Users\aksoa\Desktop\stations.txt");

            while (reader.Peek() >= 0)
            {
                String line = reader.ReadLine(); // Gets file in format: 123456-12345-2017.gz, 123456 is USAF_ID, 12345 is WBAN_ID, 2017 is year
                String File = line.Substring(55, 20);
                String USAF = File.Substring(0, 6);
                String WBAN = File.Substring(7, 5);
                int american = 0;
                int florida = 0;

                // Search Stations DB for matching WBAN_ID, see if that record is the state we want
                try
                {
                    string query = "";
                    string USAF2 = USAF;
                    string WBAN2 = WBAN;

                    if (USAF == "999999")
                    {
                        // Query is to use WBAN ID
                        query = "SELECT * FROM CPC.USAF WHERE WBAN_ID=\"" + WBAN + "\"";
                        USAF2 = "      ";
                    }
                    else
                    {
                        // Query is to use USAF ID
                        query = "SELECT * FROM CPC.USAF WHERE USAF_ID=\"" + USAF + "\"";
                        if (WBAN == "99999") WBAN2 = "     ";
                    }

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    MySqlDataReader SQLreader = cmd.ExecuteReader();
                    
                    int ctry = SQLreader.GetOrdinal("CTRY");
                    int st = SQLreader.GetOrdinal("ST");
                    
                    while(SQLreader.Read())
                    {
                        if (SQLreader.GetString(ctry).Trim() == "US") american = 1;
                        if (SQLreader.GetString(st) == "FL") florida = 1;
                    }

                    SQLreader.Close();
                }
                catch (MySqlException ex)
                {
                    Console.WriteLine("    Error pulling data:");
                    Console.WriteLine(ex.Message);
                }

                if (american == 1 && florida == 0) // If American but haven't found Florida, last ditch to search WBAN DB
                {
                    // Search Stations DB for matching WBAN_ID, see if that record is the state we want
                    try
                    {
                        string query = "SELECT * FROM CPC.WBAN WHERE WBAN_ID=\"" + WBAN + "\"";

                        MySqlCommand cmd = new MySqlCommand(query, conn);
                        MySqlDataReader SQLreader = cmd.ExecuteReader();
                        
                        int state = SQLreader.GetOrdinal("STATE_PROVINCE");

                        while (SQLreader.Read())
                        {
                            if (SQLreader.GetString(state) == "FL") florida = 1;
                        }

                        SQLreader.Close();
                    }
                    catch (MySqlException ex)
                    {
                        Console.WriteLine("    Error pulling data:");
                        Console.WriteLine(ex.Message);
                    }
                }
                
                if (american == 1 && florida == 1)
                { // Confirmed file is American and Floridian
                    Console.WriteLine("Importing file " + File);
                    
                }
            }

            Console.WriteLine("Directory List Complete, status {0}", response.StatusDescription);
            Console.ReadLine();

            writer.Close();
            reader.Close();
            response.Close();
        }
    }
}
