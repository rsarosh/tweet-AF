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
        [FunctionName("TweetAF")]
        public static void Run([CosmosDBTrigger(
            databaseName: "rafat-tweets",
            collectionName: "tweets",
            ConnectionStringSetting = "DBConnection",
            LeaseCollectionName = "leases")]IReadOnlyList<Document> items, TraceWriter log)
        {

            //Queue Conneciton
            Microsoft.WindowsAzure.Storage.Queue.CloudQueueClient queueClient;
            Microsoft.WindowsAzure.Storage.Queue.CloudQueue queue;
            Microsoft.WindowsAzure.Storage.CloudStorageAccount storageAccount = Microsoft.WindowsAzure.Storage.CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=scmrstorage;AccountKey=FtW5Qlz/5rWiqX0MPlGO0X2anGs5t7ea/H/ZkdcIEHlTA9isEinpscnuuhw8GwKR+7+Eo2IDRG1jwdMoDsRTqg==;EndpointSuffix=core.windows.net");
            queueClient = storageAccount.CreateCloudQueueClient();

            queue = queueClient.GetQueueReference("tweetqueue");

            foreach (var doc in items)
            {
                string m = String.Format("{{ \"Author\": \"{0}\", \"Post\":\"{1}\", \"Sentiments\": {2} }}",
                                doc.GetPropertyValue<string>("Author"),
                                doc.GetPropertyValue<string>("Post"),
                                doc.GetPropertyValue<string>("Sentiments"));
                Microsoft.WindowsAzure.Storage.Queue.CloudQueueMessage message = new Microsoft.WindowsAzure.Storage.Queue.CloudQueueMessage
                (m);
                queue.AddMessage(message);
                log.Verbose("Message Added");

            }
        }
    }
}
