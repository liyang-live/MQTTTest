using System;
using System.Net.Mqtt;
using System.Reactive.Linq;
using System.Text;

namespace MicroSoftMqttClientTest
{
    class Program
    {
        static void Main(string[] args)
        {

            ClientTest clientTest = new ClientTest();

            clientTest.Start();



            Console.WriteLine("Hello World!");

            Console.ReadLine();
        }
    }
    public class ClientTest
    {

        public async void Start()
        {

            var configuration = new MqttConfiguration
            {
                BufferSize = 128 * 1024,
                Port = 55555,
                KeepAliveSecs = 10,
                WaitTimeoutSecs = 2,
                MaximumQualityOfService = MqttQualityOfService.AtMostOnce,
                AllowWildcardsInTopicFilters = true
            };
            var client = await MqttClient.CreateAsync("127.0.0.1", configuration);
            var sessionState = await client.ConnectAsync(new MqttClientCredentials(clientId: int.MaxValue.ToString()), cleanSession: true);

            await client.SubscribeAsync("foo/bar/topic1", MqttQualityOfService.AtMostOnce); //QoS0
            await client.SubscribeAsync("foo/bar/topic2", MqttQualityOfService.AtLeastOnce); //QoS1
            await client.SubscribeAsync("foo/bar/topic3", MqttQualityOfService.ExactlyOnce); //QoS2


            //client.MessageStream.Subscribe(msg => Console.WriteLine($"Message received in topic {msg.Topic}"));

            var message3 = new MqttApplicationMessage("foo/bar/topic4", Encoding.UTF8.GetBytes("Foo Message 4"));

            await client.PublishAsync(message3, MqttQualityOfService.AtMostOnce); //QoS0

            client.MessageStream.Where(msg => msg.Topic == "foo/bar/topic2").Subscribe(msg => Console.WriteLine($"Message received in topic {msg.Topic}"));
           
        }

    }
}
