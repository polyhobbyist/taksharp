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

        Stream GetStream();
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

        public Stream GetStream()
        {
            return client.GetStream();
        }
    }
    public class Tak
    {
        Stream stream;
        ITakNetwork network;

        public delegate void cotHandler(CoT.Event e);

        public event cotHandler OnCot;

        bool listening = false;

        public Tak(ITakNetwork net)
        {
            this.network = net;
        }

        public async Task connect(string ip, int port)
        {
            await network.Connect(ip, port);
            stream = network.GetStream();
        }


        public async Task putObject(object o)
        { 
            var x = new XmlSerializer(o.GetType());

            x.Serialize(stream, o);

            await Task.Delay(0);
        }

        public Task listen()
        {
            listening = true;
            return Task.Run(() => ReadClientAsync());
        }
        public void stopListening()
        {
            listening = false;
        }


        private void  ReadClientAsync()
        {
            byte[] buffer = new byte[2048];
            var xmlReader = XmlReader.Create(stream); 
            while (listening && network.isConnected())
            {
                try
                {
                    // This stream object contains a stream of XML blocks,
                    // which may not be complete on read.
                    // Read the stream until we have a complete XML block.

                    int bytesRead = 0;
                    while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        string data = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                        Console.WriteLine(data);

                        var x = new XmlSerializer(typeof(CoT.Event));
                        if (x.CanDeserialize(xmlReader))
                        {
                            var e = x.Deserialize(xmlReader) as CoT.Event;

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
    