using System.Xml.Serialization;

namespace Witsml.Data.Measures
{
    public class WitsmlMeasureWithDatum : Measure
    {
        [XmlAttribute("datum")] public string Datum { get; set; } = null;
    }
}
