using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.IO.Compression;

public class TerrainToJson : MonoBehaviour
{
    public Terrain terrain;
    [System.Serializable]
    public class TerrainProperties
    {
        public float[,] heightmap;
        public Vector3 terrainSize;
        public int detailResolution;
        public int alphamapResolution;

        // Add a default constructor for deserialization
        public TerrainProperties() { }

        public void SetTerrainDataProperties(TerrainData terrainData)
        {
            int heightmapWidth = terrainData.heightmapResolution;
            int heightmapHeight = terrainData.heightmapResolution;
            heightmap = terrainData.GetHeights(0, 0, heightmapWidth, heightmapHeight);

            terrainSize = terrainData.size;
            detailResolution = terrainData.detailResolution;
            alphamapResolution = terrainData.alphamapResolution;
        }
    }

    private string GetTerrainPropertiesJson()
    {
        JsonSerializerSettings settings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            },
            Formatting = Formatting.None // Change to None
        };

        TerrainProperties terrainProperties = new TerrainProperties();
        terrainProperties.SetTerrainDataProperties(terrain.terrainData);
        string jsonString = JsonConvert.SerializeObject(terrainProperties, settings);
        return jsonString;
    }


    private void SaveJsonToFile(string jsonString, string filename)
    {
        string path = Path.Combine(Application.persistentDataPath, filename);

        using (FileStream fileStream = new FileStream(path, FileMode.Create))
        using (GZipStream gzipStream = new GZipStream(fileStream, System.IO.Compression.CompressionLevel.Optimal))
        using (StreamWriter streamWriter = new StreamWriter(gzipStream))
        {
            streamWriter.Write(jsonString);
        }

        Debug.Log("Terrain properties saved to: " + path);
    }

    [ContextMenu("Save Terrain Properties to JSON")]
    private void SaveTerrainPropertiesToJson()
    {
        string jsonString = GetTerrainPropertiesJson();
        SaveJsonToFile(jsonString, "terrainProperties.json");
    }

    private string LoadJsonFromFile(string filename)
    {
        string path = Path.Combine(Application.persistentDataPath, filename);

        if (File.Exists(path))
        {
            using (FileStream fileStream = new FileStream(path, FileMode.Open))
            using (GZipStream gzipStream = new GZipStream(fileStream, CompressionMode.Decompress))
            using (StreamReader streamReader = new StreamReader(gzipStream))
            {
                string jsonString = streamReader.ReadToEnd();
                return jsonString;
            }
        }
        else
        {
            Debug.LogError("File not found: " + path);
            return null;
        }
    }

    private void SetTerrainProperties(TerrainProperties terrainProperties)
    {
        if (terrainProperties != null)
        {
            terrain.terrainData.SetHeights(0, 0, terrainProperties.heightmap);
            terrain.terrainData.size = terrainProperties.terrainSize;
            terrain.terrainData.alphamapResolution = terrainProperties.alphamapResolution;
        }
    }

    private void SetTerrainPropertiesFromJson(string jsonString)
    {
        if (!string.IsNullOrEmpty(jsonString))
        {
            TerrainProperties terrainProperties = JsonConvert.DeserializeObject<TerrainProperties>(jsonString);
            SetTerrainProperties(terrainProperties);
        }
    }

    [ContextMenu("Load Terrain Properties from JSON")]
    private void LoadTerrainPropertiesFromJson()
    {
        string jsonString = LoadJsonFromFile("terrainProperties.json");
        SetTerrainPropertiesFromJson(jsonString);
    }
}