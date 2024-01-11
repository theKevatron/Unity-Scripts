using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Biome", menuName = "Terrain/Biome", order = 1)]
public class Biome : ScriptableObject
{
    public string biomeName;
    public float minHeight;
    public float maxHeight;
    public TerrainPalette terrainPalette;
}

[System.Serializable]
public class TerrainPalette
{
    public TerrainLayer[] terrainLayers;
}
