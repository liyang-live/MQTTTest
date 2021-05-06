using MQTTnet;
using MQTTnet.Diagnostics;
using MQTTnet.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using MQTTnet.Core.Adapter;
//using MQTTnet.Core.Diagnostics;
//using MQTTnet.Core.Protocol;
//using MQTTnet.Core.Server;

namespace MQTTServer
{
    public class MqttServerHelper
    {

        public async void Start()
        {
            // Start a MQTT server.
            var mqttServer = new MqttFactory().CreateMqttServer();
            //mqttServer.CreateMqttServer();
            MqttServerOptions serverOptions = new MqttServerOptions();
            serverOptions.Storage = new RetainedMessageHandler();
            //  mqttServer.MqttNetGlobalLog
            //MqttNetGlobalLog
            //mqttServer.DefaultLogger.
            // Write all trace messages to the console window.

            //mqttServer.IMqttNetLogger

            //MqttNetGlobalLogger.LogMessagePublished += (s, e) =>
            //{
            //    var trace = $">> [{e.TraceMessage.Timestamp:O}] [{e.TraceMessage.ThreadId}] [{e.TraceMessage.Source}] [{e.TraceMessage.Level}]: {e.TraceMessage.Message}";
            //    if (e.TraceMessage.Exception != null)
            //    {
            //        trace += Environment.NewLine + e.TraceMessage.Exception.ToString();
            //    }

            //    Console.WriteLine(trace);
            //};

            await mqttServer.StartAsync(serverOptions);

            await mqttServer.PublishAsync("/test", "aaaaaaaaaaaaaaaaaa");

            Console.WriteLine("Press any key to exit.");
            Console.ReadLine();
            await mqttServer.StopAsync();
        }
    }
}
