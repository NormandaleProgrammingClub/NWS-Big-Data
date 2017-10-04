using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.IO.Compression;
using MySql.Data.MySqlClient;

namespace FTPDirectory
{
    class Program
    {
        static MySqlConnection conn;
        static string myConnectionString;

        static void Main(string[] args)
        {
            string FTPURL = "ftp://ftp.ncdc.noaa.gov/pub/data/noaa/2017/";
            myConnectionString = "server=70.99.105.198;uid=";

            Console.WriteLine("Please enter your MySQL server credentials:");
            Console.Write("Username: ");
            string u = Console.ReadLine();
            Console.Write("Password: ");
            myConnectionString += u + ";pwd=" + ReadPassword() + ";";

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

            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(FTPURL);
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
                String FileName = line.Substring(55, 20);
                String USAF = FileName.Substring(0, 6);
                String WBAN = FileName.Substring(7, 5);
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
                        query = "SELECT * FROM CPC.USAF WHERE WBAN_ID=\"" + WBAN + "\" AND CTRY=\"US\" AND ST=\"FL\"";
                        USAF2 = "      ";
                    }
                    else
                    {
                        // Query is to use USAF ID
                        query = "SELECT * FROM CPC.USAF WHERE USAF_ID=\"" + USAF + "\" AND CTRY=\"US\" AND ST=\"FL\"";
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
                        string query = "SELECT * FROM CPC.WBAN WHERE WBAN_ID=\"" + WBAN + "\" AND CTRY=\"US\" AND ST=\"FL\"";
                        
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
                    Console.WriteLine("Importing file " + FileName);

                    FtpWebRequest request2 = (FtpWebRequest)WebRequest.Create(FTPURL + FileName);
                    request2.Method = WebRequestMethods.Ftp.DownloadFile;

                    FtpWebResponse response2 = (FtpWebResponse)request2.GetResponse();
                    Stream responseStream2 = response2.GetResponseStream();

                    long length = response2.ContentLength;

                    int bytesRead = 0;
                    byte[] buffer = new byte[2048];
                    FileStream fileStream2 = new FileStream(@"C:\Users\aksoa\Desktop\compressed.gz", FileMode.Create);

                    while (true)
                    {
                        bytesRead = responseStream2.Read(buffer, 0, 2048);

                        if (bytesRead == 0)
                            break;

                        fileStream2.Write(buffer, 0, bytesRead);
                    }

                    Console.WriteLine("Done saving.");

                    fileStream2.Seek(0, SeekOrigin.Begin); // Move back to beginning for the decompressor

                    FileStream newStream2 = File.Create(@"C:\Users\aksoa\Desktop\decompressed.txt");
                    using (GZipStream decompressionStream = new GZipStream(fileStream2, CompressionMode.Decompress))
                    {
                        decompressionStream.CopyTo(newStream2);
                        Console.WriteLine("Decompressed file");
                    }

                    newStream2.Seek(0, SeekOrigin.Begin);

                    StreamReader reader2 = new StreamReader(newStream2);
                    while (reader2.Peek() >= 0)
                    {
                        String line2 = reader2.ReadLine();

                        Console.WriteLine("Variable Characters: " + line2.Substring(0, 4));
                        Console.WriteLine("USAF Identifier: " + line2.Substring(4, 6));
                        Console.WriteLine("WBAN Identifier: " + line2.Substring(10, 5));
                        Console.WriteLine(": " + line2.Substring(15, 8));
                        Console.WriteLine(": " + line2.Substring(23, 4));
                        Console.WriteLine(": " + line2.Substring(27, 1));
                        Console.WriteLine(": " + line2.Substring(28, 6));
                        Console.WriteLine(": " + line2.Substring(34, 7));
                        Console.WriteLine(": " + line2.Substring(41, 5));
                        Console.WriteLine(": " + line2.Substring(46, 5));
                        Console.WriteLine(": " + line2.Substring(51, 5));
                        Console.WriteLine(": " + line2.Substring(56, 4));
                        Console.WriteLine(": " + line2.Substring(60, 3));
                        Console.WriteLine(": " + line2.Substring(63, 1));
                        Console.WriteLine(": " + line2.Substring(64, 1));
                        Console.WriteLine(": " + line2.Substring(65, 4));
                        Console.WriteLine(": " + line2.Substring(69, 1));
                        Console.WriteLine(": " + line2.Substring(70, 5));
                        Console.WriteLine(": " + line2.Substring(75, 1));
                        Console.WriteLine(": " + line2.Substring(76, 1));
                        Console.WriteLine(": " + line2.Substring(77, 1));
                        Console.WriteLine(": " + line2.Substring(78, 6));
                        Console.WriteLine(": " + line2.Substring(84, 1));
                        Console.WriteLine(": " + line2.Substring(85, 1));
                        Console.WriteLine(": " + line2.Substring(86, 1));
                        Console.WriteLine(": " + line2.Substring(87, 5));
                        Console.WriteLine(": " + line2.Substring(92, 1));
                        Console.WriteLine(": " + line2.Substring(93, 5));
                        Console.WriteLine(": " + line2.Substring(98, 1));
                        Console.WriteLine(": " + line2.Substring(99, 5));
                        Console.WriteLine(": " + line2.Substring(104, 1));
                        Console.WriteLine("Additional Data: " + line2.Substring(105));

                        Console.ReadLine();
                        Console.WriteLine();
                    }

                    reader2.Close();
                    newStream2.Close();
                    fileStream2.Close();
                    response2.Close();
                }
            }

            Console.WriteLine("Directory List Complete, status {0}", response.StatusDescription);
            Console.ReadLine();

            writer.Close();
            reader.Close();
            response.Close();
        }

        public static string ReadPassword()
        { // http://rajeshbailwal.blogspot.com/2012/03/password-in-c-console-application.html
            string password = "";
            ConsoleKeyInfo info = Console.ReadKey(true);
            while (info.Key != ConsoleKey.Enter)
            {
                if (info.Key != ConsoleKey.Backspace)
                {
                    Console.Write("*");
                    password += info.KeyChar;
                }
                else if (info.Key == ConsoleKey.Backspace)
                {
                    if (!string.IsNullOrEmpty(password))
                    {
                        password = password.Substring(0, password.Length - 1); // remove one character from the list of password characters
                        int pos = Console.CursorLeft; // get the location of the cursor
                        Console.SetCursorPosition(pos - 1, Console.CursorTop); // move the cursor to the left by one character
                        Console.Write(" "); // replace it with space
                        Console.SetCursorPosition(pos - 1, Console.CursorTop); // move the cursor to the left by one character again
                    }
                }
                info = Console.ReadKey(true);
            }
            Console.WriteLine(); // add a new line because user pressed enter at the end of their password
            return password;
        }
    }
}
