using System.Net;
using TakSharp;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TakSharp.tests
{
    class TakUnitTestNetwork : ITakNetwork
    {
        public string ip = "";
        public int port = 0;
        MemoryStream stream = new MemoryStream();

        public TakUnitTestNetwork()
        {

        }

        internal void set(string s)
        {
            stream.Position = 0;
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
        }

        internal CoT.Event? getCurrentEvent()
        {
            var x = new XmlSerializer(typeof(CoT.Event));

            stream.Position = 0;

            return x.Deserialize(stream) as CoT.Event;

        }

        public async Task Connect(string ip, int port)
        {
            this.ip = ip;
            this.port = port;

            await Task.Delay(0);
        }

        public Stream GetStream()
        {
            return stream;
        }

    };

    [TestClass]
    public class UnitTakSharp
    {
        const string test1Pong =
            @"<event 
                version=""2.0"" 
                uid=""takPong"" 
                type=""t-x-c-t-r"" 
                how=""h-g-i-g-o"" 
                start=""2022-10-06T03:24:58.747654Z"" 
                time=""2022-10-06T03:24:58.747640Z"" 
                stale=""2022-10-06T03:25:58.747656Z"">
                    <point le=""9999999.0"" ce=""9999999.0"" hae=""9999999.0"" lon=""0"" lat=""0"" />
            </event>";

        const string testLocation =
        @"<event 
            version=""2.0"" 
            uid=""S-1-5-21-71875714-25651929-3250119497-1001"" 
            type=""a-f-G-U-C-I"" 
            how=""h-e"" 
            start=""2022-10-06T03:24:57.71Z""
            time=""2022-10-06T03:24:57.71Z"" 
            stale=""2022-10-06T03:31:12.71Z"">
            <detail>
                <__group name=""Cyan"" role=""Team Member"" />
                    <status battery=""100"" />
                    <takv version=""4.7.0.163"" 
                        platform=""WinTAK-CIV"" 
                        device=""Razer Blade 17 (2022) - RZ09-0423"" 
                        os=""Microsoft Windows 11 Home"" />
                    <track course=""0.00000000"" speed=""0.00000000"" />
                    <contact callsign=""OLDS"" endpoint=""*:-1:stcp"" />
                    <uid Droid=""OLDS"" />
                    <precisionlocation />
            </detail>
            <point le=""9999999"" ce=""9999999"" hae=""9999999"" lon=""-122.013928207906"" lat=""47.6272416580357"" />
        </event>";

        void AssertCotEqual(CoT.Event g, CoT.Event e)
        {
            Assert.AreEqual(g.version, e.version);
            Assert.AreEqual(g.uid, e.uid);
            Assert.AreEqual(g.type, e.type);
            Assert.AreEqual(g.how, e.how);
            Assert.AreEqual(g.stale, e.stale);
            Assert.AreEqual(g.time, e.time);
            Assert.AreEqual(g.start, e.start);
        }


        [TestMethod]
        public async Task TestWritePong()
        {
            TakUnitTestNetwork tun = new TakUnitTestNetwork();
            Tak t = new Tak(tun);

            await t.connect("172.31.248.54", 8087);

            Assert.AreEqual(tun.ip, "172.31.248.54");
            Assert.AreEqual(tun.port, 8087);

            var g = new CoT.Event();
            g.version = "2.0";
            g.uid = "takSharp";
            g.type = "t-x-d-d";
            g.uid = "takPong";
            g.how = "m-g";
            g.time = CoT.FormatTime(DateTime.UtcNow);
            g.start = CoT.FormatTime(DateTime.UtcNow);
            g.stale = CoT.FormatTime(DateTime.UtcNow + TimeSpan.FromHours(1));


            await t.putObject(g);

            var e = tun.getCurrentEvent();
            Assert.IsNotNull(e);

            Assert.AreEqual(g.version, e.version);  
            Assert.AreEqual(g.uid, e.uid);
            Assert.AreEqual(g.type, e.type);
            Assert.AreEqual(g.how, e.how);
        }

        [TestMethod]
        public async Task TestReadOneCot()
        {
            TakUnitTestNetwork tun = new TakUnitTestNetwork();

            tun.set(test1Pong);

            Tak t = new Tak(tun);

            await t.connect("172.31.248.54", 8087);
            t.OnCot += (CoT.Event e) =>
            {
                Assert.Equals(e.how, "h-g-i-g-o");
                Assert.Equals(e.uid, "takPong");
                t.stopListening();
            };

            await t.startListening();
        }
    }
}