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

            await tak.login("Meatloaf", "", "Cyan");

            tak.listen();

            Console.WriteLine("And there you have it");
        }
    }
}
