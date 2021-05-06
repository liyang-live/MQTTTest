using System;
using System.Net.Mqtt;
using System.Net.Mqtt.Sdk;
using System.Text;
using System.Timers;

namespace MicroSoftMqttTest
{
    class Program
    {
        private static Server server = new Server();
        static void Main(string[] args)
        {

            server.Start();

            Timer timer = new Timer();
            timer.Interval = 500;
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
            Console.ReadLine();
        }

        private static void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            server.Send();
        }
    }

    public class Server
    {

        public IMqttConnectedClient client;

        public async void Start()
        {

            var configuration = new MqttConfiguration
            {
                BufferSize = 128 * 1024,
                Port = 55555,
                KeepAliveSecs = 10,
                WaitTimeoutSecs = 2,
                MaximumQualityOfService = MqttQualityOfService.AtMostOnce,
                AllowWildcardsInTopicFilters = true,

            };
            //var     = await MqttClient.CreateAsync("192.168.1.10", configuration);

            IMqttServer mqttServer = MqttServer.Create(configuration);

            mqttServer.ClientConnected += MqttServer_ClientConnected;
            mqttServer.ClientDisconnected += MqttServer_ClientDisconnected;
            mqttServer.MessageUndelivered += MqttServer_MessageUndelivered;
            mqttServer.Start();


            client = await mqttServer.CreateClientAsync();
            // client.PublishAsync();


            Console.WriteLine("Hello World!");


        }


        public async void Send()
        {

            var message1 = new MqttApplicationMessage("foo/bar/topic1", Encoding.UTF8.GetBytes("Foo Message 1"));
            var message2 = new MqttApplicationMessage("foo/bar/topic2", Encoding.UTF8.GetBytes("Foo Message 2"));
            var message3 = new MqttApplicationMessage("foo/bar/topic3", Encoding.UTF8.GetBytes("Foo Message 3"));

            await client.PublishAsync(message1, MqttQualityOfService.AtMostOnce); //QoS0
            await client.PublishAsync(message2, MqttQualityOfService.AtLeastOnce); //QoS1
            await client.PublishAsync(message3, MqttQualityOfService.ExactlyOnce); //QoS2

        }



        private void MqttServer_MessageUndelivered(object sender, MqttUndeliveredMessage e)
        {
            Console.WriteLine(e.SenderId + "  " + e.Message.Topic);
        }

        private void MqttServer_ClientDisconnected(object sender, string e)
        {
            Console.WriteLine(e);
        }

        private void MqttServer_ClientConnected(object sender, string e)
        {
            Console.WriteLine(e);
        }
    }
}
