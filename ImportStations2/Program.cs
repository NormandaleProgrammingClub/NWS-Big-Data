using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using MySql.Data.MySqlClient;

namespace ImportStations2
{
    class Program
    {
        static MySqlConnection conn;
        static string myConnectionString;

        static void Main(string[] args)
        {
            // text = String.Format("\"{0}\"", text.Replace("|","").Replace("\r","").Replace("\n","");

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
            FileStream newStream = File.OpenRead(@"C:\Users\aksoa\OneDrive - Normandale\Computer Club\Big Data\Stations WBAN List\wbanmasterlist.psv\wbanmasterlist.psv");
            StreamReader reader = new StreamReader(newStream);
            int count = 0;

            reader.ReadLine(); // Skip line
            while (reader.Peek() >= 0)
            //while (count == 0)
            {
                String line = reader.ReadLine();
                //line = String.Format("\"{0}\"", line.Replace("|", "").Replace("\r", "").Replace("\n", ""));
                //String line2 = String.Format("{0}", line.Replace("\"|", "$%&").Replace("\"", "").Replace("|", "").Replace("$%&", "|"));
                String line2 = String.Format("{0}", line.Replace("\r", "").Replace("\n", ""));

                //Console.WriteLine(line2);

                String[] lines = new String[20];
                for(int i=0; i<20; i++)
                {
                    lines[i] = "";
                }
                line2.Split('|').CopyTo(lines, 0);

                /*for(int i=0; i<lines.Length; i++)
                {
                    Console.WriteLine("    " + i + ": " + lines[i]);
                }
                Console.WriteLine();

                Console.WriteLine("    " + lines.Length);*/

                /*Console.WriteLine("REGION: " + lines[0].Trim('"'));
                Console.WriteLine("WBAN_ID: " + lines[1].Trim('"'));
                Console.WriteLine("STATION_NAME: " + lines[2].Trim('"'));
                Console.WriteLine("STATE_PROVINCE: " + lines[3].Trim('"'));
                Console.WriteLine("COUNTY: " + lines[4].Trim('"'));
                Console.WriteLine("COUNTRY: " + lines[5].Trim('"'));
                Console.WriteLine("EXTENDED_NAME: " + lines[6].Trim('"'));
                Console.WriteLine("CALL_SIGN: " + lines[7].Trim('"'));
                Console.WriteLine("STATION_TYPE: " + lines[8].Trim('"'));
                Console.WriteLine("DATE_ASSIGNED: " + lines[9].Trim('"'));
                Console.WriteLine("BEGIN_DATE: " + lines[10].Trim('"'));
                Console.WriteLine("COMMENTS: " + lines[11].Trim('"'));
                Console.WriteLine("LOCATION: " + lines[12].Trim('"'));
                Console.WriteLine("ELEV_OTHER: " + lines[13].Trim('"'));
                Console.WriteLine("ELEV_GROUND: " + lines[14].Trim('"'));
                Console.WriteLine("ELEV_RUNWAY: " + lines[15].Trim('"'));
                Console.WriteLine("ELEV_BAROMETRIC: " + lines[16].Trim('"'));
                Console.WriteLine("ELEV_STATION: " + lines[17].Trim('"'));
                Console.WriteLine("ELEV_UPPER_AIR: " + lines[18].Trim('"'));*/

                try
                {
                    MySqlCommand comm = conn.CreateCommand();
                    comm.CommandText = "INSERT INTO CPC.WBAN(REGION, WBAN_ID, STATION_NAME, STATE_PROVINCE, COUNTY, COUNTRY, EXTENDED_NAME, CALL_SIGN, STATION_TYPE, DATE_ASSIGNED, BEGIN_DATE, COMMENTS, LOCATION, ELEV_OTHER, ELEV_GROUND, ELEV_RUNWAY, ELEV_BAROMETRIC, ELEV_STATION, ELEV_UPPER_AIR) VALUES(@col0, @col1, @col2, @col3, @col4, @col5, @col6, @col7, @col8, @col9, @col10, @col11, @col12, @col13, @col14, @col15, @col16, @col17, @col18)";
                    comm.Parameters.AddWithValue("@col0", lines[0].Trim('"'));
                    comm.Parameters.AddWithValue("@col1", lines[1].Trim('"'));
                    comm.Parameters.AddWithValue("@col2", lines[2].Trim('"'));
                    comm.Parameters.AddWithValue("@col3", lines[3].Trim('"'));
                    comm.Parameters.AddWithValue("@col4", lines[4].Trim('"'));
                    comm.Parameters.AddWithValue("@col5", lines[5].Trim('"'));
                    comm.Parameters.AddWithValue("@col6", lines[6].Trim('"'));
                    comm.Parameters.AddWithValue("@col7", lines[7].Trim('"'));
                    comm.Parameters.AddWithValue("@col8", lines[8].Trim('"'));
                    comm.Parameters.AddWithValue("@col9", lines[9].Trim('"'));
                    comm.Parameters.AddWithValue("@col10", lines[10].Trim('"'));
                    comm.Parameters.AddWithValue("@col11", lines[11].Trim('"'));
                    comm.Parameters.AddWithValue("@col12", lines[12].Trim('"'));
                    comm.Parameters.AddWithValue("@col13", lines[13].Trim('"'));
                    comm.Parameters.AddWithValue("@col14", lines[14].Trim('"'));
                    comm.Parameters.AddWithValue("@col15", lines[15].Trim('"'));
                    comm.Parameters.AddWithValue("@col16", lines[16].Trim('"'));
                    comm.Parameters.AddWithValue("@col17", lines[17].Trim('"'));
                    comm.Parameters.AddWithValue("@col18", lines[18].Trim('"'));
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
