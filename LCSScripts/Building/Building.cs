using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[ExecuteInEditMode]
public class Building : MonoBehaviour
{
    [Header("Savable Data")]
    [SerializeField]
    public int currentActivatableIndex = 0;
    [SerializeField]
    public int lastIndex;

    public List<BuildingActivatable> buildingActivatables;
    public BuildingActivatable currentActivatable;
    public MeshRenderer finishedBuilding;

    [Header("Effects")]
    public EasyFX easyFX;
    public PFX PFX;
    public SFX SFX;

    private void OnEnable()
    {
        Reset();
        LoadCompleted();
        easyFX = FindObjectOfType<EasyFX>();
        PFX = PFX.Dirt;
        SFX = SFX.DirtHit;
    }

    private void Reset()
    {
        lastIndex = 0;
        buildingActivatables?.Clear();
        currentActivatable = null;
        if (finishedBuilding != null)
            finishedBuilding.enabled = false;
    }

    private void GetBuildingGroups()
    {
        buildingActivatables = new List<BuildingActivatable>();
        GetComponentsInChildren<BuildingActivatable>(true, buildingActivatables);
        lastIndex = buildingActivatables.Count - 1;
        // Reset all building activatables
        foreach (BuildingActivatable buildingActivatable in buildingActivatables)
        {
            buildingActivatable.gameObject.SetActive(true);
            buildingActivatable.InCompleteBuildingActivatable();
        }
    }
    private void CompleteBuilding()
    {
        foreach (BuildingActivatable buildingActivatable in buildingActivatables)
        {
            buildingActivatable.CompleteBuildingActivatable();
            buildingActivatable.gameObject.SetActive(false);
        }
        if (finishedBuilding != null)
            finishedBuilding.enabled = true;
    }
    public void LoadCompleted()
    {
        GetBuildingGroups();

        // Check if building completed
        if (buildingActivatables[lastIndex].isComplete || currentActivatableIndex > lastIndex)
        {
            CompleteBuilding();
            return;
        }
        // If building not completed
        else if (buildingActivatables[lastIndex].isComplete == false || currentActivatableIndex <= lastIndex)
        {
            // Activate saved progress(building not completed)
            for (int i = 0; i <= currentActivatableIndex; i++)
            {
                if (i < currentActivatableIndex)
                    buildingActivatables[i].CompleteBuildingActivatable();
                else if (i == currentActivatableIndex)
                    if (buildingActivatables[i].colliders != null)
                    {
                        foreach (Collider collider in buildingActivatables[i].colliders)
                        {
                            collider.enabled = true;
                        }
                    }
            }
            currentActivatable = buildingActivatables[currentActivatableIndex];
        }

    }

    public void UpdateBuildingProgress()
    {
        if (currentActivatableIndex < lastIndex)
        {
            easyFX.PlayPFX((int)PFX, transform.position, transform.rotation, 1);
            easyFX.PlaySFX((int)SFX, transform.position, 1);
            currentActivatableIndex++;
            currentActivatable = buildingActivatables[currentActivatableIndex];
            if (buildingActivatables[currentActivatableIndex].colliders != null)
            {
                foreach (Collider collider in buildingActivatables[currentActivatableIndex].colliders)
                {
                    collider.enabled = true;
                }
            }
        }
        else if (currentActivatableIndex == lastIndex)
        {
            currentActivatableIndex++;
            currentActivatable = null;
        }

        // Check if building completed
        if (buildingActivatables[lastIndex].isComplete || currentActivatableIndex > lastIndex)
        {
            CompleteBuilding();
            return;
        }
    }

}
