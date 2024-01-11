using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;

/// <summary>
/// Saves and loads terrain heightmap and splatmap data
/// </summary>

[ExecuteInEditMode]
public class TerrainMapsDataHandler : MonoBehaviour
{
    public Terrain[] allTerrainTiles;
    public Terrain[] nearbyTerrainTiles;
    public string fileSuffix = ".bin";

    [ContextMenu("Save All Terrains")]
    public void SaveAllTerrainTiles()
    {
        allTerrainTiles = GetComponentsInChildren<Terrain>();
        foreach (Terrain terrain in allTerrainTiles)
        {
            string fullPath = Path.Combine(Application.persistentDataPath, terrain.name + fileSuffix);
            SaveTerrainTileMapsBinary(terrain, fullPath);
        }
    }
    public void SaveNearbyTiles()
    {
        nearbyTerrainTiles = GetComponentsInChildren<Terrain>();
        foreach (Terrain terrain in nearbyTerrainTiles)
        {
            string fullPath = Path.Combine(Application.persistentDataPath, terrain.name + fileSuffix);
            SaveTerrainTileMapsBinary(terrain, fullPath);
        }
    }
    // Formats to binary and compresses WorldMapData before saving it to file
    public void SaveTerrainTileMapsBinary(Terrain terrain, string fileName)
    {
        TerrainTileMaps terrainTileMaps = GetTerrainTileMaps(terrain);

        // Format data to binary before saving to file
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        using (FileStream fileStream = File.Create(fileName))
        {
            // Compress data using GZip
            using (GZipStream compressionStream = new GZipStream(fileStream, System.IO.Compression.CompressionLevel.Optimal))
            {
                binaryFormatter.Serialize(compressionStream, terrainTileMaps);
            }
        }
    }
    // Get and return heightmap data and splatmap data of a terrain tile
    public TerrainTileMaps GetTerrainTileMaps(Terrain terrain)
    {
        TerrainTileMaps terrainTileMaps = new TerrainTileMaps();

        TerrainData terrainData = terrain.terrainData;

        // Store heightmap data
        float[,] heightmapData = terrainData.GetHeights(0, 0, terrainData.heightmapResolution, terrainData.heightmapResolution);
        terrainTileMaps.heightmapData = new float[terrainData.heightmapResolution * terrainData.heightmapResolution];
        for (int y = 0; y < terrainData.heightmapResolution; y++)
        {
            for (int x = 0; x < terrainData.heightmapResolution; x++)
            {
                terrainTileMaps.heightmapData[y * terrainData.heightmapResolution + x] = heightmapData[x, y];
            }
        }

        // Store splatmap texture data
        Texture2D splatmapTexture = SplatmapToTexture2D(terrainData);
        terrainTileMaps.splatmapTextureFormat = splatmapTexture.format;
        terrainTileMaps.splatmapPNGData = ImageConversion.EncodeToPNG(splatmapTexture);

        return terrainTileMaps;
    }
    // Convert splatmap/alphamap data to 2D texture
    private Texture2D SplatmapToTexture2D(TerrainData terrainData)
    {
        float[,,] splatmaps = terrainData.GetAlphamaps(0, 0, terrainData.alphamapWidth, terrainData.alphamapHeight);
        int splatmapLayerCount = terrainData.alphamapLayers;

        Texture2D splatmapTexture = new Texture2D(terrainData.alphamapWidth, terrainData.alphamapHeight, TextureFormat.RGBA32, false);

        for (int y = 0; y < terrainData.alphamapHeight; y++)
        {
            for (int x = 0; x < terrainData.alphamapWidth; x++)
            {
                Color pixelColor = new Color(0, 0, 0, 0);

                for (int layer = 0; layer < splatmapLayerCount; layer++)
                {
                    pixelColor[layer % 4] = splatmaps[x, y, layer];

                    if (layer % 4 == 3 || layer == splatmapLayerCount - 1)
                    {
                        splatmapTexture.SetPixel(x, y, pixelColor);
                        pixelColor = new Color(0, 0, 0, 0);
                    }
                }
            }
        }

        splatmapTexture.Apply();
        return splatmapTexture;
    }

