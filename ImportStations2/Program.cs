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
            FileStream newStream = File.OpenRead(@"C:\Users\aksoa\OneDrive - Normandale\Computer Club\Big Data\wbanmasterlist.psv\wbanmasterlist.psv");
            StreamReader reader = new StreamReader(newStream);
            int count = 0;

            reader.ReadLine(); // Skip line
            while (reader.Peek() >= 0)
            //while (count == 0)
            {
                String line = reader.ReadLine();
                //line = String.Format("\"{0}\"", line.Replace("|", "").Replace("\r", "").Replace("\n", ""));
                String line2 = String.Format("{0}", line.Replace("\"|", "$%&").Replace("\"", "").Replace("|", "").Replace("$%&", "|"));

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

                /*Console.WriteLine("NCDCSTN_ID: " + line.Substring(0, 8));
                Console.WriteLine("COOP_ID: " + line.Substring(9, 6));
                Console.WriteLine("NWSLI_ID: " + line.Substring(16, 8));
                Console.WriteLine("GHCND_ID: " + line.Substring(25, 15));
                Console.WriteLine("NAME_COOP_SHORT: " + line.Substring(41, 30));
                Console.WriteLine("FIPS_COUNTRY_NAME: " + line.Substring(72, 20));
                Console.WriteLine("STATE_PROV: " + line.Substring(93, 2));
                Console.WriteLine("COUNTY: " + line.Substring(96, 50));
                Console.WriteLine("NWS_CLIM_DIV: " + line.Substring(147, 2));
                Console.WriteLine("NWS_CLIM_DIV_NAME: " + line.Substring(150, 40));
                Console.WriteLine("LAT_DEC: " + line.Substring(191, 15));
                Console.WriteLine("LON_DEC: " + line.Substring(207, 15));
                Console.WriteLine("LAT_LON_PRECISION: " + line.Substring(223, 10));
                Console.WriteLine("ELEV_GROUND: " + line.Substring(234, 8));
                Console.WriteLine("ELEV_GROUND_UNIT: " + line.Substring(243, 6));
                Console.WriteLine("UTC_OFFSET: " + line.Substring(250, 3));
                Console.WriteLine("NWS_REGION: " + line.Substring(254, 10));
                Console.WriteLine("NWS_WFO: " + line.Substring(265, 3));
                Console.WriteLine("COOP_SOD: " + line.Substring(269, 1));
                Console.WriteLine("COOP_HPD: " + line.Substring(271, 1));*/

                try
                {
                    MySqlCommand comm = conn.CreateCommand();
                    comm.CommandText = "INSERT INTO CPC.WBAN(REGION, WBAN_ID, STATION_NAME, STATE_PROVINCE, COUNTY, COUNTRY, EXTENDED_NAME, CALL_SIGN, STATION_TYPE, DATE_ASSIGNED, BEGIN_DATE, COMMENTS, LOCATION, ELEV_OTHER, ELEV_GROUND, ELEV_RUNWAY, ELEV_BAROMETRIC, ELEV_STATION, ELEV_UPPER_AIR) VALUES(@col0, @col1, @col2, @col3, @col4, @col5, @col6, @col7, @col8, @col9, @col10, @col11, @col12, @col13, @col14, @col15, @col16, @col17, @col18)";
                    comm.Parameters.AddWithValue("@col0", lines[0]);
                    comm.Parameters.AddWithValue("@col1", lines[1]);
                    comm.Parameters.AddWithValue("@col2", lines[2]);
                    comm.Parameters.AddWithValue("@col3", lines[3]);
                    comm.Parameters.AddWithValue("@col4", lines[4]);
                    comm.Parameters.AddWithValue("@col5", lines[5]);
                    comm.Parameters.AddWithValue("@col6", lines[6]);
                    comm.Parameters.AddWithValue("@col7", lines[7]);
                    comm.Parameters.AddWithValue("@col8", lines[8]);
                    comm.Parameters.AddWithValue("@col9", lines[9]);
                    comm.Parameters.AddWithValue("@col10", lines[10]);
                    comm.Parameters.AddWithValue("@col11", lines[11]);
                    comm.Parameters.AddWithValue("@col12", lines[12]);
                    comm.Parameters.AddWithValue("@col13", lines[13]);
                    comm.Parameters.AddWithValue("@col14", lines[14]);
                    comm.Parameters.AddWithValue("@col15", lines[15]);
                    comm.Parameters.AddWithValue("@col16", lines[16]);
                    comm.Parameters.AddWithValue("@col17", lines[17]);
                    comm.Parameters.AddWithValue("@col18", lines[18]);
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
