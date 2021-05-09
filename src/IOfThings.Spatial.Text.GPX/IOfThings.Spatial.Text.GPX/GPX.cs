using System.Globalization;

namespace IOfThings.Spatial.Text.GPX
{
    public class GPX
    {
        public const string NMEANamespace = "http://www.blueforest.com/GPX/1/1/NMEA";
        public const string NMEAPreferedPrefix = "nmea";
        public const string Namespace = "http://www.topografix.com/GPX/1/1/gpx.xsd";
        
        public const string PreferedPrefix = "gpx";
        public const string SchemaLocation = "http://www.topografix.com/GPX/1/1 http://www.topografix.com/GPX/1/1/gpx.xsd";

        public const string V1_1 = "1.1";
        public const string mimeStr = "application/gpx+xml";

        internal const string defaultVersion = V1_1;

        public static CultureInfo CultureInfoSet = new CultureInfo("en-US");

        public class XML
        {
            public class Tags
            {
                public const string gpx = "gpx";
                public const string metadata = "metadata";
                public const string waypoints = "wpt";
                public const string routes = "rte";
                public const string track = "trk";

                public const string name = "name";
                public const string desc = "desc";
                public const string author = "author";
                public const string copyright = "copyright";
                public const string link = "link";
                public const string time = "time";
                public const string keywords = "keywords";
                public const string bounds = "bounds";

                public const string text = "text";
                public const string type = "type";
                public const string trkseg = "trkseg";
                public const string trackpoint = "trkpt";
                public const string elevation = "ele";
                public const string course = "course";
                public const string speed = "speed";
                public const string extensions = "extensions";
            }

            public class Attributes
            {
                public const string version = "version";
                public const string creator = "creator";

                public const string minlat = "minlat";
                public const string maxlat = "maxlat";
                public const string minlon = "minlon";
                public const string maxlon = "maxlon";

                public const string href = "href";

                public const string lat = "lat";
                public const string lon = "lon";
            }
        }
    }
}
