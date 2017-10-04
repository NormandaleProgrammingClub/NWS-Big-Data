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

                // Search Stations DB for matching WBAN_ID, see if that record is the state we want
                int exists = 0;
                try
                {
                    //Console.WriteLine("    Searching DB...");
                    //string query = "SELECT STATE_PROVINCE FROM CPC.WBAN WHERE WBAN_ID=\"" + line + "\" LIMIT 1";
                    //string query = "SELECT STATE_PROVINCE FROM CPC.WBAN WHERE WBAN_ID=\"" + line + "\"";
                    string query = "";
                    string USAF2 = USAF;
                    string WBAN2 = WBAN;

                    if (USAF == "999999")
                    {
                        // Query is to use WBAN ID
                        query = "SELECT CTRY,ST FROM CPC.USAF WHERE WBAN_ID=\"" + WBAN + "\"";
                        USAF2 = "      ";
                    }
                    else
                    {
                        // Query is to use USAF ID
                        query = "SELECT CTRY,ST FROM CPC.USAF WHERE USAF_ID=\"" + USAF + "\"";
                        if(WBAN == "99999") WBAN2 = "     ";
                    }

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    MySqlDataReader SQLreader = cmd.ExecuteReader();
                    
                    while(SQLreader.Read())
                    {
                        /*
                        if (SQLreader.GetString(0).Trim() == "US")
                        {
                            Console.Write("*");
                            if (SQLreader.GetString(1) == "FL") Console.Write("*");
                            Console.Write(" ");
                        }

                        // USAF: 000000 | WBAN: 00000 | Country: US / United States | State: MN
                        Console.Write("USAF: " + USAF + " | WBAN: " + WBAN);
                        Console.Write(" | Country: " + SQLreader.GetString(0).Trim() + " / " + CountryCode(SQLreader.GetString(0)));
                        Console.WriteLine(" | State: " + SQLreader.GetString(1));
                        
                        writer.WriteLine("USAF: " + USAF2 + " | WBAN: " + WBAN2 + " | Country: " + SQLreader.GetString(0).Trim() + " / " + CountryCode(SQLreader.GetString(0)) + " | State: " + SQLreader.GetString(1));
                        */

                        // SQLreader.GetString(SQLreader.GetOrdinal("dbid")); // Get by column name
                        
                        if (SQLreader.GetString(0).Trim() == "US")
                        {
                            Console.WriteLine("USAF: " + USAF2 + " | WBAN: " + WBAN2 + " | State: " + SQLreader.GetString(1));
                            writer.WriteLine("USAF: " + USAF2 + " | WBAN: " + WBAN2 + " | State: " + SQLreader.GetString(1) + " | Src: " + File);
                        }

                    }

                    SQLreader.Close();
                    //Console.WriteLine();
                    //Console.WriteLine("    Data pulled");
                }
                catch (MySqlException ex)
                {
                    Console.WriteLine("    Error pulling data:");
                    Console.WriteLine(ex.Message);
                }

                if(exists==1) Console.ReadLine();
                //Console.WriteLine();
            }

            Console.WriteLine("Directory List Complete, status {0}", response.StatusDescription);

            Console.ReadLine();

            writer.Close();
            reader.Close();
            response.Close();
        }

        static string CountryCode(string Code)
        {
            Code = Code.Trim();

            switch (Code)
            {
                case "AF": return "Afghanistan";
                case "AX": return "Aland Islands";
                case "AL": return "Albania";
                case "DZ": return "Algeria";
                case "AS": return "American Samoa";
                case "AD": return "Andorra";
                case "AO": return "Angola";
                case "AI": return "Anguilla";
                case "AQ": return "Antarctica";
                case "AG": return "Antigua and Barbuda";
                case "AR": return "Argentina";
                case "AM": return "Armenia";
                case "AW": return "Aruba";
                case "AU": return "Australia";
                case "AT": return "Austria";
                case "AZ": return "Azerbaijan";
                case "BS": return "Bahamas";
                case "BH": return "Bahrain";
                case "BD": return "Bangladesh";
                case "BB": return "Barbados";
                case "BY": return "Belarus";
                case "BE": return "Belgium";
                case "BZ": return "Belize";
                case "BJ": return "Benin";
                case "BM": return "Bermuda";
                case "BT": return "Bhutan";
                case "BO": return "Bolivia";
                case "BA": return "Bosnia and Herzegovina";
                case "BW": return "Botswana";
                case "BV": return "Bouvet Island";
                case "BR": return "Brazil";
                case "VG": return "British Virgin Islands";
                case "IO": return "British Indian Ocean Territory";
                case "BN": return "Brunei Darussalam";
                case "BG": return "Bulgaria";
                case "BF": return "Burkina Faso";
                case "BI": return "Burundi";
                case "KH": return "Cambodia";
                case "CM": return "Cameroon";
                case "CA": return "Canada";
                case "CV": return "Cape Verde";
                case "KY": return "Cayman Islands";
                case "CF": return "Central African Republic";
                case "TD": return "Chad";
                case "CL": return "Chile";
                case "CN": return "China";
                case "HK": return "Hong Kong";
                case "MO": return "Macao";
                case "CX": return "Christmas Islands";
                case "CC": return "Cocos (Keeling) Islands";
                case "CO": return "Colombia";
                case "KM": return "Comoros";
                case "CG": return "Congo (Brazzaville)";
                case "CD": return "Congo (Kinshasa)";
                case "CK": return "Cook Islands";
                case "CR": return "Costa Rica";
                case "CI": return "Cote d'Ivoire";
                case "HR": return "Croatia";
                case "CU": return "Cuba";
                case "CY": return "Cyprus";
                case "CZ": return "Czzech Republic";
                case "DK": return "Denmark";
                case "DJ": return "Djibouti";
                case "DM": return "Dominica";
                case "DO": return "Dominican Republic";
                case "EC": return "Ecuador";
                case "EG": return "Egypt";
                case "SV": return "El Salvador";
                case "GQ": return "Equatorial Guinea";
                case "ER": return "Eritrea";
                case "EE": return "Estonia";
                case "ET": return "Ethiopia";
                case "FK": return "Falkland Islands";
                case "FO": return "Faroe Islands";
                case "FJ": return "Fiji";
                case "FI": return "Finland";
                case "FR": return "France";
                case "GF": return "French Guiana";
                case "PF": return "French Polynesia";
                case "TF": return "French Southern Territories";
                case "GA": return "Gabon";
                case "GM": return "Gambia";
                case "GE": return "Georgia";
                case "DE": return "Germany";
                case "GH": return "Ghana";
                case "GI": return "Gibraltar";
                case "GR": return "Greece";
                case "GL": return "Greenland";
                case "GD": return "Grenada";
                case "GP": return "Guadeloupe";
                case "GU": return "Guam";
                case "GT": return "Guatemala";
                case "GG": return "Guernsey";
                case "GN": return "Guinea";
                case "GW": return "Guinea-Bissau";
                case "GY": return "Guyana";
                case "HT": return "Haiti";
                case "HM": return "Heard and Mcdonald Islands";
                case "VA": return "Holy See (Vatican City State)";
                case "HN": return "Honduras";
                case "HU": return "Hungary";
                case "IS": return "Iceland";
                case "IN": return "India";
                case "ID": return "Indonesia";
                case "IR": return "Iran";
                case "IQ": return "Iraq";
                case "IE": return "Ireland";
                case "IM": return "Isle of Man";
                case "IL": return "Israel";
                case "IT": return "Italy";
                case "JM": return "Jamaica";
                case "JP": return "Japan";
                case "JE": return "Jersey";
                case "JO": return "Jordan";
                case "KZ": return "Kazakhstan";
                case "KE": return "Kenya";
                case "KI": return "Kiribati";
                case "KP": return "North Korea";
                case "KR": return "South Korea";
                case "KW": return "Kuwait";
                case "KG": return "Kyrgyzstan";
                case "LA": return "Lao";
                case "LV": return "Latvia";
                case "LB": return "Lebanon";
                case "LS": return "Lesotho";
                case "LR": return "Liberia";
                case "LY": return "Libya";
                case "LI": return "Liechtenstein";
                case "LT": return "Lithuania";
                case "LU": return "Luxembourg";
                case "MK": return "Macedonia";
                case "MG": return "Madagascar";
                case "MW": return "Malawi";
                case "MY": return "Malaysia";
                case "MV": return "Maldives";
                case "ML": return "Mali";
                case "MT": return "Malta";
                case "MH": return "Marshall Islands";
                case "MQ": return "Martinique";
                case "MR": return "Mauritania";
                case "MU": return "Mauritius";
                case "YT": return "Mayotte";
                case "MX": return "Mexico";
                case "FM": return "Micronesia";
                case "MD": return "Moldova";
                case "MC": return "Monaco";
                case "MN": return "Mongolia";
                case "ME": return "Montenegro";
                case "MS": return "Montserrat";
                case "MA": return "Morocco";
                case "MZ": return "Mozambique";
                case "MM": return "Myanmar";
                case "NA": return "Namibia";
                case "NR": return "Nauru";
                case "NP": return "Nepal";
                case "NL": return "Netherlands";
                case "AN": return "Netherlands Antilles";
                case "NC": return "New Caledonia";
                case "NZ": return "New Zealand";
                case "NI": return "Nicaragua";
                case "NE": return "Niger";
                case "NG": return "Nigeria";
                case "NU": return "Niue";
                case "NF": return "Norfolk Islands";
                case "MP": return "Northern Mariana Islands";
                case "NO": return "Norway";
                case "OM": return "Oman";
                case "PK": return "Pakistan";
                case "PW": return "Palau";
                case "PS": return "Palestinian Territory";
                case "PA": return "Panama";
                case "PG": return "Papua New Guinea";
                case "PY": return "Paraguay";
                case "PE": return "Peru";
                case "PH": return "Philippines";
                case "PN": return "Pitcairn";
                case "PL": return "Poland";
                case "PT": return "Portugal";
                case "PR": return "Puerto Rico";
                case "QA": return "Qatar";
                case "RE": return "Reunion";
                case "RO": return "Romania";
                case "RU": return "Russian Federation";
                case "RW": return "Rwanda";
                case "BL": return "Saint-Barthelemy";
                case "SH": return "Saint Helena";
                case "KN": return "Saint Kitts and Nevis";
                case "LC": return "Saint Lucia";
                case "MF": return "Saint-Martin";
                case "PM": return "Saint Pierre and Miquelon";
                case "VC": return "Saint Vincent and Grenadines";
                case "WS": return "Samoa";
                case "SM": return "San Marino";
                case "ST": return "Sao Tome and Principe";
                case "SA": return "Saudi Arabia";
                case "SN": return "Senegal";
                case "RS": return "Serbia";
                case "SC": return "Seychelles";
                case "SL": return "Sierra Leone";
                case "SG": return "Singapore";
                case "SK": return "Slovakia";
                case "SI": return "Slovenia";
                case "SB": return "Solomon Islands";
                case "SO": return "Somalia";
                case "ZA": return "South Africa";
                case "GS": return "South Georgia";
                case "SS": return "South Sudan";
                case "ES": return "Spain";
                case "LK": return "Sri Lanka";
                case "SD": return "Sudan";
                case "SR": return "Suriname";
                case "SJ": return "Svalbard";
                case "SZ": return "Swaziland";
                case "SE": return "Sweden";
                case "CH": return "Switzerland";
                case "SY": return "Syria";
                case "TW": return "Taiwan";
                case "TJ": return "Tajikistan";
                case "TZ": return "Tanzania";
                case "TH": return "Thailand";
                case "TL": return "Timor-Leste";
                case "TG": return "Togo";
                case "TK": return "Tokelau";
                case "TO": return "Tonga";
                case "TT": return "Trinidad and Tobago";
                case "TN": return "Tunisia";
                case "TR": return "Turkey";
                case "TM": return "Turkmenistan";
                case "TC": return "Turks and Caicos Islands";
                case "TV": return "Tuvalu";
                case "UG": return "Uganda";
                case "UA": return "Ukraine";
                case "AE": return "United Arab Emirates";
                case "GB": return "United Kingdom";
                case "US": return "United States of America";
                case "UM": return "US Minor Outlying Islands";
                case "UY": return "Uruguay";
                case "UZ": return "Uzbekistan";
                case "VU": return "Vanuatu";
                case "VE": return "Venezuela";
                case "VN": return "Viet Nam";
                case "VI": return "US Virgin Islands";
                case "WF": return "Wallis and Futuna Islands";
                case "EH": return "Western Sahara";
                case "YE": return "Yemen";
                case "ZM": return "Zambia";
                case "ZW": return "Zimbabew";
                default: return "Unknown Country Code";
            }
        }
    }
}
