using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using MySql.Data.MySqlClient;

namespace ImportStations
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
            FileStream newStream = File.OpenRead(@"C:\Users\aksoa\OneDrive - Normandale\Computer Club\Big Data\coop-stations.txt");
            StreamReader reader = new StreamReader(newStream);
            int count = 0;
            while (reader.Peek() >= 0)
            //while (count == 0)
            {
                String line = reader.ReadLine();

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
                    comm.CommandText = "INSERT INTO CPC.Stations(NCDCSTN_ID, COOP_ID, NWSLI_ID, GHCND_ID, NAME_COOP_SHORT, FIPS_COUNTRY_NAME, STATE_PROV, COUNTY, NWS_CLIM_DIV, NWS_CLIM_DIV_NAME, LAT_DEC, LON_DEC, LAT_LON_PRECISION, ELEV_GROUND, ELEV_GROUND_UNIT, UTC_OFFSET, NWS_REGION, NWS_WFO, COOP_SOD, COOP_HPD) VALUES(@col0, @col1, @col2, @col3, @col4, @col5, @col6, @col7, @col8, @col9, @col10, @col11, @col12, @col13, @col14, @col15, @col16, @col17, @col18, @col19)";
                    comm.Parameters.AddWithValue("@col0", line.Substring(0, 8));
                    comm.Parameters.AddWithValue("@col1", line.Substring(9, 6));
                    comm.Parameters.AddWithValue("@col2", line.Substring(16, 8));
                    comm.Parameters.AddWithValue("@col3", line.Substring(25, 15));
                    comm.Parameters.AddWithValue("@col4", line.Substring(41, 30));
                    comm.Parameters.AddWithValue("@col5", line.Substring(72, 20));
                    comm.Parameters.AddWithValue("@col6", line.Substring(93, 2));
                    comm.Parameters.AddWithValue("@col7", line.Substring(96, 50));
                    comm.Parameters.AddWithValue("@col8", line.Substring(147, 2));
                    comm.Parameters.AddWithValue("@col9", line.Substring(150, 40));
                    comm.Parameters.AddWithValue("@col10", line.Substring(191, 15));
                    comm.Parameters.AddWithValue("@col11", line.Substring(207, 15));
                    comm.Parameters.AddWithValue("@col12", line.Substring(223, 10));
                    comm.Parameters.AddWithValue("@col13", line.Substring(234, 8));
                    comm.Parameters.AddWithValue("@col14", line.Substring(243, 6));
                    comm.Parameters.AddWithValue("@col15", line.Substring(250, 3));
                    comm.Parameters.AddWithValue("@col16", line.Substring(254, 10));
                    comm.Parameters.AddWithValue("@col17", line.Substring(265, 3));
                    comm.Parameters.AddWithValue("@col18", line.Substring(269, 1));
                    comm.Parameters.AddWithValue("@col19", line.Substring(271, 1));
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
