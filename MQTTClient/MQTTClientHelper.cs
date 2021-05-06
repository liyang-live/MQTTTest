using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;
using MQTTnet.Client.Receiving;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MQTTClient
{
    //public class MQTTClientHelper
    //{

    //    public async void Start()
    //    {
    //        // Create a new MQTT client.
    //        var factory = new MqttFactory();
    //        var mqttClient = factory.CreateMqttClient();

    //        // Use WebSocket connection.
    //        var options = new MqttClientOptionsBuilder().Build();
    //        //.WithWebSocketServer("broker.hivemq.com:8000/mqtt")
    //        //.Build();

    //        await mqttClient.ConnectAsync(options, CancellationToken.None); // Since 3.0.5 with CancellationToken

    //       await mqttClient.SubscribeAsync(new MQTTnet.Client.Subscribing.MqttClientSubscribeOptions
    //        {
    //            TopicFilters = new List<MqttTopicFilter> { new MqttTopicFilter { Topic = "/test" } }
    //        }, CancellationToken.None);

    //        mqttClient.ApplicationMessageReceivedHandler = new MqttApplicationMessageReceivedHandlerDelegate(new Action<MqttApplicationMessageReceivedEventArgs>(OnAppMessage));
    //    }

    //    private async void OnAppMessage(MqttApplicationMessageReceivedEventArgs e)
    //    {
    //    }

    //    private async void OnConnected(MqttClientConnectedEventArgs e)
    //    {
    //    }

    //    private async void OnDisconnected(MqttClientDisconnectedEventArgs e)
    //    {
    //    }


    //    private void doWork(CancellationToken token)
    //    {
    //        if (token.IsCancellationRequested)
    //        {
    //            //做一些结束任务之前的必要工作
    //            return;
    //        }
    //    }
    //}

    public class MQTTClientHelper
    {
        private static MqttClient mqttClient = null;
        private static IMqttClientOptions options = null;
        private static bool runState = false;
        private static bool running = false;

        /// <summary>
        /// 服务器IP
        /// </summary>
        private static string ServerUrl = "182.61.51.85";
        /// <summary>
        /// 服务器端口
        /// </summary>
        private static int Port = 61613;
        /// <summary>
        /// 选项 - 开启登录 - 密码
        /// </summary>
        private static string Password = "ruichi8888";
        /// <summary>
        /// 选项 - 开启登录 - 用户名
        /// </summary>
        private static string UserId = "admin";
        /// <summary>
        /// 主题
        /// <para>China/Hunan/Yiyang/Nanxian</para>
        /// <para>Hotel/Room01/Tv</para>
        /// <para>Hospital/Dept01/Room001/Bed001</para>
        /// <para>Hospital/#</para>
        /// </summary>
        private static string Topic = "China/Hunan/Yiyang/Nanxian";
        /// <summary>
        /// 保留
        /// </summary>
        private static bool Retained = false;
        /// <summary>
        /// 服务质量
        /// <para>0 - 至多一次</para>
        /// <para>1 - 至少一次</para>
        /// <para>2 - 刚好一次</para>
        /// </summary>
        private static int QualityOfServiceLevel = 0;
        public static void Stop()
        {
            runState = false;
        }

        public static bool IsRun()
        {
            return (runState && running);
        }
        /// <summary>
        /// 启动客户端
        /// </summary>
        public static void Start()
        {
            try
            {
                runState = true;
                System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(Work));
                thread.Start();
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private static void Work()
        {
            running = true;
            Console.WriteLine("Work >>Begin");
            try
            {
                var factory = new MqttFactory();
                mqttClient = factory.CreateMqttClient() as MqttClient;

                options = new MqttClientOptionsBuilder()
                    .WithTcpServer(ServerUrl, Port)
                    .WithCredentials(UserId, Password)
                    .WithClientId(Guid.NewGuid().ToString().Substring(0, 5))
                    .Build();

                mqttClient.ConnectAsync(options);
                mqttClient.ConnectedHandler = new MqttClientConnectedHandlerDelegate(new Func<MqttClientConnectedEventArgs, Task>(Connected));
                mqttClient.DisconnectedHandler = new MqttClientDisconnectedHandlerDelegate(new Func<MqttClientDisconnectedEventArgs, Task>(Disconnected));
                mqttClient.ApplicationMessageReceivedHandler = new MqttApplicationMessageReceivedHandlerDelegate(new Action<MqttApplicationMessageReceivedEventArgs>(MqttApplicationMessageReceived));
                while (runState)
                {
                    System.Threading.Thread.Sleep(100);
                }
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp);
            }
            Console.WriteLine("Work >>End");
            running = false;
            runState = false;
        }

        public static void StartMain()
        {
            try
            {
                var factory = new MqttFactory();

                var mqttClient = factory.CreateMqttClient();

                var options = new MqttClientOptionsBuilder()
                    .WithTcpServer(ServerUrl, Port)
                    .WithCredentials(UserId, Password)
                    .WithClientId(Guid.NewGuid().ToString().Substring(0, 5))
                    .Build();

                mqttClient.ConnectAsync(options);

                mqttClient.UseConnectedHandler(async e =>
                {
                    Console.WriteLine("Connected >>Success");
                // Subscribe to a topic
                var topicFilterBulder = new TopicFilterBuilder().WithTopic(Topic).Build();
                    await mqttClient.SubscribeAsync(topicFilterBulder);
                    Console.WriteLine("Subscribe >>" + Topic);
                });

                mqttClient.UseDisconnectedHandler(async e =>
                {
                    Console.WriteLine("Disconnected >>Disconnected Server");
                    await Task.Delay(TimeSpan.FromSeconds(5));
                    try
                    {
                        await mqttClient.ConnectAsync(options);
                    }
                    catch (Exception exp)
                    {
                        Console.WriteLine("Disconnected >>Exception" + exp.Message);
                    }
                });

                mqttClient.UseApplicationMessageReceivedHandler(e =>
                {
                    Console.WriteLine("MessageReceived >>" + Encoding.UTF8.GetString(e.ApplicationMessage.Payload));
                });
                Console.WriteLine(mqttClient.IsConnected.ToString());
            }
            catch (Exception exp)
            {
                Console.WriteLine("MessageReceived >>" + exp.Message);
            }
        }
        /// <summary>
        /// 发布
        /// <paramref name="QoS"/>
        /// <para>0 - 最多一次</para>
        /// <para>1 - 至少一次</para>
        /// <para>2 - 仅一次</para>
        /// </summary>
        /// <param name="Topic">发布主题</param>
        /// <param name="Message">发布内容</param>
        /// <returns></returns>
        public static void Publish(string Topic, string Message)
        {
            try
            {
                if (mqttClient == null) return;
                if (mqttClient.IsConnected == false)
                    mqttClient.ConnectAsync(options);

                if (mqttClient.IsConnected == false)
                {
                    Console.WriteLine("Publish >>Connected Failed! ");
                    return;
                }

                Console.WriteLine("Publish >>Topic: " + Topic + "; QoS: " + QualityOfServiceLevel + "; Retained: " + Retained + ";");
                Console.WriteLine("Publish >>Message: " + Message);
                MqttApplicationMessageBuilder mamb = new MqttApplicationMessageBuilder()
                 .WithTopic(Topic)
                 .WithPayload(Message).WithRetainFlag(Retained);
                if (QualityOfServiceLevel == 0)
                {
                    mamb = mamb.WithAtMostOnceQoS();
                }
                else if (QualityOfServiceLevel == 1)
                {
                    mamb = mamb.WithAtLeastOnceQoS();
                }
                else if (QualityOfServiceLevel == 2)
                {
                    mamb = mamb.WithExactlyOnceQoS();
                }

                mqttClient.PublishAsync(mamb.Build());
            }
            catch (Exception exp)
            {
                Console.WriteLine("Publish >>" + exp.Message);
            }
        }
        /// <summary>
        /// 连接服务器并按标题订阅内容
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private static async Task Connected(MqttClientConnectedEventArgs e)
        {
            try
            {
                List<TopicFilter> listTopic = new List<TopicFilter>();
                if (listTopic.Count() <= 0)
                {
                    var topicFilterBulder = new TopicFilterBuilder().WithTopic(Topic).Build();
                    listTopic.Add(topicFilterBulder);
                    Console.WriteLine("Connected >>Subscribe " + Topic);
                }
                await mqttClient.SubscribeAsync(listTopic.ToArray());
                Console.WriteLine("Connected >>Subscribe Success");
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.Message);
            }
        }
        /// <summary>
        /// 失去连接触发事件
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private static async Task Disconnected(MqttClientDisconnectedEventArgs e)
        {
            try
            {
                Console.WriteLine("Disconnected >>Disconnected Server");
                await Task.Delay(TimeSpan.FromSeconds(5));
                try
                {
                    await mqttClient.ConnectAsync(options);
                }
                catch (Exception exp)
                {
                    Console.WriteLine("Disconnected >>Exception " + exp.Message);
                }
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.Message);
            }
        }
        /// <summary>
        /// 接收消息触发事件
        /// </summary>
        /// <param name="e"></param>
        private static void MqttApplicationMessageReceived(MqttApplicationMessageReceivedEventArgs e)
        {
            try
            {
                string text = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
                string Topic = e.ApplicationMessage.Topic;
                string QoS = e.ApplicationMessage.QualityOfServiceLevel.ToString();
                string Retained = e.ApplicationMessage.Retain.ToString();
                Console.WriteLine("MessageReceived >>Topic:" + Topic + "; QoS: " + QoS + "; Retained: " + Retained + ";");
                Console.WriteLine("MessageReceived >>Msg: " + text);
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.Message);
            }
        }
    }


}
