﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

[Serializable]
public struct TileState
{
	public Sprite Sprite;
	public float Damage;
}

public abstract class BasicTile : LayerTile
{
	[Tooltip("What it sounds like when walked over")]
	public FloorTileType floorTileType = FloorTileType.floor;

	[Tooltip("Allow gases to pass through the cell this tile occupies?")]
	[FormerlySerializedAs("AtmosPassable")]
	[SerializeField]
	private bool atmosPassable = false;

	[Tooltip("Does this tile form a seal against the floor?")]
	[FormerlySerializedAs("IsSealed")]
	[SerializeField]
	private bool isSealed = false;

	[Tooltip("Should this tile get initialized with Space gasmix at round start (e.g. asteroids)?")]
	public bool SpawnWithNoAir;

	[FormerlySerializedAs("OreCategorie")]
	[SerializeField]
	private OreCategory oreCategory;

	public TileState[] HealthStates;

	[Tooltip("Does this tile allow items / objects to pass through it?")]
	[FormerlySerializedAs("Passable")]
	[SerializeField]
	private bool passable = false;

	[Tooltip("Can this tile be mined?")]
	[FormerlySerializedAs("Mineable")]
	[SerializeField]
	private bool mineable = false;
	/// <summary>
	/// Can this tile be mined?
	/// </summary>
	public bool Mineable => mineable;

	[Tooltip("What things are allowed to pass through this even if it is not passable?")]
	[FormerlySerializedAs("PassableException")]
	[SerializeField]
	private PassableDictionary passableException = null;

	[Tooltip("What is this tile's max health?")]
	[FormerlySerializedAs("MaxHealth")]
	[SerializeField]
	private float maxHealth = 0f;
	public float MaxHealth => maxHealth;

	[Tooltip("Armor of this tile")]
	[FormerlySerializedAs("Armor")]
	[SerializeField]
	private Armor armor = new Armor
	{
		Melee = 90,
		Bullet = 90,
		Laser = 90,
		Energy = 90,
		Bomb = 90,
		Bio = 100,
		Rad = 100,
		Fire = 100,
		Acid = 90
	};

	/// <summary>
	/// Armor of this tile
	/// </summary>
	public Armor Armor => armor;

	[Tooltip("Sound this tile makes when hit")] [SerializeField]
	private string[] hitSounds = null;
	public string[] HitSounds => hitSounds;

	[Tooltip("Set the remains this tile can spawn once destroyed")] [SerializeField]
	private List<TileDestroyedRemains> tileDestroyedRemains = null;
	public List<TileDestroyedRemains> TileDestroyedRemains => tileDestroyedRemains;

	[Tooltip("How does the tile change as its health changes?")]
	[FormerlySerializedAs("HealthStates")]
	[SerializeField]
	private TileState[] healthStates;

	[Tooltip("Resistances of this tile.")]
	[FormerlySerializedAs("Resistances")]
	[SerializeField]
	private Resistances resistances = null;
	/// <summary>
	/// Resistances of this tile.
	/// </summary>
	public Resistances Resistances => resistances;

	[Tooltip("Interactions which can occur on this tile. They will be checked in the order they appear in this list (top to bottom).")]
	[SerializeField]
	private List<TileInteraction> tileInteractions = null;
	/// <summary>
	/// Interactions which can occur on this tile.
	/// </summary>
	public List<TileInteraction> TileInteractions => tileInteractions;

	[Tooltip("What object to spawn when it's deconstructed or destroyed.")]
	[SerializeField]
	private GameObject spawnOnDeconstruct = null;
	/// <summary>
	/// Object to spawn when deconstructed.
	/// </summary>
	public GameObject SpawnOnDeconstruct => spawnOnDeconstruct;

	[Tooltip("How much of the object to spawn when it's deconstructed. Defaults to 1 if" +
	         " an object is specified and this is 0.")]
	[SerializeField]
	private int spawnAmountOnDeconstruct = 1;
	/// <summary>
	/// How many of the object to spawn when it's deconstructed.
	/// </summary>
	public int SpawnAmountOnDeconstruct => SpawnOnDeconstruct == null ? 0 : Mathf.Max(1, spawnAmountOnDeconstruct);

	public override void RefreshTile(Vector3Int position, ITilemap tilemap)
	{
		foreach (Vector3Int p in new BoundsInt(-1, -1, 0, 3, 3, 1).allPositionsWithin)
		{
			tilemap.RefreshTile(position + p);
		}
	}

	/// <summary>
	/// Checks if the tile is Passable by the ColliderType
	/// It will return the default Passable bool unless an exception is avalaible in PassableException
	/// </summary>
	/// <param name="colliderType"></param>
	/// <returns>IsPassable</returns>
	public bool IsPassable(CollisionType colliderType)
	{
		if (passableException.ContainsKey(colliderType))
		{
			return passableException[colliderType];
		}
		else
		{
			return passable;
		}
	}

	public bool IsAtmosPassable()
	{
		return atmosPassable;
	}

	public bool IsSpace()
	{
		return IsAtmosPassable() && !isSealed;
	}
}

[Serializable]
public class TileDestroyedRemains
{
	[Tooltip("What game object to spawn as a remain")]
	public GameObject Object = null;
	[Tooltip("Amount of said game object")]
	public int Amount = 1;
	[Tooltip("We will make a roll from 0,0 to 1,0. If the roll value is inside this range, we will spawn the game object")]
	[NaughtyAttributes.MinMaxSlider(0,1)]
	public Vector2 ProbabilityRange;
}
