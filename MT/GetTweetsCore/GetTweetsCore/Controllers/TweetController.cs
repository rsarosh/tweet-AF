using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using System.Net.Http;


namespace GetTweetsCore.Controllers
{
    [Route("api/[controller]")]
    [EnableCors(origins: "http://localhost:4200/", headers: "*", methods: "*")]
    public class TweetController : Controller
    {
        CloudQueue tweetQueue;
      
        public TweetController()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=scmrstorage;AccountKey=FtW5Qlz/5rWiqX0MPlGO0X2anGs5t7ea/H/ZkdcIEHlTA9isEinpscnuuhw8GwKR+7+Eo2IDRG1jwdMoDsRTqg==;EndpointSuffix=core.windows.net");
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            tweetQueue = queueClient.GetQueueReference("tweetqueue");
            
        }

        // GET api/values
        [HttpGet]
        public IEnumerable<Tweet> Get()
        {
            List<Tweet> _tweets = new List<Tweet>();

            do
            {

                Tweet t = GetTweets().Result;
                if (t != null)
                {
                    t.Post = System.Net.WebUtility.UrlDecode(t.Post);
                    _tweets.Add(t);
                }
                else
                    return _tweets;

            } while (true);
        }

        private async Task<Tweet> GetTweets()
        {
            CloudQueueMessage retrievedMessage = await tweetQueue.GetMessageAsync();
            if (retrievedMessage != null)
            {
               await tweetQueue.DeleteMessageAsync(retrievedMessage);
               var t =  await Task<Tweet>.Factory.StartNew(() => {
                    Tweet result =  JsonConvert.DeserializeObject<Tweet>(retrievedMessage.AsString);
                    return result;
                    });
                return t;
            }

            //else
            //{
            //    return null; 

            //    /*
            //    //Send the fake data;
            //    Random r = new Random();
            //    List<Tweet> t = new List<Tweet>();
            //    t.Add(new Tweet { Author = "AAAA", Post = "asdfasdf", Sentiments = r.Next(100) });
            //    t.Add(new Tweet { Author = "BBB", Post = "asdfasdf", Sentiments = r.Next(100) });
            //    t.Add(new Tweet { Author = "CCC", Post = "asdfasdf", Sentiments = r.Next(100) });
            //    return t.ToArray();
            //    */
            //}
            return null;
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }


    public class Tweet
    {
        public string Author;
        public string Post;
        public float Sentiments;
    }
}
