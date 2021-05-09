using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml.Linq;

namespace IOfThings.Spatial.Text.GPX
{
    public class GPXReader
    {
        // Represents the EN-US culture, used for numers in NMEA sentences
        public static CultureInfo GPXCultureInfo = new CultureInfo("en-US");

        public static GeoPath Read(Uri p_uri)
        {
            return Read(p_uri, EllipticSystem.WGS84);
        }

        public static GeoPath Read(Uri p_uri, EllipticSystem origin)
        {
            using (WebClient l_c = new WebClient())
            {
                Stream l_in = l_c.OpenRead(p_uri);
                GeoPath l_n = Read(l_in, origin);
                return l_n;
            }
        }

        public static GeoPath Read(Stream p_in, EllipticSystem origin)
        {
            return Read(p_in, origin, new GeoPath());
        }

        public static GeoPath Read(Stream p_in, EllipticSystem origin, GeoPath target)
        {
            XDocument gpxDoc = XDocument.Load(p_in);
            GeoPath t = target ?? new GeoPath();

            // avoid to use the namespace for the track and waypoints, while many vendor does not support namesapce and often are implement 
            // the <trk xmlns=""> tricks to edit the gpx, which is NOT standard and cause the gpx to be invalidated.

            t.Waypoints = from waypoint in gpxDoc.Descendants().Where(d => d.Name.LocalName == GPX.XML.Tags.waypoints)
                          select new Waypoint(new Location(Double.Parse(waypoint.Attribute(GPX.XML.Attributes.lat).Value, GPXCultureInfo),
                                                           Double.Parse(waypoint.Attribute(GPX.XML.Attributes.lon).Value, GPXCultureInfo),
                                                           waypoint.Element(waypoint.Name.Namespace + GPX.XML.Tags.elevation) != null ? Double.Parse(waypoint.Element(waypoint.Name.Namespace + GPX.XML.Tags.elevation).Value, GPXCultureInfo) : 0))
                          {
                              Name = waypoint.Element(waypoint.Name.Namespace + GPX.XML.Tags.name)?.Value,
                              Time = waypoint.Element(waypoint.Name.Namespace + GPX.XML.Tags.time) != null ? DateTime.Parse(waypoint.Element(waypoint.Name.Namespace + GPX.XML.Tags.time).Value).ToUniversalTime() : DateTime.UtcNow
                          };


            t.Tracks = from track in gpxDoc.Descendants().Where(d => d.Name.LocalName == GPX.XML.Tags.track)
                       select new Track
                       {
                           Name = track.Element(track.Name.Namespace + GPX.XML.Tags.name)?.Value,
                           Segments = (
                                from tracksegment in track.Descendants(track.Name.Namespace + GPX.XML.Tags.trkseg)
                                select new TrackSegment
                                {
                                    Name = tracksegment.Element(track.Name.Namespace + GPX.XML.Tags.name)?.Value,
                                    Trackpoints =
                                            from trackpoint in tracksegment.Descendants(track.Name.Namespace + GPX.XML.Tags.trackpoint)
                                            select new Point(new Location(Double.Parse(trackpoint.Attribute(GPX.XML.Attributes.lat).Value, GPXCultureInfo),
                                                                          Double.Parse(trackpoint.Attribute(GPX.XML.Attributes.lon).Value, GPXCultureInfo),
                                                                          trackpoint.Element(track.Name.Namespace + GPX.XML.Tags.elevation) != null ? Double.Parse(trackpoint.Element(track.Name.Namespace + GPX.XML.Tags.elevation).Value, GPXCultureInfo) : 0))
                                            {
                                                Time = trackpoint.Element(track.Name.Namespace + GPX.XML.Tags.time) != null ? DateTime.Parse(trackpoint.Element(track.Name.Namespace + GPX.XML.Tags.time).Value).ToUniversalTime() : DateTime.UtcNow
                                            }
                                }
                           )
                       };

            return t;
        }

    }
}
