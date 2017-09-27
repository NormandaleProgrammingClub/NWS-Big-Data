using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace FTPDirectory
{
    class Program
    {
        static void Main(string[] args)
        {
            // Get the object used to communicate with the server.  
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://ftp.ncdc.noaa.gov/pub/data/noaa/2017/");
            request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;

            // This example assumes the FTP site uses anonymous logon.  
            //request.Credentials = new NetworkCredential("anonymous", "janeDoe@contoso.com");

            FtpWebResponse response = (FtpWebResponse)request.GetResponse();

            Stream responseStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(responseStream);
            //Console.WriteLine(reader.ReadToEnd());

            while (reader.Peek() >= 0)
            {
                String line = reader.ReadLine().Substring(55, 20);
                Console.WriteLine(line);
            }

            Console.WriteLine("Directory List Complete, status {0}", response.StatusDescription);

            Console.ReadLine();

            reader.Close();
            response.Close();
        }
    }
}
