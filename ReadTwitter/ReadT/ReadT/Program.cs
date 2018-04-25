using LinqToTwitter;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using AzureCosmosDBLib;
using Microsoft.Azure.Documents.Client;
using System.Configuration;
using Microsoft.Azure.Documents;


/*
 * 
 * This application reads the tweets from the twitter and stores them to CosmosDB collection. 
 *  
 */
 
namespace ReadT
{
    public class Tweet
    {
        public string Author;
        public string Post;
        public float Sentiments;
    }

    class Program
    {

        static string twConsumerKey;
        static string twConsumerSecrate;
        static string cognitiveKey;
        static string cognitiveSecrate;

        static Twitter _twitter;
        static string _TweetHashTag = "Microsoft. Azure, CosmosDB, Google, spark";
        static CognitiveSvc _cognitiveSvc;
        static void Main(string[] args)
        {
            ConnectionPolicy connectionPolicy = new ConnectionPolicy();
            connectionPolicy.UserAgentSuffix = " samples-net/3";
            connectionPolicy.ConnectionMode = ConnectionMode.Direct;
            connectionPolicy.ConnectionProtocol = Protocol.Tcp;
            twConsumerKey = ConfigurationManager.AppSettings["TwitterConsumerKey"];
            twConsumerSecrate = ConfigurationManager.AppSettings["TwitterConsumerSecret"];
            cognitiveKey = ConfigurationManager.AppSettings["CongnitiveSubscriptionKey"];
            cognitiveSecrate = ConfigurationManager.AppSettings["CongnitiveConsumerSecret"];

            _twitter = new Twitter(twConsumerKey, twConsumerSecrate);
            _cognitiveSvc = new CognitiveSvc(cognitiveKey, cognitiveSecrate);

            Client<Tweet>.Initialize(ConfigurationManager.AppSettings["Database"],
                                                         ConfigurationManager.AppSettings["Collection"],
                                                         ConfigurationManager.AppSettings["Endpoint"],
                                                         ConfigurationManager.AppSettings["AuthKey"], connectionPolicy);
            List<Task> tasks = new List<Task>();
            while (true)
            {
                tasks.Add(Task.Factory.StartNew(() => SaveTweets()));
                if (Console.ReadKey().Key == ConsoleKey.Escape)
                {
                    break;
                }
            }
            return;
        }


        private async static void SaveTweets()
        {
            List<Tweet> _tweets = new List<Tweet>();
            char delimiter = ',';
            do
            {

                foreach (string hashtag in _TweetHashTag.Split(delimiter))
                {
                    _tweets = _twitter.SearchTweets(hashtag).Result as List<Tweet>;
                    
                    foreach (Tweet _t in _tweets)
                    {
                        Tweet t = new Tweet();
                        t.Author = _t.Author;
                        t.Post = _t.Post;
                        t.Sentiments = _cognitiveSvc.GetSentiments(_t.Post).Result; ;
                        Document doc = await Client<Tweet>.CreateItemAsync(t);
                        Console.Write("*");
                    };
                    Console.Write(".");
                }
                Thread.Sleep(30000); // Sleep 5 Sec
            } while (true);

        }
       
    }


}
