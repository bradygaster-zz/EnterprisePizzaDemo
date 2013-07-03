using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace EnterprisePizza.Infrastructure
{
    public class InitializationRequest
    {
        public string Issuer { get; set; }
        public string IssuerKey { get; set; }
        public string Namespace { get; set; }
    }

    public class ServiceBusTopicHelper
    {
        string _namespace;
        string _issuer;
        string _issuerKey;
        NamespaceManager _namespaceManager;
        MessagingFactory _messagingFactory;
        TokenProvider _tokenProvider;
        Uri _serviceUri;
        List<Tuple<string, SubscriptionClient>> _subscribers;
        private ReceiveMode _receiveMode;

        private ServiceBusTopicHelper()
        {
            _subscribers = new List<Tuple<string, SubscriptionClient>>();
        }

        private void SetupServiceBusEnvironment()
        {
            if (_namespaceManager == null)
            {
                _tokenProvider = TokenProvider.CreateSharedSecretTokenProvider(_issuer, _issuerKey);
                _serviceUri = ServiceBusEnvironment.CreateServiceUri("sb", _namespace, string.Empty);
                _messagingFactory = MessagingFactory.Create(_serviceUri, _tokenProvider);
                _namespaceManager = new NamespaceManager(_serviceUri, _tokenProvider);
            }
        }

        public static ServiceBusTopicHelper Setup(InitializationRequest request)
        {
            var ret = new ServiceBusTopicHelper
            {
                _namespace = request.Namespace,
                _issuer = request.Issuer,
                _issuerKey = request.IssuerKey
            };

            return ret;
        }

        public ServiceBusTopicHelper Subscribe<T>(Action<T> receiveHandler,
            string filterSqlStatement = null,
            string subscriptionName = null,
            ReceiveMode receiveMode = ReceiveMode.ReceiveAndDelete)
        {

            // if they asked for a subscription with a filter and no name, blow up
            if (!string.IsNullOrEmpty(filterSqlStatement)
                && string.IsNullOrEmpty(subscriptionName))
                throw new ArgumentException("If filterSqlStatement is provided, subscriptionName must also be provided.");

            _receiveMode = receiveMode;
            SetupServiceBusEnvironment();
            var topicName = string.Format("Topic_{0}", typeof(T).Name);

            subscriptionName = string.IsNullOrEmpty(subscriptionName)
                ? string.Format("Subscription_{0}", typeof(T).Name)
                : subscriptionName;

            if (!_namespaceManager.TopicExists(topicName))
                _namespaceManager.CreateTopic(topicName);

            var topic = _namespaceManager.GetTopic(topicName);

            SubscriptionDescription subscription;

            // always create a new subscription just in case the calling code's changed expectations
            if (_namespaceManager.SubscriptionExists(topic.Path, subscriptionName))
                _namespaceManager.DeleteSubscription(topic.Path, subscriptionName);

            if (string.IsNullOrEmpty(filterSqlStatement))
            {
                subscription = _namespaceManager.CreateSubscription(topic.Path, subscriptionName);
            }
            else
            {
                var filter = new SqlFilter(filterSqlStatement);
                subscription = _namespaceManager.CreateSubscription(topic.Path, subscriptionName, filter);
            }

            var subscriptionClient = _messagingFactory.CreateSubscriptionClient(topicName, subscriptionName, receiveMode);

            _subscribers.Add(new Tuple<string, SubscriptionClient>(topicName, subscriptionClient));

            Begin<T>(receiveHandler, subscriptionClient);

            return this;
        }

        private void Begin<T>(Action<T> receiveHandler, SubscriptionClient subscriptionClient)
        {
            Debug.WriteLine("Calling BeginReceive");
            subscriptionClient.BeginReceive(
                TimeSpan.FromMinutes(5),
                (cb) => ProcessBrokeredMessage(receiveHandler, subscriptionClient, cb),
                null);
        }

        private void ProcessBrokeredMessage<T>(Action<T> receiveHandler, SubscriptionClient subscriptionClient, IAsyncResult cb)
        {
            try
            {
                var brokeredMessage = subscriptionClient.EndReceive(cb);
                if (brokeredMessage != null)
                {
                    var messageData = brokeredMessage.GetBody<T>();
                    try
                    {
                        receiveHandler(messageData);

                        if (_receiveMode == ReceiveMode.PeekLock)
                        {
                            brokeredMessage.BeginComplete((result) =>
                            {
                                var m = result.AsyncState as BrokeredMessage;
                                if (m != null) m.EndComplete(result);
                            }, brokeredMessage);
                        }
                    }
                    catch (Exception)
                    {
                        if (_receiveMode == ReceiveMode.PeekLock)
                        {
                            brokeredMessage.BeginAbandon((result) =>
                            {
                                var m = result.AsyncState as BrokeredMessage;
                                if (m != null)
                                    m.EndAbandon(result);
                            }, brokeredMessage);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("BeginReceive threw exception");
                if (OnError != null)
                    OnError(ex); // report the error if the user wants to know the error
            }
            finally
            {
                Begin<T>(receiveHandler, subscriptionClient);
            }
        }

        public ServiceBusTopicHelper Publish<T>(T message, Action<BrokeredMessage> callback = null)
        {
            SetupServiceBusEnvironment();
            var topicName = string.Format("Topic_{0}", typeof(T).Name);
            var topicClient = _messagingFactory.CreateTopicClient(topicName);

            try
            {
                var m = new BrokeredMessage(message);
                if (callback != null)
                    callback(m);
                topicClient.Send(m);
            }
            catch (Exception x)
            {
                throw x;
            }
            finally
            {
                topicClient.Close();
            }

            return this;
        }

        public ServiceBusTopicHelper Close()
        {
            _subscribers.ForEach((s) => s.Item2.Close());
            return this;
        }

        public ServiceBusTopicHelper ClearTopics()
        {
            _subscribers.ForEach((s) => _namespaceManager.DeleteTopic(s.Item1));
            return this;
        }

        public delegate void ServiceBusExceptionHandler(Exception exception);

        public event ServiceBusExceptionHandler OnError;
    }
}