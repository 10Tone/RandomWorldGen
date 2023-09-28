
using System.Linq;
using Godot;
using Godot.Collections;

namespace RandomWorldGen;

public partial class World : Node2D
{
	[Export()] private NodePath _tileMapPath;
	[Export()] private MapResource _mapResource;
	
	[Export()] public Array<Vector2I> WaterTiles { get; private set; }
	[Export()] public Array<Vector2I> SandTiles { get; private set; }
	[Export()] public Array<Vector2I> GrassTiles { get; private set; }
	[Export()] public Array<Vector2I> ForestTiles { get; private set; }

	private Vector2I _mapSize = new Vector2I(1920 / 8, 1080 / 8);
	private TileMap _tileMap;
	private Dictionary<Vector2, float> _temperature;
	private Dictionary<Vector2, float> _moisture;
	private Dictionary<Vector2, float> _altitude;
	private Dictionary<Vector2, float> _biome;
	private RandomNumberGenerator _rng;
	private Vector2I _sand = new (0, 0);
	private Vector2I _grass = new (1, 0);
	private Vector2I _forest = new (0, 1);
	private Vector2I _water = new (1, 1);

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
		noise.NoiseType = _mapResource.NoiseType;
		noise.FractalType = _mapResource.FractalType;
		noise.DomainWarpEnabled = true;
		noise.DomainWarpAmplitude = 10;
		noise.Frequency = freq * 0.01f;
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
					_tileMap.SetCell(0, pos, 0, _water);
					WaterTiles.Add(pos);
				}
				//sand
				else if (alt >= _mapResource.AltitudeTreshold && alt < _mapResource.AltitudeTreshold * _mapResource.SandMultiplier)
				{
					_tileMap.SetCell(0, pos, 0, _sand);
					SandTiles.Add(pos);
				}

				else
				{
					if (temp < _mapResource.TemperatureTreshold)
					{
						_tileMap.SetCell(0, pos, 0, _grass);
						GrassTiles.Add(pos);
					}
					else
					{
						_tileMap.SetCell(0, pos, 0, _forest);
						ForestTiles.Add(pos);
					}
					
				}
			}
		}

		// ReplaceSurroundingTiles(_sand, _grass);
		RemoveOneTilePlacement();
		// ReplaceSurroundingTiles(_sand, _sand, _grass);

		// ReplaceSurroundingTiles(_grass, _grass);
	}

	private void RemoveOneTilePlacement()
	{
		ReplaceSurroundingTiles(_forest, _grass, _grass);
		ReplaceSurroundingTiles(_forest, _sand, _sand);
		ReplaceSurroundingTiles(_sand, _water, _water);
		ReplaceSurroundingTiles(_sand, _grass, _grass);
		ReplaceSurroundingTiles(_grass, _sand, _sand);
		ReplaceSurroundingTiles(_grass, _forest, _forest);
		ReplaceSurroundingTiles(_water, _sand, _sand);
	}

	private void ReplaceSurroundingTiles(Vector2I targetTile, Vector2I surroundedByTile, Vector2I newTile)
	{
		foreach (var pos in GetSurroundedCells(targetTile, surroundedByTile))
		{
			_tileMap.SetCell(0, pos, 0, newTile);
		}
	}

	private Array<Vector2I> GetSurroundedCells(Vector2I targetAtlasId, Vector2I surroundedByAtlasId)
	{
		var surroundedTiles = new Array<Vector2I>();
		foreach (var cellPos in _tileMap.GetUsedCells(0))
		{
			if (_tileMap.GetCellAtlasCoords(0, cellPos, true) != targetAtlasId) continue;
			var tiles = new Array<Vector2I>();
			var surroundingNeighbours = _tileMap.GetSurroundingCells(cellPos);
			foreach (var cell in surroundingNeighbours)
			{
				if (_tileMap.GetCellAtlasCoords(0, cell, true) == surroundedByAtlasId)
				{
					tiles.Add(cell);
				}

				if (tiles.Count >= 4)
				{
					surroundedTiles.Add(cellPos);
				}
			}
		}
		return surroundedTiles;
	}

	private void GenerateWorld()
	{
		ResetTileArrays();
		_temperature = GenerateMap(_mapResource.Temperature.Frequency, _mapResource.Temperature.FractalOctaves);
		_moisture = GenerateMap(_mapResource.Moisture.Frequency, _mapResource.Moisture.FractalOctaves);
		_altitude = GenerateMap(_mapResource.Altitude.Frequency, _mapResource.Altitude.FractalOctaves);
		SetTile();

	}

	private void ResetTileArrays()
	{
		WaterTiles = new Array<Vector2I>();
		SandTiles = new Array<Vector2I>();
		GrassTiles = new Array<Vector2I>();
		ForestTiles = new Array<Vector2I>();
	}
}