using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TerrainPainter : MonoBehaviour
{
    public Terrain terrain;
    public TerrainLayer[] terrainLayers;

    public bool update = false;

    public Biome biome;

    public enum ValidatorType { AboveHeight, BelowHeight, AboveSlope, BelowSlope }
    [System.Serializable]
    public struct LayerValidator
    {
        public ValidatorType validatorType;
        public float threshold;
    }

    public LayerValidator[] layerValidators;
    private delegate float Validator(float value, float threshold);
    private Validator[] validators = new Validator[]
    {
        (value, threshold) => Mathf.SmoothStep(1, 0, (value - threshold) / 0.1f),
        (value, threshold) => Mathf.SmoothStep(0, 1, (value - threshold) / 0.1f),
        (value, threshold) => Mathf.SmoothStep(0, 1, (value - threshold) / 5f),
        (value, threshold) => Mathf.SmoothStep(1, 0, (value - threshold) / 5f),
    };

    private void Update()
    {
        if (update)
        {
            terrain = GetComponent<Terrain>();
            terrainLayers = biome.terrainPalette.terrainLayers;
            terrain.terrainData.terrainLayers = terrainLayers;
            PaintTerrain();
        }
    }

    public void PaintTerrain()
    {
        if (terrain == null || terrainLayers == null || terrainLayers.Length != 4 || layerValidators.Length != 4)
        {
            Debug.LogError("Invalid input.");
            return;
        }

        if (terrain.terrainData.terrainLayers.Length != terrainLayers.Length)
        {
            TerrainLayer[] newTerrainLayers = new TerrainLayer[terrainLayers.Length];
            terrainLayers.CopyTo(newTerrainLayers, 0);
            terrain.terrainData.terrainLayers = newTerrainLayers;
        }

        int alphamapResolution = terrain.terrainData.alphamapResolution;
        int heightmapResolution = terrain.terrainData.heightmapResolution;
        float[,] heights = terrain.terrainData.GetHeights(0, 0, heightmapResolution, heightmapResolution);
        float[,,] splatmap = new float[alphamapResolution, alphamapResolution, terrainLayers.Length];

        for (int y = 0; y < alphamapResolution; y++)
        {
            for (int x = 0; x < alphamapResolution; x++)
            {
                float normX = x / (float)(alphamapResolution - 1);
                float normY = y / (float)(alphamapResolution - 1);

                float height = heights[Mathf.RoundToInt(normY * (heightmapResolution - 1)), Mathf.RoundToInt(normX * (heightmapResolution - 1))];
                float slope = terrain.terrainData.GetSteepness(normX, normY);

                float[] layerWeights = new float[terrainLayers.Length];

                for (int i = 0; i < terrainLayers.Length; i++)
                {
                    float value = layerValidators[i].validatorType < ValidatorType.AboveSlope ? height : slope;
                    layerWeights[i] = validators[(int)layerValidators[i].validatorType](value, layerValidators[i].threshold);
                }

                float totalWeight = layerWeights[0] + layerWeights[1] + layerWeights[2] + layerWeights[3];

                for (int i = 0; i < terrainLayers.Length; i++)
                {
                    splatmap[y, x, i] = layerWeights[i] / totalWeight;
                }
            }
        }

        terrain.terrainData.SetAlphamaps(0, 0, splatmap);
    }
}