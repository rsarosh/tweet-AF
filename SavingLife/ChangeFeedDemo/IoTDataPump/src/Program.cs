using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Sql;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents;
using Newtonsoft.Json;
using System.Threading;
using System.Configuration;
using System.Diagnostics;
using AzureCosmosDBLib;

namespace IotPumpData
{
    class Program
    {
        static string _iotid;
        static double _temp;
        static long _timestamp;
        static long DOC_PER_THREAD = 1000;
        static long NUMBER_OF_THREAD = 1;
     
        static void Main(string[] args)
        {
            Stopwatch st = new Stopwatch();
            st.Start();

            ConnectionPolicy connectionPolicy = new ConnectionPolicy();
            connectionPolicy.UserAgentSuffix = " samples-net/3";
            connectionPolicy.ConnectionMode = ConnectionMode.Direct;
            connectionPolicy.ConnectionProtocol = Protocol.Tcp;

            // Set the read region selection preference order
            connectionPolicy.PreferredLocations.Add(LocationNames.WestUS); // first preference
            connectionPolicy.PreferredLocations.Add(LocationNames.NorthEurope); // second preference
            connectionPolicy.PreferredLocations.Add(LocationNames.SoutheastAsia); // third preference


            Client<IoTData>.Initialize(ConfigurationManager.AppSettings["database"],
                                                          ConfigurationManager.AppSettings["collection"],
                                                          ConfigurationManager.AppSettings["endpoint"],
                                                          ConfigurationManager.AppSettings["authKey"], connectionPolicy);
            //ReadValue();

            //QueryValue();
            while (true)
            {
                PumpData(null);
                st.Stop();
                Console.WriteLine(" Pumped {0} data in {1} sec", NUMBER_OF_THREAD * DOC_PER_THREAD, st.Elapsed.TotalSeconds);
                if (Console.ReadKey().Key ==  ConsoleKey.Escape)
                {
                    break;
                }
            }
            return;
        }

        private static async void ReadValue()
        {
             var response =  await Client<IoTData>.GetItemAsync("733b6279-52ce-4ec6-88dc-6cc975bf8f1d", "733b6279-52ce-4ec6-88dc-6cc975bf8f1d");
             Console.WriteLine(response);
        }


        private static void QueryValue()
        {
            var response = Client<IoTData>.QueryItem("733b6279-52ce-4ec6-88dc-6cc975bf8f1d");
            Console.WriteLine(response);
        }


        private static void PumpData( Object stateInfo )
        {
            Random rnd = new Random();
            string[] iots = new string[] { "AA", "BB", "CC" };
            int ctr = 0;
            int min = 0;
            int max = 5;
            long docCtr = 0;
            do
            {
                
           
                if (docCtr++ > DOC_PER_THREAD) docCtr = 0;
                InsertData(rnd.Next(min, max), "AA");// iots[ctr++]);

                Console.Write(".");

                ConsoleKeyInfo k = Console.ReadKey();

                if (k.Key == ConsoleKey.UpArrow) //Temp up
                {
                    max++;
                    if (max > 100)
                        max = 100;
                }

                if (k.Key == ConsoleKey.DownArrow) //Temp down
                {
                    max--;
                    if (max < 5)
                        max = 5;
                }

                if (k.Key == ConsoleKey.Escape) //break
                {
                    break;
                }

            } while (true);
        }

        private async static void InsertData(double temp, String iotID)
        {
            _iotid = iotID;
            _temp =  temp;
            _timestamp = DateTime.UtcNow.Ticks;
            Random r = new Random(DateTime.Now.Millisecond);

            IoTData data = new IoTData
            {
                id = Guid.NewGuid().ToString(),
                iotid = _iotid,
                temp = _temp,

                lat = 47.639002,
                //r.Next(100),
                longitude = -122.128196,//r.Next(100),
                carid ="AAA",
                timestamp = _timestamp
            };

            Document doc =  await Client<IoTData>.CreateItemAsync(data);
            IoTData i =  (IoTData) doc;

        }
    }
    public class IoTData 
    {
        [JsonProperty("id")]
        public string id;

        [JsonProperty("iotid")]
        public string iotid;

        [JsonProperty("lat")]
        public double lat;

        [JsonProperty("longitude")]
        public double longitude;

        [JsonProperty("carid")]
        public string carid;

        [JsonProperty("temp")]
        public double temp;

        [JsonProperty("timestamp")]
        public long timestamp;

        public static explicit operator IoTData (Document doc)
        {
            IoTData _iotData = new IoTData();
            _iotData.id = doc.Id;
            _iotData.iotid = doc.GetPropertyValue<string>("iotid");
            _iotData.temp = doc.GetPropertyValue<double>("temp");
            _iotData.timestamp = doc.GetPropertyValue<long>("timestamp");
            return _iotData;
        }
    }
}
