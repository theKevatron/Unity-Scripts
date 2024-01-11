using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[ExecuteInEditMode]
public class BuildingActivatable : MonoBehaviour
{
    public string[] definitions;
    private Building building;
    public HintMaterials hintMaterials;
    public Collider[] colliders;

    public bool isComplete;
    public BuildingItem inspector;

    [Header("Rock Traits")]
    public bool rockSmall;
    public bool rockLarge;

    [Header("Log Traits")]
    public Status status;
    public Quality quality;
    public Style style;
    public Type type;

    void OnEnable()
    {
        Define();
    }
    public void Reset()
    {
        definitions = this.gameObject.name.Split(char.Parse("_"));
        building = GetComponentInParent<Building>();
        hintMaterials = GetComponent<HintMaterials>();
        colliders = GetComponents<Collider>();
        inspector = null;
        rockSmall = false;
        rockLarge = false;
        quality = Quality.Undefined;
        style = Style.Undefined;
        type = Type.Undefined;
    }
    public void Define()
    {
        Reset();
        foreach (string definition in definitions)
        {
            if (definition == "RockSmall")
                rockSmall = true;
            else if (definition == "RockLarge")
                rockLarge = true;
            else if (definition == "Pulp")
                quality = Quality.Pulp;
            else if (definition == "Saw")
                quality = Quality.Saw;
            else if (definition == "Veneer")
                quality = Quality.Veneer;
            else if (definition == "Square")
                style = Style.Square;
            else if (definition == "D")
                style = Style.D;
            else if (definition == "Round")
                style = Style.Round;
            if (definition == "Sill")
                type = Type.Sill;
            else if (definition == "Wall")
                type = Type.Wall;
            else if (definition == "Top")
                type = Type.Top;
            else if (definition == "Gable")
                type = Type.Gable;
            else if (definition == "Ridge")
                type = Type.Ridge;
            else if (definition == "Board")
                type = Type.Board;
        }
    }

    // Used by Building to Load Progress and by this to finish activatable
    public void CompleteBuildingActivatable()
    {
        if (inspector != null)
            Destroy(inspector.gameObject);
        hintMaterials?.ActivateMaterialsOriginal();
        if (colliders != null)
        {
            foreach (Collider collider in colliders)
            {
                collider.enabled = true;
                collider.isTrigger = false;
            }
        }
        isComplete = true;
    }
    public void InCompleteBuildingActivatable()
    {
        hintMaterials?.DeactivateMesh();
        if (colliders != null)
        {
            foreach (Collider collider in colliders)
            {
                collider.enabled = false;
                collider.isTrigger = true;
            }
        }
        isComplete = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isComplete == false)
        {
            if (other.gameObject.layer == 8)
            {
                if (other.CompareTag("PlayerRangeTrigger"))
                {
                    hintMaterials?.ActivateMaterialsHintCorrect();
                }
            }
            // If the objects layer is "Grabbable" (layer 20)
            else if (other.gameObject.layer == 20)
            {
                if (other.CompareTag("BuildingItem"))
                {
                    //then get the "buildingItem" component
                    inspector = other.GetComponent<BuildingItem>();
                    CheckBuildingItem();
                }
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (isComplete == false)
        {
            if (other.gameObject.layer == 8)
            {
                if (other.CompareTag("PlayerRangeTrigger"))
                {
                    hintMaterials?.DeactivateMesh();
                }
            }
            // If the objects layer is "Grabbable" (layer 20)
            else if (other.gameObject.layer == 20)
            {
                if (other.CompareTag("BuildingItem"))
                {
                    hintMaterials?.ActivateMaterialsHintCorrect();
                }
            }
        }
    }

    public void CheckBuildingItem()
    {
        if (rockSmall == true && inspector?.rockSmall == true)
        {
            CompleteBuildingActivatable();
            building?.UpdateBuildingProgress();
        }
        else if (rockLarge == true && inspector?.rockLarge == true)
        {
            CompleteBuildingActivatable();
            building?.UpdateBuildingProgress();
        }
        else if (inspector.finished == true)
        {
            CheckLogQuality();
        }
        else
        {
            hintMaterials?.ActivateMaterialsHintIncorrect();
        }
    }
    public void CheckLogQuality()
    {
        if (quality == Quality.Pulp)
        {
            if (inspector.quality == Quality.Pulp)
            {
                CheckLogStyle();
            }
            else
            {
                hintMaterials?.ActivateMaterialsHintIncorrect();
            }
        }
        else if (quality == Quality.Saw)
        {
            if (inspector.quality == Quality.Saw)
                CheckLogStyle();
            else
                hintMaterials?.ActivateMaterialsHintIncorrect();
        }
        else if (quality == Quality.Veneer)
        {
            if (inspector.quality == Quality.Veneer)
                CheckLogStyle();
            else
                hintMaterials?.ActivateMaterialsHintIncorrect();
        }
        else
        {
            hintMaterials?.ActivateMaterialsHintIncorrect();
        }
    }
    public void CheckLogStyle()
    {
        if (style == Style.Square)
        {
            if (inspector.style == Style.Square)
                CheckLogType();
            else
                hintMaterials?.ActivateMaterialsHintIncorrect();
        }
        else if (style == Style.D)
        {
            if (inspector.style == Style.D)
                CheckLogType();
            else
                hintMaterials?.ActivateMaterialsHintIncorrect();
        }
        else if (style == Style.Round)
        {
            if (inspector.style == Style.Round)
                CheckLogType();
            else
                hintMaterials?.ActivateMaterialsHintIncorrect();
        }
        else
        {
            hintMaterials?.ActivateMaterialsHintIncorrect();
        }
    }
    public void CheckLogType()
    {
        if (type == Type.Sill)
        {
            if (inspector.type == Type.Sill)
            {
                CompleteBuildingActivatable();
                building?.UpdateBuildingProgress();
            }
            else
                hintMaterials?.ActivateMaterialsHintIncorrect();
        }
        else if (type == Type.Wall)
        {
            if (inspector.type == Type.Wall)
            {
                CompleteBuildingActivatable();
                building?.UpdateBuildingProgress();
            }
            else
                hintMaterials?.ActivateMaterialsHintIncorrect();
        }
        else if (type == Type.Top)
        {
            if (inspector.type == Type.Top)
            {
                CompleteBuildingActivatable();
                building?.UpdateBuildingProgress();
            }
            else
                hintMaterials?.ActivateMaterialsHintIncorrect();
        }
        else if (type == Type.Gable)
        {
            if (inspector.type == Type.Gable)
            {
                CompleteBuildingActivatable();
                building?.UpdateBuildingProgress();
            }
            else
                hintMaterials?.ActivateMaterialsHintIncorrect();
        }
        else if (type == Type.Ridge)
        {
            if (inspector.type == Type.Ridge)
            {
                CompleteBuildingActivatable();
                building?.UpdateBuildingProgress();
            }
            else
                hintMaterials?.ActivateMaterialsHintIncorrect();
        }
        else if (type == Type.Board)
        {
            if (inspector.type == Type.Board)
            {
                CompleteBuildingActivatable();
                building?.UpdateBuildingProgress();
            }
            else
                hintMaterials?.ActivateMaterialsHintIncorrect();
        }
        else
        {
            hintMaterials?.ActivateMaterialsHintIncorrect();
        }
    }
}
