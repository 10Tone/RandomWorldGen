using Godot;
using Godot.Collections;

namespace RandomWorldGen;

[GlobalClass]
public partial class MapResource : Resource
{
    [Export()] public FastNoiseLite.NoiseTypeEnum NoiseType { get; set; }
    [Export()] public FastNoiseLite.FractalTypeEnum FractalType { get; set; }
    
    
    [Export(PropertyHint.Range, "0, 1,")] public float AltitudeTreshold { get; set; }
    [Export(PropertyHint.Range, "0, 1,")] public float TemperatureTreshold { get; set; }
    [Export(PropertyHint.Range, "0, 1,")] public float MoistureTreshold { get; set; }
    [Export()] public MapPropertyResource Altitude { get; set; }
    [Export()] public MapPropertyResource Temperature { get; set; }
    [Export()] public MapPropertyResource Moisture { get; set; }
    [Export(PropertyHint.Range, "1, 3,")] public float SandMultiplier { get; set; }
}