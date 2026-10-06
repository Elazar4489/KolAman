using System.Text.Json;
public class GeographicClassificationService 
{
    private readonly Dictionary<string, List<(double Lon, double Lat)>> _polygons = new();

    public GeographicClassificationService(string geoJsonFilePath)
    {
        LoadGeoJson(geoJsonFilePath);
    }

    private void LoadGeoJson(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"GeoJSON file not found at: {filePath}");

        string jsonContent = File.ReadAllText(filePath);
        var geoJson = JsonSerializer.Deserialize<GeoJsonFeatureCollection>(jsonContent);

        if (geoJson?.Features == null) return;

        foreach (var feature in geoJson.Features)
        {
            string regionName = feature.Properties.Region; // NORTH, CENTER, SOUTH[span_0](start_span)[span_0](end_span)

            if (feature.Geometry.Coordinates.Count > 0)
            {
                var ring = feature.Geometry.Coordinates[0];
                var points = ring.Select(coord => (Lon: coord[0], Lat: coord[1])).ToList();
                _polygons[regionName] = points;
            }
        }
    }

    public string ClassifyRegion(double lat, double lon)
    {
        string polygonKey = "deep.command";
        foreach (var polygon in _polygons)
        {
            if (IsPointInPolygon(lon, lat, polygon.Value))
            {
                if (polygon.Key == "NORTH") polygonKey = "northern.command";
                else if (polygon.Key == "CENTER") polygonKey = "central.command";
                else if (polygon.Key == "SOUTH") polygonKey = "southern.command";
            }
        }

        return polygonKey;
    }

    private bool IsPointInPolygon(double testLon, double testLat, List<(double Lon, double Lat)> polygon)
    {
        bool inside = false;
        int j = polygon.Count - 1;

        for (int i = 0; i < polygon.Count; j = i++)
        {
            double xi = polygon[i].Lon, yi = polygon[i].Lat;
            double xj = polygon[j].Lon, yj = polygon[j].Lat;

            bool intersect = ((yi > testLat) != (yj > testLat)) &&
                             (testLon < (xj - xi) * (testLat - yi) / (yj - yi) + xi);
            if (intersect)
                inside = !inside;
        }

        return inside;
    }
}
