using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TakSharp
{

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

            public EventDetail detail;

        }

        public static string FormatTime(DateTime dt)
        {
            return dt.ToString("o");
        }
    }

    [XmlInclude(typeof(LoginDetail))]
    public class EventDetail
    {
        public Remarks remarks;

        [XmlElement(ElementName = "__chat")]
        public Chat chat;


        [XmlElement(ElementName = "link")]
        public Link link;
    }

    public class Link
    {
        [XmlAttributeAttribute()] public string uid;
        [XmlAttributeAttribute()] public string parent_callsign;
        [XmlAttributeAttribute()] public string type;
        [XmlAttributeAttribute()] public string relation;
    }

    public class Chat
    {
        [XmlAttributeAttribute()] public string id;
        [XmlAttributeAttribute()] public string chatroom;
        [XmlAttributeAttribute()] public string senderCallsign;
        [XmlAttributeAttribute()] public string groupOwner;
        [XmlAttributeAttribute()] public string messageId;




    }

    public class Remarks
    {
        [XmlAttributeAttribute()] public string source;
        [XmlAttributeAttribute()] public string sourceId;
        [XmlAttributeAttribute()] public string to;
        [XmlAttributeAttribute()] public string time;
        [XmlText()]
        public string content;
    }

    // CoT Event for Login. Xml node should be Login

    [XmlRoot("login")]
    public class LoginDetail : EventDetail
    {
        [XmlAttributeAttribute()]
        public string contact;
        [XmlAttributeAttribute()]
        public string endpoint;
        [XmlAttributeAttribute()]
        public string group;
        [XmlAttributeAttribute()]
        public string password;
        [XmlAttributeAttribute()]
        public string status;
        [XmlAttributeAttribute()]
        public string uid;
        [XmlAttributeAttribute()]
        public string user;
        [XmlAttributeAttribute()]
        public string nonce;
        [XmlAttributeAttribute()]
        public string deviceId;
    }
}
