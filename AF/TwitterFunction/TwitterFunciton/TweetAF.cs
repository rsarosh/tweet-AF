using Microsoft.Azure.Documents.Client;
using Microsoft.WindowsAzure.Storage;

using System;
using System.Collections.Generic;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace TweeterFunciton
{
    public static class TweetAF
    {
        static CognitiveSvc _cognitiveSvc;

        [FunctionName("TweetAF")]
        public static void Run([CosmosDBTrigger(
            databaseName: "rafat-tweets",
            collectionName: "tweets",
            ConnectionStringSetting = "DBConnection",
            LeaseCollectionName = "leases")]IReadOnlyList<Document> items, TraceWriter log)
        {

            

            if (_cognitiveSvc == null)
            {
                _cognitiveSvc = new CognitiveSvc("Ocp-Apim-Subscription-Key", "d42e7ac9cbd04c81921810");
            }
            //Queue Conneciton
            Microsoft.WindowsAzure.Storage.Queue.CloudQueueClient queueClient;
            Microsoft.WindowsAzure.Storage.Queue.CloudQueue queue;
            Microsoft.WindowsAzure.Storage.CloudStorageAccount storageAccount = Microsoft.WindowsAzure.Storage.CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=scmrstorage;AccountKey=FtW5Qlz/5rWiqX0MPlGO0X2anGs5t7ea/H/ZkdcIEHlTA9isEinpscnuuhw8GwKR+7+Eo2IDRG1jwdMoDsRTqg==;EndpointSuffix=core.windows.net");
            queueClient = storageAccount.CreateCloudQueueClient();

            queue = queueClient.GetQueueReference("tweetqueue");

            foreach (var doc in items)
            {
                string _post = doc.GetPropertyValue<string>("Post");
                float _sentiments = _cognitiveSvc.GetSentiments(_post).Result; 
                string m = String.Format("{{ \"Author\": \"{0}\", \"Post\":\"{1}\", \"Sentiments\": {2} }}",
                                doc.GetPropertyValue<string>("Author"),
                                _post,
                                _sentiments);
                Microsoft.WindowsAzure.Storage.Queue.CloudQueueMessage message = new Microsoft.WindowsAzure.Storage.Queue.CloudQueueMessage
                (m);
                queue.AddMessage(message);
                log.Verbose("Message Added");

            }
        }
    }
}
