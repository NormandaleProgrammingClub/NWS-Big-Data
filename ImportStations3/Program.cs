using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using MySql.Data.MySqlClient;

namespace ImportStations3
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
            Console.WriteLine();
            Console.WriteLine();

            Console.WriteLine("Importing stations");
            FileStream newStream = File.OpenRead(@"C:\Users\aksoa\OneDrive - Normandale\Computer Club\Big Data\isd-history (Has USAF_ID).txt");
            StreamReader reader = new StreamReader(newStream);
            int count = 0;

            for (int i = 0; i < 22; i++) // Skip 22 lines
            {
                reader.ReadLine(); // Skip line
            }
            while (reader.Peek() >= 0)
            //while (count == 0)
            {
                String line = reader.ReadLine();
                Console.WriteLine(line);

                /*Console.WriteLine("USAF_ID: " + line.Substring(0, 6));
                Console.WriteLine("WBAN_ID: " + line.Substring(7, 5));
                Console.WriteLine("STATION_NAME: " + line.Substring(13, 29));
                Console.WriteLine("CTRY: " + line.Substring(43, 4));
                Console.WriteLine("ST: " + line.Substring(48, 2));
                Console.WriteLine("CALL: " + line.Substring(51, 5));
                Console.WriteLine("LAT: " + line.Substring(57, 7));
                Console.WriteLine("LON: " + line.Substring(65, 8));
                Console.WriteLine("ELEV: " + line.Substring(74, 7));
                Console.WriteLine("BEGIN: " + line.Substring(82, 8));
                Console.WriteLine("END: " + line.Substring(91, 8));*/

                try
                {
                    MySqlCommand comm = conn.CreateCommand();
                    comm.CommandText = "INSERT INTO CPC.USAF(USAF_ID, WBAN_ID, STATION_NAME, CTRY, ST, ICAO_CALL, LAT, LON, ELEV, BEGIN, END) VALUES(@col0, @col1, @col2, @col3, @col4, @col5, @col6, @col7, @col8, @col9, @col10)";
                    comm.Parameters.AddWithValue("@col0", line.Substring(0, 6));
                    comm.Parameters.AddWithValue("@col1", line.Substring(7, 5));
                    comm.Parameters.AddWithValue("@col2", line.Substring(13, 29));
                    comm.Parameters.AddWithValue("@col3", line.Substring(43, 4));
                    comm.Parameters.AddWithValue("@col4", line.Substring(48, 2));
                    comm.Parameters.AddWithValue("@col5", line.Substring(51, 5));
                    comm.Parameters.AddWithValue("@col6", line.Substring(57, 7));
                    comm.Parameters.AddWithValue("@col7", line.Substring(65, 8));
                    comm.Parameters.AddWithValue("@col8", line.Substring(74, 7));
                    comm.Parameters.AddWithValue("@col9", line.Substring(82, 8));
                    comm.Parameters.AddWithValue("@col10", line.Substring(91, 8));
                    comm.ExecuteNonQuery();
                }
                catch (MySqlException ex)
                {
                    Console.WriteLine(ex.Message);
                }

                //Console.ReadLine();
                //Console.WriteLine();
            }

            Console.WriteLine("Finished importing");

            reader.Close();
            newStream.Close();
            conn.Close();
            Console.ReadLine();
        }
    }
}
