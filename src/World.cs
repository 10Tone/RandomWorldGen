
using Godot;
using Godot.Collections;

namespace RandomWorldGen;

public partial class World : Node2D
{
	[Export()] private NodePath _tileMapPath;
	[Export()] private MapResource _mapResource;

	private Vector2I _mapSize = new Vector2I(1920 / 8, 1080 / 8);
	private TileMap _tileMap;
	private Dictionary<Vector2, float> _temperature;
	private Dictionary<Vector2, float> _moisture;
	private Dictionary<Vector2, float> _altitude;
	private Dictionary<Vector2, float> _biome;
	private RandomNumberGenerator _rng;

	public override void _Ready()
	{
		_tileMap = GetNode<TileMap>(_tileMapPath);
		_rng = new RandomNumberGenerator();
		// _seed = _rng.RandiRange(0, 10000);
		GenerateWorld();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(Input.IsActionJustPressed("spacebar"))
		{
			_tileMap.Clear();
			GenerateWorld();
		}
	}

	private Dictionary<Vector2, float> GenerateMap(float freq, int oct)
	{
		var noise = new FastNoiseLite();
		var seed = _rng.RandiRange(0, 10000);
		noise.Seed = seed;
		noise.NoiseType = FastNoiseLite.NoiseTypeEnum.Simplex;
		noise.Frequency = freq;
		noise.FractalOctaves = oct;

		var mapGrid = new Dictionary<Vector2, float>();

		for (int x = 0; x < _mapSize.X; x++)
		{
			for (int y = 0; y < _mapSize.Y; y++)
			{
				var a = 2 * (Mathf.Abs(noise.GetNoise2D(x, y)));
				mapGrid[new Vector2(x, y)] = a;
			}
		}

		return mapGrid;
	}

	private void SetTile()
	{
		for (var x = 0; x < _mapSize.X; x++)
		{
			for (var y = 0; y < _mapSize.Y; y++)
			{
				var pos = new Vector2I(x, y);
				var alt = _altitude[pos];
				var temp = _temperature[pos];
				var moist = _moisture[pos];
				
				//water
				if (alt < _mapResource.AltitudeTreshold)
				{
					_tileMap.SetCell(0, pos, 0, new Vector2I(1, 1));
					
				}
				//sand
				else if (alt >= _mapResource.AltitudeTreshold && alt < _mapResource.AltitudeTreshold * 2)
				{
					_tileMap.SetCell(0, pos, 0, new Vector2I(0, 0));
				}

				else
				{
					if (temp < _mapResource.TemperatureTreshold)
					{
						_tileMap.SetCell(0, pos, 0, new Vector2I(1, 0));
					}
					else {_tileMap.SetCell(0, pos, 0, new Vector2I(0, 1));}
					
					
				}
			}
		}
	}

	private void GenerateWorld()
	{
		_temperature = GenerateMap(_mapResource.Temperature.Frequency, _mapResource.Temperature.FractalOctaves);
		_moisture = GenerateMap(_mapResource.Moisture.Frequency, _mapResource.Moisture.FractalOctaves);
		_altitude = GenerateMap(_mapResource.Altitude.Frequency, _mapResource.Altitude.FractalOctaves);
		SetTile();
		// var noise = new FastNoiseLite();
		// var rng = new RandomNumberGenerator();
		// noise.Seed = rng.RandiRange(0, 10);
		//
		// // var cells = new List<Vector2>();
		//
		// for (var x = 0; x < _mapSize.X; x++)
		// {
		// 	for (var y = 0; y < _mapSize.Y; y++)
		// 	{
		// 		var a = noise.GetNoise2D(x, y);
		// 		if (a < 0.4)
		// 		{
		// 			var cell = new Vector2I(x, y);
		// 			_tileMap.SetCell(0, cell, 0, new Vector2I(1, 1));
		// 		}
		// 		if (a < 0.2)
		// 		{
		// 			var cell = new Vector2I(x, y);
		// 			_tileMap.SetCell(0, cell, 0, new Vector2I(1, 0));
		// 		}
		// 		else
		// 		{
		// 			var cell = new Vector2I(x, y);
		// 			_tileMap.SetCell(0, cell, 0, new Vector2I(0, 1));
		// 		}
		// 	}
		// }
	}
}