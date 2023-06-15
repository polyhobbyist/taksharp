using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Newtonsoft.Json;
using System.Xml.Serialization;
using System.Collections.Specialized;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.VisualBasic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Xml;

namespace TakSharp
{
    //  https://freetakteam.github.io/FreeTAKServer-User-Docs/API/REST_APIDoc/#list-of-supported-attitudes  

    public interface ITakNetwork
    {
        Task Connect(string ip, int port);

        bool isConnected();

        NetworkStream GetStream();
    };

    public class TakTcpNetwork : ITakNetwork
    {
        private TcpClient client = new TcpClient();

        public async Task Connect(string ip, int port)
        {
            var ipAddress = Dns.GetHostAddresses(ip)[0];
            await client.ConnectAsync(ip, port);

        }

        public bool isConnected()
        {
            return client.Connected;
        }

        public NetworkStream GetStream()
        {
            return client.GetStream();
        }
    }
    public class Tak
    {
        NetworkStream stream;
        ITakNetwork network;
        Thread pingThread;

        public delegate void cotHandler(CoT.Event e);

        public event cotHandler OnCot;

        bool listening = false;

        public Tak(ITakNetwork net)
        {
            this.network = net;
        }

        private async void sendPingAsync()
        {
            while (network.isConnected())
            {
                var g = new CoT.Event();
                g.version = "2.0";
                g.uid = "takSharp";
                g.type = "t-x-d-d";
                g.uid = "takPing";
                g.how = "m-g";
                g.time = CoT.FormatTime(DateTime.UtcNow);
                g.start = CoT.FormatTime(DateTime.UtcNow);
                g.stale = CoT.FormatTime(DateTime.UtcNow + TimeSpan.FromSeconds(60));

                await putObject(g);

                await Task.Delay(60000);
            }   
        }

        public async Task connect(string ip, int port)
        {
            await network.Connect(ip, port);
            stream = network.GetStream();

            pingThread = new Thread(sendPingAsync);
            pingThread.Start();
        }


        public async Task putObject(object o)
        { 
            var x = new XmlSerializer(o.GetType());

            x.Serialize(stream, o);

            await Task.Delay(1);
        }

        public void stopListening()
        {
            listening = false;
        }

        public bool isListening()
        {
            return listening;
        }

        static public string nonce()
        {
            Random random = new Random();

            // Utility to generate nonce string
            DateTime created = DateTime.Now;

            return Convert.ToBase64String(new SHA512Managed().ComputeHash(Encoding.ASCII.GetBytes(created + random.Next().ToString())));
        }

        public void listen()
        {
            listening = true;
            byte[] buffer = new byte[2048];
            while (listening && network.isConnected())
            {
                try
                {
                    var messageBuilder = new StringBuilder();
                    do
                    {
                        stream.Read(buffer, 0, buffer.Length);
                        var s = Encoding.UTF8.GetString(buffer);
                        messageBuilder.Append(s);
                    } while (stream.DataAvailable);

                    if (messageBuilder.Length > 0)
                    {
                        var message = messageBuilder.ToString();
                        var x = new XmlSerializer(typeof(CoT.Event));
                        var e = (CoT.Event)x.Deserialize(new StringReader(message));
                        if (OnCot != null)
                        {
                            OnCot(e);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
    };
}
    