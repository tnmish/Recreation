using Xunit;
using Recreation.Converters;
using NetTopologySuite.Geometries;
using System.Text.Json;

namespace Recreation.Tests
{
    public class GeometryJsonConverterTests
    {
        [Fact]
        public void Read_ValidPointGeometry_ReturnsPoint()
        {
            var converter = new GeometryJsonConverter();
            var json = "\"POINT(1.0 2.0)\"";
            var jsonElement = JsonDocument.Parse(json).RootElement;

            var result = converter.Read(ref jsonElement, typeof(Geometry), new JsonSerializerOptions());

            Assert.IsType<Point>(result);
            var point = (Point)result;
            Assert.Equal(1.0, point.X);
            Assert.Equal(2.0, point.Y);
        }

        [Fact]
        public void Read_ValidPolygonGeometry_ReturnsPolygon()
        {
            var converter = new GeometryJsonConverter();
            var json = "\"POLYGON((0 0, 1 0, 1 1, 0 1, 0 0))\"";
            var jsonElement = JsonDocument.Parse(json).RootElement;

            var result = converter.Read(ref jsonElement, typeof(Geometry), new JsonSerializerOptions());

            Assert.IsType<Polygon>(result);
            var polygon = (Polygon)result;
            Assert.Equal(5, polygon.ExteriorRing.NumPoints); // Should have 5 points (closed ring)
        }

        [Fact]
        public void Read_ValidLineStringGeometry_ReturnsLineString()
        {
            var converter = new GeometryJsonConverter();
            var json = "\"LINESTRING(0 0, 1 1, 2 2)\"";
            var jsonElement = JsonDocument.Parse(json).RootElement;

            var result = converter.Read(ref jsonElement, typeof(Geometry), new JsonSerializerOptions());

            Assert.IsType<LineString>(result);
            var lineString = (LineString)result;
            Assert.Equal(3, lineString.NumPoints);
        }

        [Fact]
        public void Read_InvalidGeometryString_ReturnsNull()
        {
            var converter = new GeometryJsonConverter();
            var json = "\"INVALID_GEOMETRY(0 0)\"";
            var jsonElement = JsonDocument.Parse(json).RootElement;

            var result = converter.Read(ref jsonElement, typeof(Geometry), new JsonSerializerOptions());

            Assert.Null(result);
        }

        [Fact]
        public void Read_EmptyString_ReturnsNull()
        {
            var converter = new GeometryJsonConverter();
            var json = "\"\"";
            var jsonElement = JsonDocument.Parse(json).RootElement;

            var result = converter.Read(ref jsonElement, typeof(Geometry), new JsonSerializerOptions());

            Assert.Null(result);
        }

        [Fact]
        public void Write_ValidPoint_WritesCorrectJson()
        {
            var converter = new GeometryJsonConverter();
            var point = new Point(3.0, 4.0);
            var options = new JsonSerializerOptions();

            var result = JsonSerializer.Serialize(point, typeof(Geometry), options);

            Assert.Equal("\"POINT(3 4)\"", result);
        }

        [Fact]
        public void Write_ValidPolygon_WritesCorrectJson()
        {
            var converter = new GeometryJsonConverter();
            var coordinates = new Coordinate[]
            {
                new Coordinate(0, 0),
                new Coordinate(1, 0),
                new Coordinate(1, 1),
                new Coordinate(0, 1),
                new Coordinate(0, 0)
            };
            var polygon = new Polygon(new LinearRing(coordinates));
            var options = new JsonSerializerOptions();

            var result = JsonSerializer.Serialize(polygon, typeof(Geometry), options);

            Assert.Equal("\"POLYGON((0 0, 1 0, 1 1, 0 1, 0 0))\"", result);
        }
    }
}