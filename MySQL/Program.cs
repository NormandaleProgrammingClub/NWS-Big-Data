using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace MySQL
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

            try
            {
                Console.WriteLine("Pulling data...");
                Console.WriteLine();
                string query = "SELECT col0,col1 FROM CPC.Table";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string someStringFromColumnZero = reader.GetString(0);
                    string someStringFromColumnOne = reader.GetString(1);
                    Console.WriteLine(someStringFromColumnZero + "," + someStringFromColumnOne);
                }
                reader.Close();
                Console.WriteLine();
                Console.WriteLine("Data pulled");
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Error pulling data:");
                Console.WriteLine(ex.Message);
            }
            Console.WriteLine();
            Console.WriteLine();

            try
            {
                Console.WriteLine("Inserting data...");
                MySqlCommand comm = conn.CreateCommand();
                comm.CommandText = "INSERT INTO CPC.Table(col0,col1) VALUES(@col0, @col1)";
                comm.Parameters.AddWithValue("@col0", "Myname");
                comm.Parameters.AddWithValue("@col1", "Myaddress");
                comm.ExecuteNonQuery();
                Console.WriteLine("Data inserted");
            }
            catch(MySqlException ex)
            {
                Console.WriteLine("Error inserting data:");
                Console.WriteLine(ex.Message);
            }

            Console.ReadLine();
        }
    }
}
