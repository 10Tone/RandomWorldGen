using Godot;

namespace RandomWorldGen;

[GlobalClass]
public partial class MapPropertyResource : Resource
{
    [Export()] public float Frequency { get; set; }
    [Export()] public int FractalOctaves { get; set; }
    
}