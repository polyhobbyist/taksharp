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
        public const string typeAll = "a-f-G-U-C-I";
        public const string typeAirAsset = "a";
        public const string typeFrendlyForceLocation = "f";
        public const string typeGroundAsset = "G";
        public const string typeUnknownType = "U";
        public const string typeControlStation = "C";
        public const string typeImagry = "I";

        public const string attitudeFriend = "friend";
        public const string attitudeFriendly = "friendly";
        public const string attitudeHostile = "hostile";
        public const string attitudeUnknown = "unknown";
        public const string attitudePending = "pending";
        public const string attitudeAssumed = "assumed";
        public const string attitudeNeutral = "neutral";
        public const string attitudeSuspect = "suspect";

        public const string howHuman = "h-t";
        public const string howRetyped = "h-t";
        public const string howMachine = "m-";
        public const string howGps = "m-g";
        public const string howGigo = "h-g-i-g-o";
        public const string howMayday = "a-f-G-E-V-9-1-1";
        public const string howEstimated = "h-e";
        public const string howCalculated = "h-c";
        public const string howTranscribed = "h-t";
        public const string howPasted = "h-p";
        public const string howMagnetic = "m-m";
        public const string howIns = "m-n";
        public const string howSimulated = "m-s";
        public const string howConfigured = "m-c";
        public const string howRadio = "m-r";
        public const string howPassed = "m-p";
        public const string howPropagated = "m-p";
        public const string howFused = "m-f";
        public const string howTracker = "m-a";
        public const string howIns_gps = "m-g-n";
        public const string howDgps = "m-g-d";
        public const string howEplrs = "m-r-e";
        public const string howPlrs = "m-r-p";
        public const string howDoppler = "m-r-d";
        public const string howVhf = "m-r-v";
        public const string howTadil = "m-r-t";
        public const string howTadila = "m-r-t-a";
        public const string howTadilb = "m-r-t-b";
        public const string howTadilj = "m-r-t-j";

        [Serializable()]
        [XmlRoot("events")]
        public class Events
        {
            [XmlElement("event")]
            public Event[] events;
        }

        [Serializable()]
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
            public string time = DateTime.UtcNow.ToString("O");
            [XmlAttributeAttribute()]
            public string start = DateTime.UtcNow.ToString("O");
            [XmlAttributeAttribute()]
            public string stale = DateTime.UtcNow.AddSeconds(60).ToString("O");

            public Point point;

            public EventDetail detail;

        }

        public static string FormatTime(DateTime dt)
        {
            return dt.ToString("O");
        }
    }

    [Serializable()]
    [XmlRoot("point")]
    public class Point
    {
        [XmlAttributeAttribute()]
        public double lat;

        [XmlAttributeAttribute()]
        public double lon;

        [XmlAttributeAttribute()]
        public double ce;

        [XmlAttributeAttribute()]
        public double le;

        [XmlAttributeAttribute()]
        public double hae;
    }

    [Serializable()]
    public class EventDetail
    {
        public LinkAttribute link_attr;
        public Remarks remarks;

        [XmlElement("__chat")]
        public Chat chat;

        [XmlElement("link")]
        public Link[] link;

        public Takv takv;

        [XmlElement("__group")]
        public Group group;

        public Track track;

        public Contact contact;

        public Uid uid;

        public Status status;
    }

    [Serializable()]
    [XmlRoot("status")]
    public class Status
    {
        [XmlAttributeAttribute()] public string battery;

    }

    [Serializable()]
    [XmlRoot("link_attr")]
    public class LinkAttribute
    {
        [XmlAttributeAttribute()] public string order;
        [XmlAttributeAttribute()] public string routetype;
        [XmlAttributeAttribute()] public string direction;
        [XmlAttributeAttribute()] public string method;
        [XmlAttributeAttribute()] public string color;
    }

    [Serializable()]
    public class Subscription
    {
        public string event_type;
        public TimePeriod time_period;
    }

    [Serializable()]
    public class TimePeriod
    {
        public string start;
        public string stop;
    }

    [Serializable()]
    public class Uid
    {
        [XmlAttributeAttribute()] public string droid;
               
    }

    [Serializable()]
    public class Contact
    {
        [XmlAttributeAttribute()] public string callsign;
        [XmlAttributeAttribute()] public string endpoint;
        [XmlAttributeAttribute()] public string phone;
    }

    [Serializable()]
    [XmlRoot("track")]
    public class Track
    {
        [XmlAttributeAttribute()] public double course;
        [XmlAttributeAttribute()] public double speed;
    }

    [Serializable()]
    [XmlRoot("__group")]
    public class Group
    {
        [XmlAttributeAttribute()] public string name;
        [XmlAttributeAttribute()] public string role;
    }

    [Serializable()]
    [XmlRoot("takv")]
    public class Takv
    {
        [XmlAttributeAttribute()] public string version;
        [XmlAttributeAttribute()] public string platform;
        [XmlAttributeAttribute()] public string os;
        [XmlAttributeAttribute()] public string device;
    }

    [Serializable()]
    [XmlRoot("link")]
    public class Link
    {
        [XmlAttributeAttribute()] public string uid;
        [XmlAttributeAttribute()] public string parent_callsign;
        [XmlAttributeAttribute()] public string type;
        [XmlAttributeAttribute()] public string relation;
        [XmlAttributeAttribute()] public string point;
    }

    [Serializable()]
    [XmlRoot("chat")]
    public class Chat
    {
        [XmlAttributeAttribute()] public string id;
        [XmlAttributeAttribute()] public string chatroom;
        [XmlAttributeAttribute()] public string senderCallsign;
        [XmlAttributeAttribute()] public string groupOwner;
        [XmlAttributeAttribute()] public string messageId;
    }

    [Serializable()]
    [XmlRoot("remarks")]
    public class Remarks
    {
        [XmlAttributeAttribute()] public string source;
        [XmlAttributeAttribute()] public string sourceId;
        [XmlAttributeAttribute()] public string to;
        [XmlAttributeAttribute()] public string time;
        [XmlText()]
        public string content;
    }
}
