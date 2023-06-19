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
using System.Diagnostics.Eventing.Reader;

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

        public string callsign;
        public string group;
        public string role;
        public string endpoint;



        public Tak(ITakNetwork net)
        {
            this.network = net;
        }

        private string getDeviceId()
        {
            return Guid.NewGuid().ToString();
        }

        private string getUid()
        {
            return Environment.UserName;
        }

        public async Task login(string callsign, string password, string group, string role = "Team Member", string endpoint = "*:-1:stcp")
        {
            this.callsign = callsign;
            this.group = group;
            this.role = role;

            // Login to Tak
            var cot = new CoT.Event();
            cot.uid = getUid();
            cot.type = CoT.typeAll;
            cot.version = "2.0";
            cot.how = CoT.howGigo;
            cot.uid = getUid();
            cot.stale = DateTime.UtcNow.AddMinutes(60).ToString("O");
            cot.detail = new EventDetail()
            {
                uid = new Uid() { droid = cot.uid },
                contact = new Contact() { callsign = callsign, endpoint = endpoint },
                takv = new Takv() { device = getDeviceId(), os = Environment.OSVersion.Version.ToString(), platform = Environment.OSVersion.Platform.ToString(), version = "2.0" },
                group = new Group() { name = group, role = role },
                status = new Status() { battery = "100" },
                track = new Track() { course = 0.0, speed = 0.0 }

            };
            await sendEvent(cot);
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


                await sendEvent(g);

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


        public async Task sendEvent(CoT.Event cot)
        {
            // fills in some common goo
            cot.version = "2.0";
            cot.type = CoT.typeAll;
            cot.how = CoT.howGigo;
            cot.uid = getUid();

            if (cot.detail == null)
            {
                cot.detail = new EventDetail();
            }

            cot.detail.uid = new Uid() { droid = cot.uid };
            cot.detail.takv = new Takv() { device = "", os = "", platform = "", version = "2.0" };
            cot.detail.contact = new Contact() { callsign = this.callsign, endpoint = this.endpoint };
            cot.detail.group = new Group() { name = this.group, role = this.role };

            await sendRawEvent(cot);
        }

        public async Task sendRawEvent(CoT.Event cot)
        { 
            var x = new XmlSerializer(cot.GetType());

            x.Serialize(stream, cot);

            await Task.Yield();
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
                    // Cot can send multiple events in a block, so we're going to wrap them in an Events object, and process them as an array

                    messageBuilder.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                    messageBuilder.AppendLine("<events>");

                    do
                    {
                        var r = stream.Read(buffer, 0, buffer.Length);
                        var s = Encoding.UTF8.GetString(buffer, 0, r);
                        messageBuilder.Append(s);
                    } while (stream.DataAvailable);

                    if (messageBuilder.Length > 0)
                    {
                        messageBuilder.AppendLine("</events>");
                        var message = messageBuilder.ToString();
                        Console.WriteLine(message);
                        var x = new XmlSerializer(typeof(CoT.Events));
                        var es = (CoT.Events)x.Deserialize(new StringReader(message));
                        foreach (var e in es.events)
                        {
                            if (OnCot != null)
                            {
                                OnCot(e);
                            }
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
    