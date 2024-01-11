using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TerrainGroupOptions : MonoBehaviour
{
    public Terrain[] terrainsArray;
    public AnimationCurve curve;

    [Header("Smoothing")]
    public int iterations = 1;
    public float weightFactor = 1.5f;

    [Header("World Mask")]
    //public TerrainGridBoundary terrainBoundary = TerrainGridBoundary.fallOff;
    public Vector3 worldCenter = new Vector3(0, 0, 0);
    public int worldRadius = 100;
    public int fallOffDistance = 50;

    private void OnEnable()
    {
        terrainsArray = GetComponentsInChildren<Terrain>();
    }

    [ContextMenu("Stitch Terrains")]
    private void ApplyStitches()
    {
        int gridSize = Mathf.RoundToInt(Mathf.Sqrt(terrainsArray.Length));

        Terrain[,] terrainGrid = new Terrain[gridSize, gridSize];

        // Convert 1D array to 2D grid
        for (int i = 0; i < terrainsArray.Length; i++)
        {
            int x = i % gridSize;
            int z = i / gridSize;
            terrainGrid[x, z] = terrainsArray[i];
        }

        // Requires two iterations to make stitches seamless
        for (int w = 0; w < 2; w++)
        {
            // Stitch terrains together
            for (int x = 0; x < gridSize; x++)
            {
                for (int z = 0; z < gridSize; z++)
                {
                    Terrain currentTerrain = terrainGrid[x, z];

                    if (x > 0) ApplyStitchesHorizontal(terrainGrid[x - 1, z].terrainData, currentTerrain.terrainData);
                    if (z > 0) ApplyStitchesVertical(terrainGrid[x, z - 1].terrainData, currentTerrain.terrainData);
                }
            }
        }

        // Stitch terrains together
        for (int x = 0; x < gridSize; x++)
            for (int z = 0; z < gridSize; z++)
            {
                AverageDifferenceFromEdgeEachSection(terrainGrid[x, z].terrainData);
            }
    }
    private void ApplyStitchesHorizontal(TerrainData left, TerrainData right)
    {
        int heightmapResolution = left.heightmapResolution;
        float[,] leftHeights = left.GetHeights(0, 0, heightmapResolution, heightmapResolution);
        float[,] rightHeights = right.GetHeights(0, 0, heightmapResolution, heightmapResolution);

        // Iterating through rows
        for (int i = 0; i < heightmapResolution; i++)
        {
            float avgHeight = (leftHeights[heightmapResolution - 1, i] + rightHeights[0, i]) / 2.0f;
            // Set connection points of tiles to be equal
            leftHeights[heightmapResolution - 1, i] = avgHeight;
            rightHeights[0, i] = avgHeight;
        }

        left.SetHeights(0, 0, leftHeights);
        right.SetHeights(0, 0, rightHeights);
    }
    private void ApplyStitchesVertical(TerrainData bottom, TerrainData top)
    {
        int heightmapResolution = bottom.heightmapResolution;
        float[,] bottomHeights = bottom.GetHeights(0, 0, heightmapResolution, heightmapResolution);
        float[,] topHeights = top.GetHeights(0, 0, heightmapResolution, heightmapResolution);

        // Iterating through columns
        for (int i = 0; i < heightmapResolution; i++)
        {
            float avgHeight = (bottomHeights[i, heightmapResolution - 1] + topHeights[i, 0]) / 2.0f;
            // Set connection points of tiles to be equal
            bottomHeights[i, heightmapResolution - 1] = avgHeight;
            topHeights[i, 0] = avgHeight;
        }

        bottom.SetHeights(0, 0, bottomHeights);
        top.SetHeights(0, 0, topHeights);
    }
    private void AverageDifferenceFromEdgeEachSection(TerrainData terrainData)
    {
        // Billinear Interpolation
        // Split into 2x2 grid
        //
        //     _______________________<============== heightmapEdge = (terrainData.heightmapResolution - 1)
        //    |           |           |
        //    |           |           |
        //    |           |           |
        //    |           |           |
        // (Y)|-----------|-----------|<============= heightmapCenter = (heightmapEdge / 2)
        //    |           |           |
        //    |           |           |
        //    |           |           |
        //   0|___________|___________|<============= 0
        //    0          (X)

        int heightmapEdge = terrainData.heightmapResolution - 1;
        int heightmapCenter = heightmapEdge / 2;

        // 2D array to used to restore original values
        float[,] heights = terrainData.GetHeights(0, 0, terrainData.heightmapResolution, terrainData.heightmapResolution);

        float[] topHeightDifference = new float[terrainData.heightmapResolution];
        float[] rightHeightDifference = new float[terrainData.heightmapResolution];
        float[] bottomHeightDifference = new float[terrainData.heightmapResolution];
        float[] leftHeightDifference = new float[terrainData.heightmapResolution];

        // Calculate height difference from stitching edges
        for (int w = 0; w <= heightmapEdge; w++)
        {
            bottomHeightDifference[w] = heights[w, 0] - (float)heights[w, 1];
            topHeightDifference[w] = heights[w, heightmapEdge] - (float)heights[w, heightmapEdge - 1];

            leftHeightDifference[w] = heights[0, w] - (float)heights[1, w];
            rightHeightDifference[w] = heights[heightmapEdge, w] - (float)heights[heightmapEdge - 1, w];
        }

        // Alter heightmap data while ignoring edges
        for (int x = 1; x < heightmapEdge; x++)
        {
            for (int y = 1; y < heightmapEdge; y++)
            {
                // Quadrant 1
                if (x >= heightmapCenter && y >= heightmapCenter)
                {
                    float topWeight = 1 - ((heightmapEdge - (float)y) / heightmapCenter);
                    float rightWeight = 1 - ((heightmapEdge - (float)x) / heightmapCenter);

                    int totalDistance = (heightmapEdge - x) + (heightmapEdge - y);
                    float topDistanceWeight = 1 - ((heightmapEdge - (float)y) / totalDistance);
                    float rightDistanceWeight = 1 - (float)topDistanceWeight;

                    //topWeight *= topWeight;
                    //topWeight *= topWeight;
                    //rightWeight *= rightWeight;
                    //rightWeight *= rightWeight;

                    topWeight = curve.Evaluate(topWeight);
                    rightWeight = curve.Evaluate(rightWeight);

                    float newHeight = heights[x, y] + ((topHeightDifference[x] * (float)topWeight * topDistanceWeight) + (rightHeightDifference[y] * (float)rightWeight * rightDistanceWeight));
                    heights[x, y] = newHeight;
                }
                // Quadrant 2
                if (x < heightmapCenter && y > heightmapCenter)
                {
                    float topWeight = 1 - ((heightmapEdge - (float)y) / heightmapCenter);
                    float leftWeight = 1 - ((float)x / heightmapCenter);

                    int totalDistance = x + (heightmapEdge - y);
                    float topDistanceWeight = 1 - ((heightmapEdge - (float)y) / totalDistance);
                    float leftDistanceWeight = 1 - (float)topDistanceWeight;

                    //topWeight *= topWeight;
                    //topWeight *= topWeight;
                    //leftWeight *= leftWeight;
                    //leftWeight *= leftWeight;

                    topWeight = curve.Evaluate(topWeight);
                    leftWeight = curve.Evaluate(leftWeight);

                    float newHeight = heights[x, y] + ((topHeightDifference[x] * (float)topWeight * topDistanceWeight) + (leftHeightDifference[y] * (float)leftWeight * leftDistanceWeight));
                    heights[x, y] = newHeight;
                }
                // Quadrant 3
                if (x <= heightmapCenter && y <= heightmapCenter)
                {
                    float bottomWeight = 1 - ((float)y / heightmapCenter);
                    float leftWeight = 1 - ((float)x / heightmapCenter);

                    int totalDistance = x + y;
                    float bottomDistanceWeight = 1 - ((float)y / totalDistance);
                    float leftDistanceWeight = 1 - (float)bottomDistanceWeight;

                    //bottomWeight *= bottomWeight;
                    //bottomWeight *= bottomWeight;
                    //leftWeight *= leftWeight;
                    //leftWeight *= leftWeight;

                    bottomWeight = curve.Evaluate(bottomWeight);
                    leftWeight = curve.Evaluate(leftWeight);

                    float newHeight = heights[x, y] + ((bottomHeightDifference[x] * (float)bottomWeight * bottomDistanceWeight) + (leftHeightDifference[y] * (float)leftWeight * leftDistanceWeight));
                    heights[x, y] = newHeight;
                }
                // Quadrant 4
                if (x > heightmapCenter && y < heightmapCenter)
                {
                    float bottomWeight = 1 - ((float)y / heightmapCenter);
                    float rightWeight = 1 - ((heightmapEdge - (float)x) / heightmapCenter);

                    int totalDistance = (heightmapEdge - x) + y;
                    float bottomDistanceWeight = 1 - ((float)y / totalDistance);
                    float rightDistanceWeight = 1 - (float)bottomDistanceWeight;

                    //bottomWeight *= bottomWeight;
                    //bottomWeight *= bottomWeight;
                    //rightWeight *= rightWeight;
                    //rightWeight *= rightWeight;

                    bottomWeight = curve.Evaluate(bottomWeight);
                    rightWeight = curve.Evaluate(rightWeight);

                    float newHeight = heights[x, y] + ((bottomHeightDifference[x] * (float)bottomWeight * bottomDistanceWeight) + (rightHeightDifference[y] * (float)rightWeight * rightDistanceWeight));
                    heights[x, y] = newHeight;
                }
            }
        }

        // Apply heightmap data to tile
        terrainData.SetHeights(0, 0, heights);
    }

    [ContextMenu("Smooth TerrainGrid")]
    private void StitchTerrainGrid()
    {
        float[,] allHeightmaps = StoreHeightmaps();
        LoadHeightmaps(allHeightmaps);
    }
    public Terrain[,] Convert1DArrayTo2DGrid(Terrain[] array1D)
    {
        int gridSize = Mathf.RoundToInt(Mathf.Sqrt(array1D.Length));

        Terrain[,] grid2D = new Terrain[gridSize, gridSize];

        // Convert 1D array to 2D grid
        for (int i = 0; i < array1D.Length; i++)
        {
            int x = i % gridSize;
            int z = i / gridSize;
            grid2D[x, z] = array1D[i];
        }

        return grid2D;
    }
    public float[,] StoreHeightmaps()
    {
        Terrain[,] grid = Convert1DArrayTo2DGrid(terrainsArray);

        // Get Rows: 'GetLength(0)' = rows // 'GetLength(1)' = columns
        int gridSize = grid.GetLength(0);
        int overlappingAreas = gridSize - 1;
        int heightmapResolution = grid[0, 0].terrainData.heightmapResolution;

        // Instantiate one big heightmap for all terrains and populate it with heightmap data, removing overlapping edges of terrain tiles
        int combinedHeightmapResolution = ((gridSize * heightmapResolution) - overlappingAreas);
        float[,] allHeightmaps = new float[combinedHeightmapResolution, combinedHeightmapResolution];

        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                float[,] heightmap = grid[i, j].terrainData.GetHeights(0, 0, heightmapResolution, heightmapResolution);

                if (i == 0 && j == 0)
                {
                    for (int x = 0; x < heightmapResolution; x++)
                    {
                        for (int y = 0; y < heightmapResolution; y++)
                        {
                            allHeightmaps[i * heightmapResolution + x, j * heightmapResolution + y] = heightmap[x, y];
                        }
                    }
                }

                else if (i > 0 && j == 0)
                {
                    for (int x = 1; x < heightmapResolution; x++)
                    {
                        for (int y = 0; y < heightmapResolution; y++)
                        {
                            allHeightmaps[i * heightmapResolution + x - i, j * heightmapResolution + y] = heightmap[x, y];
                        }
                    }
                }

                else if (i == 0 && j > 0)
                {
                    for (int x = 0; x < heightmapResolution; x++)
                    {
                        for (int y = 1; y < heightmapResolution; y++)
                        {
                            allHeightmaps[i * heightmapResolution + x, j * heightmapResolution + y - j] = heightmap[x, y];
                        }
                    }
                }

                else if (i > 0 && j > 0)
                {
                    for (int x = 1; x < heightmapResolution; x++)
                    {
                        for (int y = 1; y < heightmapResolution; y++)
                        {
                            allHeightmaps[i * heightmapResolution + x - i, j * heightmapResolution + y - j] = heightmap[x, y];
                        }
                    }
                }
            }
        }

        int transitionZone = 20;
        // Weighted Average Smoothing
        for (int iteration = 0; iteration < iterations; iteration++)
        {
            float[,] thisIterationSmoothedHeights = allHeightmaps;

            // Horizontal
            for (int x = 0; x < combinedHeightmapResolution; x++)
            {
                for (int y = 0; y < combinedHeightmapResolution; y += (heightmapResolution - 1))
                {
                    // Apply to smoothing to transition zone near edges
                    for (int z = -transitionZone; z <= transitionZone; z++)
                    {
                        // Weighted Average Smoothing - Kernel
                        int count = 0;
                        float sum = 0;
                        for (int i = -1; i <= 1; i++)
                        {
                            for (int j = -1; j <= 1; j++)
                            {
                                int newX = x + i + z;
                                int newY = y + j + z;

                                if (newX >= 0 && newX < combinedHeightmapResolution && newY >= 0 && newY < combinedHeightmapResolution)
                                {
                                    count++;
                                    sum += thisIterationSmoothedHeights[newX, newY];
                                }
                            }
                        }

                        if (x + z >= 0 && x + z < combinedHeightmapResolution && y + z >= 0 && y + z < combinedHeightmapResolution)
                        {
                            thisIterationSmoothedHeights[x + z, y + z] = sum / count;
                        }
                        //
                    }
                }
            }

            // Vertical
            for (int x = 0; x < combinedHeightmapResolution; x += (heightmapResolution - 1))
            {
                for (int y = 0; y < combinedHeightmapResolution; y++)
                {
                    // Apply to smoothing to transition zone near edges
                    for (int z = -transitionZone; z <= transitionZone; z++)
                    {
                        // Weighted Average Smoothing - Kernel
                        int count = 0;
                        float sum = 0;
                        for (int i = -1; i <= 1; i++)
                        {
                            for (int j = -1; j <= 1; j++)
                            {
                                int newX = x + i + z;
                                int newY = y + j + z;

                                if (newX >= 0 && newX < combinedHeightmapResolution && newY >= 0 && newY < combinedHeightmapResolution)
                                {
                                    count++;
                                    sum += thisIterationSmoothedHeights[newX, newY];
                                }
                            }
                        }

                        if (x + z >= 0 && x + z < combinedHeightmapResolution && y + z >= 0 && y + z < combinedHeightmapResolution)
                        {
                            thisIterationSmoothedHeights[x + z, y + z] = sum / count;
                        }
                        //
                    }
                }
            }

            allHeightmaps = thisIterationSmoothedHeights;
        }
        //

        // Weighted Average Smoothing - Entire Tile
        for (int entireIteration = 0; entireIteration < (iterations / 10); entireIteration++)
        {
            float[,] thisIterationSmoothedHeights = allHeightmaps;

            for (int x = 0; x < combinedHeightmapResolution; x++)
            {
                for (int y = 0; y < combinedHeightmapResolution; y++)
                {
                    float sum = 0;
                    float weightSum = 0;

                    for (int i = -1; i <= 1; i++)
                    {
                        for (int j = -1; j <= 1; j++)
                        {
                            int newX = x + i;
                            int newY = y + j;

                            if (newX >= 0 && newX < combinedHeightmapResolution && newY >= 0 && newY < combinedHeightmapResolution)
                            {
                                float weight = 1.0f / Mathf.Pow((Mathf.Abs(i) + Mathf.Abs(j) + 1), weightFactor);
                                sum += allHeightmaps[newX, newY] * weight;
                                weightSum += weight;
                            }
                        }
                    }

                    thisIterationSmoothedHeights[x, y] = sum / weightSum;
                }
            }

            allHeightmaps = thisIterationSmoothedHeights;
        }

        return allHeightmaps;
    }
    public Terrain[] Convert2DGridTo1DArray(Terrain[,] grid2D)
    {
        int rows = grid2D.GetLength(0);
        int columns = grid2D.GetLength(1);

        Terrain[] array1D = new Terrain[rows * columns];
        int index = 0;

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                array1D[index] = grid2D[i, j];
                index++;
            }
        }

        return array1D;
    }
    public void LoadHeightmaps(float[,] allHeightmaps)
    {
        Terrain[,] grid = Convert1DArrayTo2DGrid(terrainsArray);

        int gridSize = grid.GetLength(0);
        int heightmapResolution = grid[0, 0].terrainData.heightmapResolution;

        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                float[,] heightmap = new float[heightmapResolution, heightmapResolution];

                if (i == 0 && j == 0)
                {
                    for (int x = 0; x < heightmapResolution; x++)
                    {
                        for (int y = 0; y < heightmapResolution; y++)
                        {
                            heightmap[x, y] = allHeightmaps[i * heightmapResolution + x, j * heightmapResolution + y];
                        }
                    }
                }

                else if (i > 0 && j == 0)
                {
                    for (int x = 0; x < heightmapResolution; x++)
                    {
                        for (int y = 0; y < heightmapResolution; y++)
                        {
                            heightmap[x, y] = allHeightmaps[i * heightmapResolution + x - i, j * heightmapResolution + y];
                        }
                    }
                }

                else if (i == 0 && j > 0)
                {
                    for (int x = 0; x < heightmapResolution; x++)
                    {
                        for (int y = 0; y < heightmapResolution; y++)
                        {
                            heightmap[x, y] = allHeightmaps[i * heightmapResolution + x, j * heightmapResolution + y - j];
                        }
                    }
                }

                else if (i > 0 && j > 0)
                {
                    for (int x = 0; x < heightmapResolution; x++)
                    {
                        for (int y = 0; y < heightmapResolution; y++)
                        {
                            heightmap[x, y] = allHeightmaps[i * heightmapResolution + x - i, j * heightmapResolution + y - j];
                        }
                    }
                }

                grid[i, j].terrainData.SetHeights(0, 0, heightmap);
            }
        }
    }

    /*
    [ContextMenu("Apply Circular Mask")]
    public void ApplyCircularMask()
    {
        Terrain[,] terrainGrid = Convert1DArrayTo2DGrid(terrainsArray);

        foreach (Terrain terrain in terrainGrid)
        {
            TerrainData terrainData = terrain.terrainData;
            int width = terrainData.heightmapResolution;
            int height = terrainData.heightmapResolution;
            float[,] heights = terrainData.GetHeights(0, 0, width, height);
            Vector3 terrainPosition = terrain.transform.position;

            for (int z = 0; z < height; z++)
            {
                for (int x = 0; x < width; x++)
                {
                    Vector2 vertexPosition = new Vector2(x * terrainData.heightmapScale.x + terrainPosition.x, z * terrainData.heightmapScale.z + terrainPosition.z);
                    float distanceFromCenter = Vector2.Distance(vertexPosition, worldCenter);

                    if (distanceFromCenter > worldRadius)
                    {
                        float falloffFactor = (distanceFromCenter - worldRadius) / fallOffDistance;
                        falloffFactor = Mathf.Clamp01(falloffFactor);
                        falloffFactor = curve.Evaluate(falloffFactor);

                        if (terrainBoundary == TerrainGridBoundary.fallOff)
                        {
                            // Apply linear falloff
                            heights[z, x] *= 1 - falloffFactor;
                        }
                        else if (terrainBoundary == TerrainGridBoundary.mountains)
                        {
                            // Apply linear falloff, but increase height instead of reducing it
                            heights[z, x] = Mathf.Lerp(heights[z, x], 1, falloffFactor);
                        }
                    }
                }
            }

            terrainData.SetHeights(0, 0, heights);
        }
    }
    */
    
}
