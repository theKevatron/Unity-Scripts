using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TerrainOverride : MonoBehaviour
{
    //Erosions erosions;
    public bool update = false;

    [Header("Terrain")]
    public Terrain terrain;
    public TerrainData terrainData;
    public float maxHeight = 1.0f;

    [Header("Height Map 1")]
    [Header("Input")]
    public bool generateInitialHeightMap = true;
    public Texture2D initialHeightMapTexture;
    public int initialHeightMapResolution;
    [Range(-1f, 1f)]
    public float initialHeightMapPrevalence = 1f;

    [Header("Height Map 2")]
    [Header("Input")]
    public bool generateSecondaryHeightMap = false;
    public Texture2D secondaryHeightMapTexture;
    public int secondaryHeightMapResolution;
    [Range(-1f, 1f)]
    public float secondaryHeightMapPrevalence = 1f;

    [Header("Generated Noise Map Settings")]
    public int seed = 12345;
    public float scale = 5f;
    public int octaves = 7;
    public float persistence = 0.3f;
    public float lacunarity = 2f;
    public float heightMultiplier = 0.5f;

    /*
    // Terrain Smoothing
    [Header("SMOOTHING")]
    [Header("Laplacian Smoothing")]
    public bool laplacianSmoothing = false;
    public int laplacianIterations = 1;
    [Header("Weighted Average Smoothing")]
    public bool weightedAverageSmoothing = false;
    public int weightedAverageIterations = 1;
    [Range(0.0001f, 5f)]
    public float weightFactor = 1.5f;
    [Header("Gaussian Smoothing")]
    // Sigma = Effect/Strength of neighboring pixels (comparable to a Weight Factor in Weight Average Smoothing)
    // Kernel = Number of neighboring pixels used, must be an odd number to have a center pixel to ensure accuracy
    public bool gaussianSmoothing = false;
    public int gaussianIterations = 1;
    [Range(0.0001f, 10f)]
    public float sigma = 1.0f;
    public enum ValidKernelSizes { _3 = 3, _5 = 5, _7 = 7, _9 = 9, _11 = 11, _13 = 13, _15 = 15, _17 = 17, _19 = 19, _21 = 21, _23 = 23, _25 = 25 }
    public ValidKernelSizes selectedKernelSize = ValidKernelSizes._3;
    */

    void OnEnable()
    {
        //erosions = GetComponent<Erosions>();
        // Reset terrain
        terrain = GetComponent<Terrain>();
        terrainData = terrain.terrainData;
        float[,] resetData = new float[terrainData.heightmapResolution, terrainData.heightmapResolution];
        terrainData.SetHeights(0, 0, resetData);

        // Initialize height map data containers
        float[,] initialHeightMapData = new float[0, 0];
        float[,] secondaryHeightMapData = new float[0, 0];

        // Generate Height Maps for those selected
        if (generateInitialHeightMap == true)
        {
            initialHeightMapResolution = terrainData.heightmapResolution;
            initialHeightMapData = GeneratePerlinNoiseHeightMapData(initialHeightMapResolution, persistence, lacunarity);
        }
        if (generateSecondaryHeightMap == true)
        {
            secondaryHeightMapResolution = terrainData.heightmapResolution;
            secondaryHeightMapData = GeneratePerlinNoiseHeightMapData(secondaryHeightMapResolution, persistence, lacunarity);
        }
        // Prioritize heightmaps that are assigned
        if (initialHeightMapTexture != null)
        {
            generateInitialHeightMap = false;
            initialHeightMapResolution = initialHeightMapTexture.width + 1;
            initialHeightMapData = GetTextureHeightMapData(initialHeightMapTexture);
        }
        if (secondaryHeightMapTexture != null)
        {
            generateSecondaryHeightMap = false;
            secondaryHeightMapResolution = secondaryHeightMapTexture.width + 1;
            secondaryHeightMapData = GetTextureHeightMapData(secondaryHeightMapTexture);
        }

        // Assign heightmap data to terrainData
        if (initialHeightMapData.Length <= 0 && secondaryHeightMapData.Length <= 0)
            Debug.Log("No Height Map Data: No Height Maps Assigned or Generated");
        else if (initialHeightMapData.Length > 0 && secondaryHeightMapData.Length <= 0)
        {
            //initialHeightMapData = ApplySmoothingToData(initialHeightMapData);
            terrainData.SetHeights(0, 0, initialHeightMapData);
        }
        else if (initialHeightMapData.Length <= 0 && secondaryHeightMapData.Length > 0)
        {
           // secondaryHeightMapData = ApplySmoothingToData(secondaryHeightMapData);
            terrainData.SetHeights(0, 0, secondaryHeightMapData);
        }
        else if (initialHeightMapData.Length > 0 && secondaryHeightMapData.Length > 0)
        {
            float[,] combinedHeightMapData = CombineHeightMapData(initialHeightMapData, secondaryHeightMapData);
            //combinedHeightMapData = ApplySmoothingToData(combinedHeightMapData);
            terrainData.SetHeights(0, 0, combinedHeightMapData);
        }
    }
    private void Update()
    {
        if (update == true)
        {
            //erosions = GetComponent<Erosions>();
            // Reset terrain
            terrain = GetComponent<Terrain>();
            terrainData = terrain.terrainData;
            float[,] resetData = new float[terrainData.heightmapResolution, terrainData.heightmapResolution];
            terrainData.SetHeights(0, 0, resetData);

            // Initialize height map data containers
            float[,] initialHeightMapData = new float[0, 0];
            float[,] secondaryHeightMapData = new float[0, 0];

            // Generate Height Maps for those selected
            if (generateInitialHeightMap == true)
            {
                initialHeightMapResolution = terrainData.heightmapResolution;
                initialHeightMapData = GeneratePerlinNoiseHeightMapData(initialHeightMapResolution, persistence, lacunarity);
            }
            if (generateSecondaryHeightMap == true)
            {
                secondaryHeightMapResolution = terrainData.heightmapResolution;
                secondaryHeightMapData = GeneratePerlinNoiseHeightMapData(secondaryHeightMapResolution, persistence, lacunarity);
            }
            // Prioritize heightmaps that are assigned
            if (initialHeightMapTexture != null)
            {
                generateInitialHeightMap = false;
                initialHeightMapResolution = initialHeightMapTexture.width + 1;
                initialHeightMapData = GetTextureHeightMapData(initialHeightMapTexture);
            }
            if (secondaryHeightMapTexture != null)
            {
                generateSecondaryHeightMap = false;
                secondaryHeightMapResolution = secondaryHeightMapTexture.width + 1;
                secondaryHeightMapData = GetTextureHeightMapData(secondaryHeightMapTexture);
            }

            // Assign heightmap data to terrainData
            if (initialHeightMapData.Length <= 0 && secondaryHeightMapData.Length <= 0)
                Debug.Log("No Height Map Data: No Height Maps Assigned or Generated");
            else if (initialHeightMapData.Length > 0 && secondaryHeightMapData.Length <= 0)
            {
                //initialHeightMapData = ApplySmoothingToData(initialHeightMapData);
                terrainData.SetHeights(0, 0, initialHeightMapData);
            }
            else if (initialHeightMapData.Length <= 0 && secondaryHeightMapData.Length > 0)
            {
                //secondaryHeightMapData = ApplySmoothingToData(secondaryHeightMapData);
                terrainData.SetHeights(0, 0, secondaryHeightMapData);
            }
            else if (initialHeightMapData.Length > 0 && secondaryHeightMapData.Length > 0)
            {
                float[,] combinedHeightMapData = CombineHeightMapData(initialHeightMapData, secondaryHeightMapData);
                //combinedHeightMapData = ApplySmoothingToData(combinedHeightMapData);
                terrainData.SetHeights(0, 0, combinedHeightMapData);
            }
        }
    }
    // Perlin noise generator function, values are saved in "generatedPerlinNoiseMap" variable(2D array)
    public float[,] GeneratePerlinNoiseHeightMapData(int heightMapResolution, float persistence, float lacunarity)
    {
        int width = heightMapResolution;
        int height = heightMapResolution;
        // Instantiate heightmap with 1 pixel padding for Interpolation, Mipmapping, Tessellation, Sampling, Bleeding etc.
        float [,] generatedPerlinNoiseHeightMapData = new float[width, height];

        // Loop through each point in the heightmap and calculate its height value
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float amplitude = 1f;
                float frequency = 1f;
                float heightValue = 0f;

                for (int o = 0; o < octaves; o++)
                {
                    float sampleX = (float)x / width * scale * frequency;
                    float sampleY = (float)y / height * scale * frequency;
                    heightValue += Mathf.PerlinNoise(sampleX + seed, sampleY + seed) * amplitude * heightMultiplier;
                    amplitude *= persistence;
                    frequency *= lacunarity;
                }

                // Store the calculated height value in the heightmap array
                generatedPerlinNoiseHeightMapData[x, y] = heightValue;
            }
        }

        return generatedPerlinNoiseHeightMapData;
    }
    public float[,] GetTextureHeightMapData(Texture2D heightMapTexture)
    {
        int width = heightMapTexture.width + 1;
        int height = heightMapTexture.height + 1;
        
        // Resize 256x256 to 257x257 for terrain heightmap
        // Step 1: Create a new RenderTexture with the desired size
        RenderTexture rt = new RenderTexture(width, height, 0);
        // Step 2: Set the filterMode and wrapMode for both the original texture and the RenderTexture
        heightMapTexture.filterMode = FilterMode.Bilinear;
        heightMapTexture.wrapMode = TextureWrapMode.Clamp;
        rt.filterMode = FilterMode.Bilinear;
        rt.wrapMode = TextureWrapMode.Clamp;
        // Step 3: Render the original texture into the RenderTexture using Graphics.Blit()
        Graphics.Blit(heightMapTexture, rt);
        // Step 4: Create a new Texture2D with the same size as the RenderTexture and read the pixel data from it
        Texture2D resizedTexture = new Texture2D(width, height);
        RenderTexture.active = rt;
        resizedTexture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        resizedTexture.Apply();
        RenderTexture.active = null; // Reset the active RenderTexture
        rt.Release(); // Release the RenderTexture resources

        // Get the pixel data from the height map texture (Height Maps with dimensions of (2^n + 1) ---> Examples: 257x257, 513x513)
        Color[] pixels = resizedTexture.GetPixels();

        // Declare 2D array for texture data
        float[,] textureHeightMapData = new float[width, height];

        // Nested loop to access each vertex and store height map data
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float heightValue = pixels[x + y * width].grayscale * maxHeight;
                textureHeightMapData[x, y] = heightValue;
            }
        }

        return textureHeightMapData;
    }
    public float[,] CombineHeightMapData(float [,] initialHeightMapData, float [,] secondaryHeightMapData)
    {
        int width = initialHeightMapResolution;
        int height = initialHeightMapResolution;

        float[,] combinedHeightMapData = new float[width, height];
        // Combine the two height maps by adding their height values
        // Nested loop to access each vertex
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float heightValue1 = initialHeightMapData[x, y];
                float heightValue2 = secondaryHeightMapData[x, y];

                // Algorithm applied to each vertex:
                // At each vertex, the value of the output terrain is equal to the value of the first input terrain multiplied
                // by the first coefficient(heightMapPrevelance1), plus the value of the second input terrain multiplied by the second coefficient(heightMapPrevelance2).
                float combinedHeight = (heightValue1 * initialHeightMapPrevalence) + (heightValue2 * secondaryHeightMapPrevalence);
                combinedHeightMapData[x, y] = combinedHeight;
            }
        }

        return combinedHeightMapData;
    }

    /*
    // Smoothing Check
    public float[,] ApplySmoothingToData(float[,] inputHeightMapData)
    {
        float[,] smoothedHeightMapData = inputHeightMapData;

        if (laplacianSmoothing == true)
        {
            smoothedHeightMapData = LaplacianSmoothData(inputHeightMapData, laplacianIterations);
        }

        if (weightedAverageSmoothing == true)
        {
            smoothedHeightMapData = WeightedAverageSmoothData(inputHeightMapData, weightedAverageIterations, weightFactor);
        }

        if (gaussianSmoothing == true)
        {
            smoothedHeightMapData = GaussianSmoothData(inputHeightMapData, sigma, ((int)selectedKernelSize));
        }

        return smoothedHeightMapData;
    }
    // Smoothing Algorithms
    private float[,] LaplacianSmoothData(float[,] inputHeightMapData, int laplacianIterations)
    {
        int width = terrainData.heightmapResolution;
        int height = terrainData.heightmapResolution;
        float[,] finalSmoothedHeights = inputHeightMapData;

        for (int iteration = 0; iteration < laplacianIterations; iteration++)
        {
            float[,] thisIterationSmoothedHeights = new float[width, height];

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    float sum = 0;
                    int count = 0;

                    for (int x = -1; x <= 1; x++)
                    {
                        for (int y = -1; y <= 1; y++)
                        {
                            int xi = i + x;
                            int yj = j + y;

                            if (xi >= 0 && xi < width && yj >= 0 && yj < height)
                            {
                                sum += finalSmoothedHeights[xi, yj];
                                count++;
                            }
                        }
                    }

                    thisIterationSmoothedHeights[i, j] = sum / count;
                }
            }

            finalSmoothedHeights = thisIterationSmoothedHeights;
        }

        return finalSmoothedHeights;
    }
    private float[,] WeightedAverageSmoothData(float[,] inputHeightMapData, int weightedAverageIterations, float weightFactor)
    {
        int width = terrainData.heightmapResolution;
        int height = terrainData.heightmapResolution;
        float[,] finalSmoothedHeights = inputHeightMapData;

        for (int iteration = 0; iteration < weightedAverageIterations; iteration++)
        {
            float[,] thisIterationSmoothedHeights = new float[width, height];

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    float sum = 0;
                    float weightSum = 0;

                    for (int x = -1; x <= 1; x++)
                    {
                        for (int y = -1; y <= 1; y++)
                        {
                            int xi = i + x;
                            int yj = j + y;

                            if (xi >= 0 && xi < width && yj >= 0 && yj < height)
                            {
                                float weight = 1.0f / Mathf.Pow((Mathf.Abs(x) + Mathf.Abs(y) + 1), weightFactor);
                                sum += finalSmoothedHeights[xi, yj] * weight;
                                weightSum += weight;
                            }
                        }
                    }

                    thisIterationSmoothedHeights[i, j] = sum / weightSum;
                }
            }

            finalSmoothedHeights = thisIterationSmoothedHeights;
        }

        return finalSmoothedHeights;

        //terrainData.SetHeights(0, 0, finalSmoothedHeights);
    }
    private float[,] GaussianSmoothData(float[,] inputHeightMapData, float sigma, int kernelSize)
    {
        int width = terrainData.heightmapResolution;
        int height = terrainData.heightmapResolution;
        float[,] heights = inputHeightMapData;

        float[,] kernel = GenerateGaussianKernel(sigma, kernelSize);

        float[,] smoothedHeights = new float[width, height];

        int offset = kernelSize / 2;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                float sum = 0;
                float weightSum = 0;

                for (int x = -offset; x <= offset; x++)
                {
                    for (int y = -offset; y <= offset; y++)
                    {
                        int xi = i + x;
                        int yj = j + y;

                        if (xi >= 0 && xi < width && yj >= 0 && yj < height)
                        {
                            float weight = kernel[x + offset, y + offset];
                            sum += heights[xi, yj] * weight;
                            weightSum += weight;
                        }
                    }
                }

                smoothedHeights[i, j] = sum / weightSum;
            }
        }

        return smoothedHeights;
        //terrainData.SetHeights(0, 0, smoothedHeights);
    }
    private float[,] GenerateGaussianKernel(float sigma, int kernelSize)
    {
        float[,] kernel = new float[kernelSize, kernelSize];
        float sum = 0;
        int offset = kernelSize / 2;
        float twoSigmaSquared = 2 * sigma * sigma;

        for (int x = -offset; x <= offset; x++)
        {
            for (int y = -offset; y <= offset; y++)
            {
                float value = Mathf.Exp(-(x * x + y * y) / twoSigmaSquared) / (Mathf.PI * twoSigmaSquared);
                kernel[x + offset, y + offset] = value;
                sum += value;
            }
        }

        for (int i = 0; i < kernelSize; i++)
        {
            for (int j = 0; j < kernelSize; j++)
            {
                kernel[i, j] /= sum;
            }
        }

        return kernel;
    }
    */

}
