using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnterprisePizza.Infrastructure
{
    public class StorageQueueHelper
    {
        CloudQueue _queue;

        public static StorageQueueHelper OpenQueue(string queueName)
        {
            var account = CloudStorageAccount.Parse(
                ConfigurationManager.AppSettings["STORAGE_ACCOUNT"]
                ); 
            
            var client = account.CreateCloudQueueClient();
            var queue = client.GetQueueReference(queueName);
            queue.CreateIfNotExists();

            return new StorageQueueHelper
            {
                _queue = queue
            };
        }

        public StorageQueueHelper Enqueue(string message)
        {
            _queue.AddMessage(new CloudQueueMessage(message));
            return this;
        }

        public StorageQueueHelper Dequeue(Action<CloudQueueMessage> handler)
        {
            var msg = _queue.GetMessage();

            while (msg != null)
            {
                if (msg != null && msg.DequeueCount < 3)
                {
                    handler(msg);
                    _queue.DeleteMessage(msg);
                }

                msg = _queue.GetMessage();
            }

            return this;
        }
    }
}
