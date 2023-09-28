using Godot;

namespace RandomWorldGen;

[GlobalClass]
public partial class MapPropertyResource : Resource
{
    [Export(PropertyHint.Range, "0.1, 2,")] public float Frequency { get; set; }
    [Export(PropertyHint.Range, "1, 8,")] public int FractalOctaves { get; set; }
    
}