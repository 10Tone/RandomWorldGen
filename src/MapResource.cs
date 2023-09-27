using Godot;
using Godot.Collections;

namespace RandomWorldGen;

[GlobalClass]
public partial class MapResource : Resource
{
    [Export()] public FastNoiseLite.NoiseTypeEnum NoiseType { get; set; }
    
    [Export()] public float AltitudeTreshold { get; set; }
    [Export()] public float TemperatureTreshold { get; set; }
    [Export()] public float MoistureTreshold { get; set; }
    [Export()] public MapPropertyResource Altitude { get; set; }
    [Export()] public MapPropertyResource Temperature { get; set; }
    [Export()] public MapPropertyResource Moisture { get; set; }
}