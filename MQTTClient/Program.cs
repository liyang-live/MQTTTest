using MQTTnet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQTTClient
{
    class Program
    {
        static void Main(string[] args)
        {
            MQTTClientHelper clientHelper = new MQTTClientHelper();
            clientHelper.Start();

        }
    }
}
