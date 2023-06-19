// See https://aka.ms/new-console-template for more information
using TakSharp;
using System.CommandLine;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.ComponentModel.DataAnnotations;

namespace CotListener
{
    class Program
    {

        static async Task Main(string ip, int port)
        { 
            var tak = new Tak(new TakTcpNetwork());
            await tak.connect(ip, port);

            tak.OnCot += (e) =>
            {
                var x = new XmlSerializer(typeof(CoT.Event));
                x.Serialize(Console.Out, e);
            };

            tak.uid = "testing";
            tak.group = "cyan";
            tak.callsign = "Meatloaf";

            // Login to Tak
            var l = new CoT.Event();
            l.stale = DateTime.UtcNow.AddMinutes(60).ToString("O");
            l.detail = new LoginDetail() 
            {
                uid = new Uid() { droid = l.uid }, 
                deviceId="testing", 
                password="", nonce=Tak.nonce()
            };
            await tak.sendEvent(l);


            // Self identify
            var id = new CoT.Event();
            id.stale = DateTime.UtcNow.AddMinutes(60).ToString("O");

            id.detail = new EventDetail()
            {
                track = new Track() { course = 0.0, speed = 0.0 },
            };
            await tak.sendEvent(id);

            // Subscribe to messaging
            var msgSub = new CoT.Event();
            msgSub.type= "t-x-c-t";
            msgSub.stale = DateTime.UtcNow.AddMinutes(60).ToString("O");
            msgSub.detail = new EventDetail()
            {
                subscription = new Subscription() { event_type= "a-f-I", time_period = new TimePeriod() { start= DateTime.UtcNow.ToString("O"), stop= msgSub.stale } },
                track = new Track() { course = 0.0, speed = 0.0 },
            };
            await tak.sendEvent(msgSub);

            tak.listen();

            Console.WriteLine("And there you have it");
        }
    }
}
