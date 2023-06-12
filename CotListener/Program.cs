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

            var g = new CoT.Event();
            g.version = "2.0";
            g.uid = "takSharp";
            g.type = "t-x-d-d";
            g.uid = "takPong";
            g.how = "m-g";
            g.time = CoT.FormatTime(DateTime.UtcNow);
            g.start = CoT.FormatTime(DateTime.UtcNow);
            g.stale = CoT.FormatTime(DateTime.UtcNow + TimeSpan.FromHours(1));


            await tak.putObject(g);

            await tak.listen();

            Console.WriteLine("And there you have it");
        }
    }
}
