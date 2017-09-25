using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.IO.Compression;
using MySql.Data.MySqlClient;

namespace FTPDownloadAndExtract
{
    class Program
    {
        static void Main(string[] args)
        {
            // NCCCPC MySQL Server IP: 192.168.10.198

            ftp://ftp.ncdc.noaa.gov/pub/data/noaa/
            // Get the object used to communicate with the server.  
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://ftp.ncdc.noaa.gov/pub/data/noaa/2017/007026-99999-2017.gz");
            request.Method = WebRequestMethods.Ftp.DownloadFile;

            // This example assumes the FTP site uses anonymous logon.  
            //request.Credentials = new NetworkCredential("anonymous", "janeDoe@contoso.com");

            FtpWebResponse response = (FtpWebResponse)request.GetResponse();

            Stream responseStream = response.GetResponseStream();
            /*StreamReader reader = new StreamReader(responseStream);
            Console.WriteLine(reader.ReadToEnd());

            Console.WriteLine("Download Complete, status {0}", response.StatusDescription);*/

            /*FileStream fileStream = File.Create(@"C:\compressed.gz", (int)responseStream.Length);
            // Initialize the bytes array with the stream length and then fill it with data
            byte[] bytesInStream = new byte[responseStream.Length];
            responseStream.Read(bytesInStream, 0, bytesInStream.Length);
            // Use write method to write to the file specified above
            fileStream.Write(bytesInStream, 0, bytesInStream.Length);*/

            /*Console.WriteLine("CanSeek: " + responseStream.CanSeek);

            if (!responseStream.CanSeek)
            {
                Console.ReadLine();
                return;
            }*/

            //StreamReader reader = new StreamReader(responseStream);

            long length = response.ContentLength;

            int bytesRead = 0;
            byte[] buffer = new byte[2048];
            FileStream fileStream = new FileStream(@"C:\Users\aksoa\Desktop\compressed.gz", FileMode.Create);

            while (true)
            {
                bytesRead = responseStream.Read(buffer, 0, 2048);

                if (bytesRead == 0)
                    break;

                fileStream.Write(buffer, 0, bytesRead);
            }

            Console.WriteLine("Done saving.");

            fileStream.Seek(0, SeekOrigin.Begin); // Move back to beginning for the decompressor
            //fileStream.Close();

            //FileStream fileStream2 = new FileStream(@"C:\Users\aksoa\Desktop\compressed.gz", FileMode.Open);
            FileStream newStream = File.Create(@"C:\Users\aksoa\Desktop\decompressed.txt");
            using (GZipStream decompressionStream = new GZipStream(fileStream, CompressionMode.Decompress))
            {
                decompressionStream.CopyTo(newStream);
                Console.WriteLine("Decompressed file");
            }

            newStream.Seek(0, SeekOrigin.Begin);
            
            StreamReader reader = new StreamReader(newStream);
            while (reader.Peek() >= 0)
            {
                String line = reader.ReadLine();

                Console.WriteLine("Variable Characters: " + line.Substring(0, 4));
                Console.WriteLine("USAF Identifier: " + line.Substring(4, 6));
                Console.WriteLine("WBAN Identifier: " + line.Substring(10, 5));
                Console.WriteLine(": " + line.Substring(15, 8));
                Console.WriteLine(": " + line.Substring(23, 4));
                Console.WriteLine(": " + line.Substring(27, 1));
                Console.WriteLine(": " + line.Substring(28, 6));
                Console.WriteLine(": " + line.Substring(34, 7));
                Console.WriteLine(": " + line.Substring(41, 5));
                Console.WriteLine(": " + line.Substring(46, 5));
                Console.WriteLine(": " + line.Substring(51, 5));
                Console.WriteLine(": " + line.Substring(56, 4));
                Console.WriteLine(": " + line.Substring(60, 3));
                Console.WriteLine(": " + line.Substring(63, 1));
                Console.WriteLine(": " + line.Substring(64, 1));
                Console.WriteLine(": " + line.Substring(65, 4));
                Console.WriteLine(": " + line.Substring(69, 1));
                Console.WriteLine(": " + line.Substring(70, 5));
                Console.WriteLine(": " + line.Substring(75, 1));
                Console.WriteLine(": " + line.Substring(76, 1));
                Console.WriteLine(": " + line.Substring(77, 1));
                Console.WriteLine(": " + line.Substring(78, 6));
                Console.WriteLine(": " + line.Substring(84, 1));
                Console.WriteLine(": " + line.Substring(85, 1));
                Console.WriteLine(": " + line.Substring(86, 1));
                Console.WriteLine(": " + line.Substring(87, 5));
                Console.WriteLine(": " + line.Substring(92, 1));
                Console.WriteLine(": " + line.Substring(93, 5));
                Console.WriteLine(": " + line.Substring(98, 1));
                Console.WriteLine(": " + line.Substring(99, 5));
                Console.WriteLine(": " + line.Substring(104, 1));
                Console.WriteLine("Additional Data: " + line.Substring(105));

                Console.ReadLine();
                Console.WriteLine();
            }

            Console.WriteLine();
            Console.WriteLine("Done");
            Console.ReadLine();

            reader.Close();
            newStream.Close();
            fileStream.Close();
            //reader.Close();
            response.Close();
        }
    }
}
