
using System.Globalization;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace USTGlobal.PIP.ApplicationCore.Helpers
{
    public static class XmlHelper
    {
        public static string ToXml(this object input)
        {
            var serializer = new XmlSerializer(input.GetType());
            var stringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var xml = string.Empty;

            using (var writer = XmlWriter.Create(stringWriter, new XmlWriterSettings { OmitXmlDeclaration = true }))
            {
                serializer.Serialize(writer, input);
                xml = stringWriter.ToString();
            }

            return xml;
        }
    }
}
