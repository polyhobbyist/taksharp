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

namespace TakSharp
{
    //  https://freetakteam.github.io/FreeTAKServer-User-Docs/API/REST_APIDoc/#list-of-supported-attitudes  




    public class CoT
    {
        public enum Attitude
        {
            [Description("friend")]
            Friend,
            [Description("friendly")]
            Friendly,
            [Description("hostile")]
            Hostle,
            [Description("unknown")]
            Unknown,
            [Description("pending")]
            Pending,
            [Description("assumed")]
            Assumed,
            [Description("neutral")]
            Neutral,
            [Description("suspect")]
            Suspect
        };

        public enum CoTHow
        { 
            mensurated,
            [Description("h-t")]
            human,
            [Description("h-t")]
            retyped,
            [Description("m-")]
            machine,
            [Description("m-g")]
            gps,
            [Description("h-g-i-g-o")]
            gigo,
            [Description("a-f-G-E-V-9-1-1")]
            mayday,
            [Description("h-e")]
            estimated,
            [Description("h-c")]
            calculated,
            [Description("h-t")]
            transcribed,
            [Description("h-p")]
            pasted,
            [Description("m-m")]
            magnetic,
            [Description("m-n")]
            ins,
            [Description("m-s")]
            simulated,
            [Description("m-c")]
            configured,
            [Description("m-r")]
            radio,
            [Description("m-p")]
            passed,
            [Description("m-p")]
            propagated,
            [Description("m-f")]
            fused,
            [Description("m-a")]
            tracker,
            [Description("m-g-n")]
            ins_gps,
            [Description("m-g-d")]
            dgps,
            [Description("m-r-e")]
            eplrs,
            [Description("m-r-p")]
            plrs,
            [Description("m-r-d")]
            doppler,
            [Description("m-r-v")]
            vhf,
            [Description("m-r-t")]
            tadil,
            [Description("m-r-t-a")]
            tadila,
            [Description("m-r-t-b")]
            tadilb,
            [Description("m-r-t-j")]
            tadilj
    }
        //{
        //  "longitude = -77.0104,
        //  "latitude = 38.889,
        //  "attitude = "hostile",
        //  "bearing = 132, 
        //  "distance = 1,
        //  "geoObject = "Gnd Combat Infantry Sniper",
        //  "how = "nonCoT",
        //  "name = "Putin",
        //  "timeout = 600  
        //}



        public string uid;
        public double longitude;
        public double latitude;
        public string attitude;
        public string how;
        public string name;
        public double bearing;
        public double distance;
        public string role;
        public string team;
        public uint timeout;


        [XmlRoot("event")]
        public class Event
        {
            [XmlAttributeAttribute()]
            public string version;
            [XmlAttributeAttribute()]
            public string type;
            [XmlAttributeAttribute()]
            public string uid;
            [XmlAttributeAttribute()]
            public string how;
            [XmlAttributeAttribute()]
            public string time;
            [XmlAttributeAttribute()]
            public string start;
            [XmlAttributeAttribute()]
            public string stale;
        }

        public static string FormatTime(DateTime dt)
        {
            return dt.ToString("o");
        }
    }

    public interface ITakNetwork
    {
        Task Connect(string ip, int port);

        Stream GetStream();
    };

    class TakTcpNetwork : ITakNetwork
    {
        private TcpClient client = new TcpClient();

        public async Task Connect(string ip, int port)
        {
            var ipAddress = Dns.GetHostAddresses(ip)[0];
            await client.ConnectAsync(ip, port);
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
        }


        public async Task putObject(object o)
        { 
            var x = new XmlSerializer(o.GetType());

            stream = network.GetStream();

            x.Serialize(stream, o);

            await Task.Delay(0);
        }

        public async Task startListening()
        {
            listening = true;
            await Task.Run(() => ReadClientAsync());
        }
        public void stopListening()
        {
            listening = false;
        }


        private void  ReadClientAsync()
        {
            byte[] bf = new byte[2048];
            try
            {
                while (listening)
                {
                    int i;
                    Byte[] bytes = new Byte[256];
                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        string data = Encoding.ASCII.GetString(bytes, 0, i);

                        var x = new XmlSerializer(typeof(CoT.Event));
                        var e = x.Deserialize(stream) as CoT.Event;

                        if (OnCot != null)
                        {
                            OnCot(e);
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }
    };
}
