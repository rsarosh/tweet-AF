using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TweeterFunciton
{
    class CognitiveSvc
    {
        string _key;
        string _value;

        public CognitiveSvc (string key, string value)
        {
            _key = key;
            _value = value;
        }
        public async Task<float> GetSentiments(string text)
        {
            using (var client = new HttpClient())
            {
 
                client.DefaultRequestHeaders.Add(
                    _key,
                    _value);

                // This is the data we're posting.
                // Anonymous types are fun!
                var request =
                    new
                    {
                        documents = new[] {
                                            new {
                                                language = "en",
                                                id = "001",
                                                text
                                                }
                                            }
                    };

                // Create form data, setting the content type
                HttpContent content = new StringContent(
                                                JsonConvert.SerializeObject(request),
                                                Encoding.UTF8,
                                                "application/json");

                // Send it to Sentiment endpoint
                // TODO: Move to Config
                var sentimentEndpoint = "https://westus2.api.cognitive.microsoft.com/text/analytics/v2.0/sentiment";
                var response = await client.PostAsync(sentimentEndpoint, content);

                // Dynamic types are fun!
                // Deserialize json response to dynamic object
                dynamic dynamicResponse =
                    JsonConvert.DeserializeObject(
                        await response.Content.ReadAsStringAsync()
                    );

                // Get the score as a number
                float score = dynamicResponse.documents[0].score;

                // Arbitrary happiness threshold
                return score ;
            }
        }
    }
}
