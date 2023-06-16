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
            tak.callsign = "Meatloaf";

            // Login to Tak
            var l = new CoT.Event();
            l.detail = new LoginDetail() 
            {
                uid = new Uid() { droid = l.uid }, 
                deviceId="testing", 
                password="", nonce=Tak.nonce() 
            };
            await tak.sendEvent(l);


            // Self identify
            var id = new CoT.Event();

            id.detail = new EventDetail()
            {
                track = new Track() { course = 0.0, speed = 0.0 },
            };
            await tak.sendEvent(id);

            tak.listen();

            Console.WriteLine("And there you have it");
        }
    }
}
