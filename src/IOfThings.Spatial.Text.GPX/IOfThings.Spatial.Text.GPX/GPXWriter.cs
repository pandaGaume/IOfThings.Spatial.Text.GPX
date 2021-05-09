using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

namespace IOfThings.Spatial.Text.GPX
{

    public class GPXWriter : PathXMLWriter
    {
        public const string DefaultCreator = "MyGeoLive";
        const string XmlSchemaStr = "http://www.w3.org/2001/XMLSchema-instance";

        public static void Save(GeoPath p_object, string p_version, string p_creator, Stream p_out)
        {
            new GPXWriter(p_out, p_version, p_creator).Write(p_object);
        }

        public static void Save(GeoPath p_object, Stream p_out)
        {
            new GPXWriter(p_out).Write(p_object);
        }

        readonly string _version = null;
        readonly string _creator = null;

        public GPXWriter(String filePath) : this(new FileStream(filePath, FileMode.OpenOrCreate)) { }
        public GPXWriter(Stream p_out) : this(p_out, GPX.defaultVersion, DefaultCreator) { }
        public GPXWriter(Stream p_out, string p_version, string p_creator) : base(p_out) { _version = p_version; _creator = p_creator; }

        //private void SaveLink(Link p_l, XmlWriter p_w)
        //{
        //    p_w.WriteStartElement(GPX.XML.Tags.link);
        //    if (p_l.Href != null) p_w.WriteAttributeString(GPX.XML.Attributes.href, p_l.Href);
        //    if (p_l.Text != null) p_w.WriteElementString(null, GPX.XML.Tags.text, GPX.Namespace, p_l.Text);
        //    if (p_l.Type != null) p_w.WriteElementString(null, GPX.XML.Tags.type, GPX.Namespace, p_l.Type);
        //    p_w.WriteEndElement();
        //}

        private void SaveMetadata(GeoPath Path, XmlWriter p_w)
        {
            p_w.WriteStartElement(GPX.XML.Tags.metadata);
            if (Path.Name != null) p_w.WriteElementString(null, GPX.XML.Tags.name, GPX.Namespace, Path.Name);
            //if (Path.Description != null) p_w.WriteElementString(null, GPX.XML.Tags.desc, GPX.Namespace, Path.Description);
            //if (l_m.Author != null)
            //{
            //    p_w.WriteStartElement(GPX.XML.Tags.author);
            //    p_w.WriteEndElement();
            //}
            //if (l_m.Copyright != null)
            //{
            //    p_w.WriteStartElement(GPX.XML.Tags.copyright);
            //    p_w.WriteEndElement();
            //}

            //if (Path.Link != null) SaveLink(l_m.Link, p_w);

            //if (Path.Time != null)
            //{
            //    p_w.WriteStartElement(GPX.XML.Tags.time);
            //    p_w.WriteEndElement();
            //}

            //if (l_m.Keywords.Count != 0)
            //{
            //    p_w.WriteStartElement(GPX.XML.Tags.keywords);
            //    p_w.WriteEndElement();
            //}

            //p_w.WriteStartElement(GPX.XML.Tags.bounds);
            //Envelope l_e = p_o.Bounds;
            //p_w.WriteAttributeString(GPX.XML.Attributes.minlat, l_e.LowerCorner.Lat.ToString(null, Geography.CultureInfoSet));
            //p_w.WriteAttributeString(GPX.XML.Attributes.minlon, l_e.UpperCorner.Lon.ToString(null, Geography.CultureInfoSet));
            //p_w.WriteAttributeString(GPX.XML.Attributes.maxlat, l_e.UpperCorner.Lat.ToString(null, Geography.CultureInfoSet));
            //p_w.WriteAttributeString(GPX.XML.Attributes.maxlon, l_e.LowerCorner.Lon.ToString(null, Geography.CultureInfoSet));
            //p_w.WriteEndElement();

            p_w.WriteEndElement();
        }

        private void SaveWaypoints(IEnumerable<Waypoint> p_wc, XmlWriter p_w)
        {
            if (p_wc.Any())
            {
                Location l_l;
                foreach (Waypoint l_wp in p_wc)
                {
                    l_l = l_wp.Location;
                    p_w.WriteStartElement(GPX.XML.Tags.waypoints);
                    p_w.WriteAttributeString(GPX.XML.Attributes.lat, l_l.Latitude.ToString(null, GPX.CultureInfoSet));
                    p_w.WriteAttributeString(GPX.XML.Attributes.lon, l_l.Longitude.ToString(null, GPX.CultureInfoSet));
                    p_w.WriteEndElement();
                }
            }
        }

        private void SaveTrackSegments(IEnumerable<TrackSegment> p_tc, XmlWriter p_w)
        {
            if (p_tc.Any())
            {
                bool l_isV11 = _version.CompareTo(GPX.V1_1) == 0;
                foreach (TrackSegment l_seg in p_tc)
                {
                    if (l_seg.Trackpoints.Any())
                    {
                        Location l_l;
                        p_w.WriteStartElement(GPX.XML.Tags.trkseg);
                        if (l_seg.Name != null && l_seg.Name != string.Empty) p_w.WriteElementString(GPX.XML.Tags.name, l_seg.Name);

                        foreach (Point l_tp in l_seg.Trackpoints)
                        {
                            l_l = l_tp.Location;
                            p_w.WriteStartElement(GPX.XML.Tags.trackpoint);
                            p_w.WriteAttributeString(GPX.XML.Attributes.lat, l_l.Latitude.ToString(null, GPX.CultureInfoSet));
                            p_w.WriteAttributeString(GPX.XML.Attributes.lon, l_l.Longitude.ToString(null, GPX.CultureInfoSet));
                            if (l_l.HasAltitude())
                            {
                                p_w.WriteElementString(GPX.XML.Tags.elevation, ((int)l_l.Altitude).ToString(null, GPX.CultureInfoSet));
                            }
                            p_w.WriteElementString(GPX.XML.Tags.time, l_tp.Time.ToString("o")); // ISO 8601 [-]CCYY-MM-DDThh:mm:ss[Z|(+|-)hh:mm]

                           
                            p_w.WriteEndElement();

                        }
                        p_w.WriteEndElement();
                    }
                }
            }
        }

        protected void SaveTracks(IEnumerable<Track> p_o, XmlWriter p_w)
        {
            foreach (Track track in p_o)
            {
                p_w.WriteStartElement(GPX.XML.Tags.track);
                if (track.Name != null && track.Name != string.Empty) p_w.WriteElementString(GPX.XML.Tags.name, track.Name);
                SaveTrackSegments(track.Segments, p_w);
                p_w.WriteEndElement();
            }
        }

        protected override void WritePath(GeoPath Path, XmlWriter p_w)
        {
            p_w.WriteStartElement(GPX.XML.Tags.gpx);
            p_w.WriteAttributeString("xmlns", GPX.NMEAPreferedPrefix, null, GPX.NMEANamespace);
            p_w.WriteAttributeString("xmlns", "xsi", null, XmlSchemaStr);
            p_w.WriteAttributeString("xsi", "schemaLocation", null, GPX.SchemaLocation);
            if (_version != null) p_w.WriteAttributeString(GPX.XML.Attributes.version, _version);
            if (_creator != null) p_w.WriteAttributeString(GPX.XML.Attributes.creator, _creator);

            // Metadata
            SaveMetadata(Path, p_w);

            // waypoint
            SaveWaypoints(Path.Waypoints, p_w);

            // tracks
            SaveTracks(Path.Tracks, p_w);

            p_w.WriteEndElement();
        }
    }
}