    [ContextMenu("Load All Terrains")]
    public void LoadAllTerrains()
    {
        allTerrainTiles = GetComponentsInChildren<Terrain>();
        foreach (Terrain terrain in allTerrainTiles)
        {
            string fullPath = Path.Combine(Application.persistentDataPath, terrain.name + fileSuffix);
            LoadTerrainTileMapsBinary(terrain, fullPath);
        }
    }
    public void LoadNearbyTerrainTiles()
    {
        nearbyTerrainTiles = GetComponentsInChildren<Terrain>();
        foreach (Terrain terrain in nearbyTerrainTiles)
        {
            string fullPath = Path.Combine(Application.persistentDataPath, terrain.name + fileSuffix);
            LoadTerrainTileMapsBinary(terrain, fullPath);
        }
    }
    // Opens a binary formatted file and decompresses data before loading TerrainArrayData from file and applying
    public void LoadTerrainTileMapsBinary(Terrain terrain, string fileName)
    {
        if (File.Exists(fileName))
        {
            // Open binary data
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            using (FileStream fileStream = File.Open(fileName, FileMode.Open))
            {
                // Decompress data before loading from file
                using (GZipStream decompressionStream = new GZipStream(fileStream, System.IO.Compression.CompressionMode.Decompress))
                {
                    TerrainTileMaps terrainTileMaps = (TerrainTileMaps)binaryFormatter.Deserialize(decompressionStream);
                    ApplyTerrainTileMaps(terrain, terrainTileMaps);
                }
            }
        }
        else
        {
            Debug.LogError("File not found: " + fileName);
        }
    }
    // Apply heightmap data and load/apply splatmap data
    public void ApplyTerrainTileMaps(Terrain terrain, TerrainTileMaps terrainTileMaps)
    {
        TerrainData terrainData = terrain.terrainData;

        // Apply heightmap data
        float[,] loadedHeightmapArray = new float[terrainData.heightmapResolution, terrainData.heightmapResolution];
        for (int y = 0; y < terrainData.heightmapResolution; y++)
        {
            for (int x = 0; x < terrainData.heightmapResolution; x++)
            {
                loadedHeightmapArray[x, y] = terrainTileMaps.heightmapData[y * terrainData.heightmapResolution + x];
            }
        }
        terrainData.SetHeights(0, 0, loadedHeightmapArray);

        // Apply splatmap data
        Texture2D loadedSplatmapTexture = new Texture2D(terrainData.alphamapWidth, terrainData.alphamapWidth, terrainTileMaps.splatmapTextureFormat, false);
        ImageConversion.LoadImage(loadedSplatmapTexture, terrainTileMaps.splatmapPNGData);
        ApplyTexture2DToSplatmap(terrain.terrainData, loadedSplatmapTexture);
    }
    // Convert 2D texture to splatmap/alphamap data and apply to terrain
    private void ApplyTexture2DToSplatmap(TerrainData terrainData, Texture2D splatmapTexture)
    {
        int width = splatmapTexture.width;
        int height = splatmapTexture.height;
        int layerCount = terrainData.alphamapLayers;

        float[,,] splatmapData = new float[width, height, layerCount];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Color pixelColor = splatmapTexture.GetPixel(x, y);

                for (int layer = 0; layer < layerCount; layer++)
                {
                    splatmapData[x, y, layer] = pixelColor[layer % 4];

                    if (layer % 4 == 3 || layer == layerCount - 1)
                    {
                        pixelColor = splatmapTexture.GetPixel(x, y);
                    }
                }
            }
        }

        terrainData.SetAlphamaps(0, 0, splatmapData);
    }
}