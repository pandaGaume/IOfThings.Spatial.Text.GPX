using System;
using System.IO;
using System.Xml;

namespace IOfThings.Spatial.Text.GPX
{
    public abstract class PathXMLWriter : IDisposable
    {
        Stream _out;
        bool _disposed = false;

        protected PathXMLWriter(Stream p_out) { _out = p_out; }

        ~PathXMLWriter()
        {
            Dispose(false);
        }

        /// <summary>
        /// Disposes this instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        public void Close()
        {
            _out?.Dispose();
        }
        public void Flush()
        {
            _out?.Flush();
        }
        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _out?.Dispose();
                _disposed = true;
            }
        }

        public void Write(GeoPath p_object)
        {
            XmlWriterSettings l_settings = new XmlWriterSettings();
            l_settings.ConformanceLevel = ConformanceLevel.Fragment;
            l_settings.OmitXmlDeclaration = true;
#if DEBUG
            l_settings.Indent = true;
#endif
            XmlWriter l_w = XmlWriter.Create(_out, l_settings);
            WritePath(p_object, l_w);
            l_w.Flush();
        }

        abstract protected void WritePath(GeoPath p_o, XmlWriter p_w);

    }
}

