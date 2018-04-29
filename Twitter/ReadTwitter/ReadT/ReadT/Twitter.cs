using LinqToTwitter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadT
{
    class Twitter
    {
        ulong maxID;
        TwitterContext twitterCtx;
        ApplicationOnlyAuthorizer auth;
      
        public Twitter (string consumerKey, string consumerSecret) {
            auth = SetAuth(consumerKey, consumerSecret).Result;
            twitterCtx = new TwitterContext(auth);
        }

        private async Task<ApplicationOnlyAuthorizer> SetAuth(string consumerKey, string consumerSecret)
        {
            ApplicationOnlyAuthorizer auth = new ApplicationOnlyAuthorizer
            {
                CredentialStore = new InMemoryCredentialStore()
                {
                    ConsumerKey = consumerKey,
                    ConsumerSecret = consumerSecret
                }
            };

            await auth.AuthorizeAsync();
            return auth;
        }

        public async Task <IEnumerable<Tweet>> SearchTweets(string hashTag)
        {
            List<Tweet> _tweets = new List<Tweet>();
          
            var searchResponse =
              await
              (from search in twitterCtx.Search
               where search.Type == SearchType.Search &&
                     search.Query == "#" + hashTag &&
                     search.Count == 1
                     && search.SinceID == maxID
               select search)
                .SingleOrDefaultAsync();

            if (searchResponse.Statuses.Count > 0)
            {
                maxID = searchResponse.Statuses.First().StatusID + 1;
            }
            else
            {
                Console.Write("*");
            }

            if (searchResponse != null && searchResponse.Statuses != null)
            {
                searchResponse.Statuses.ForEach(tweet =>
                    {
                        Tweet t = new Tweet();
                        t.Author = tweet.User.ScreenNameResponse;
                        t.Post = System.Net.WebUtility.UrlEncode(tweet.Text.Replace("\"", "\\\""));
                        t.Sentiments = 0;
                        _tweets.Add(t);
                        Console.WriteLine(
                            "[ {0} ] User: {1} => Tweet: {2}",
                            0,
                            t.Author + "\t",
                            t.Post);
                    });
                return  _tweets;
            }
            return null;

        }
    }
}
