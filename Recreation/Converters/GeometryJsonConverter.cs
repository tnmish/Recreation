using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Recreation.Converters
{
    internal class GeometryJsonConverter : JsonConverter<Geometry>
    {
        private readonly GeoJsonWriter geoJsonWriter = new();
        private readonly GeoJsonReader geoJsonReader = new();

        public override Geometry? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return geoJsonReader.Read<Geometry?>(reader.GetString());
        }

        public override void Write(Utf8JsonWriter writer, Geometry value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(geoJsonWriter.Write(value));
        }
    }
}
