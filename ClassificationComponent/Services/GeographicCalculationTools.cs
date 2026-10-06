using System.Text.Json.Serialization;

public class GeoJsonFeatureCollection
{
    [JsonPropertyName("features")]
    public List<Feature> Features { get; set; } = new();
}

public class Feature
{
    [JsonPropertyName("properties")]
    public Properties Properties { get; set; } = new();

    [JsonPropertyName("geometry")]
    public Geometry Geometry { get; set; } = new();
}

public class Properties
{
    [JsonPropertyName("region")]
    public string Region { get; set; } = string.Empty;
}

public class Geometry
{
    [JsonPropertyName("coordinates")]
    public List<List<List<double>>> Coordinates { get; set; } = new();
}
