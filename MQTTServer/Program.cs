using MQTTnet;
using MQTTnet.Server;
using System;

namespace MQTTServer
{
    class Program
    {
         static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            MqttServerHelper serverHelper = new MqttServerHelper();
            serverHelper.Start();

        }

    
    }
}
