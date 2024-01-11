using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[ExecuteInEditMode]
public class Splittable : MonoBehaviour
{
    BuildingItem buildingItem;

    public GameObject spawnable;
    public bool spawned = false;
    public int hitsToSplit = 3;

    [Header("Optional")]
    public EasyFX easyFX;
    public PFX PFX = PFX.Undefined;
    public SFX SFX = SFX.Undefined;

    private void OnEnable()
    {
        buildingItem = GetComponent<BuildingItem>();
        easyFX = FindObjectOfType<EasyFX>();
        PFX = PFX.Wood;
        SFX = SFX.WoodSplit;
        DefineSplitPrefab();
        ParticleSystem splittableHint = GetComponentInChildren<ParticleSystem>();
        splittableHint?.Play();
    }
    public void DefineSplitPrefab()
    {
        string[] itemName = this.gameObject.name.Split(char.Parse("X"));
        foreach (string definition in itemName)
        {
            if (definition == "Log_Raw_Pulp_Full_")
                spawnable = Resources.Load("LogTypes/Raw/Log_Raw_Pulp_Split_X") as GameObject;
            else if (definition == "Log_Raw_Saw_Full_")
                spawnable = Resources.Load("LogTypes/Raw/Log_Raw_Saw_Split_X") as GameObject;
            else if (definition == "Log_Raw_Veneer_Full_")
                spawnable = Resources.Load("LogTypes/Raw/Log_Raw_Veneer_Split_X") as GameObject;
            else if (definition == "Log_Workable_Pulp_Square_6Board_Full_")
                spawnable = Resources.Load("LogTypes/Finished/Pulp_Square/Log_Workable_Pulp_Square_6Board_X") as GameObject;
            else if (definition == "Log_Workable_Saw_D_6Board_Full_")
                spawnable = Resources.Load("LogTypes/Finished/Saw_D/Log_Workable_Saw_D_6Board_X") as GameObject;
            else if (definition == "Log_Workable_Veneer_Round_6Board_Full_")
                spawnable = Resources.Load("LogTypes/Finished/Veneer_Round/Log_Workable_Veneer_Round_6Board_X") as GameObject;
        }
    }

    private void Update()
    {
        if (hitsToSplit <= 0 && spawned == false)
            SplitLog();
    }
    public void DecrementHitsToSplit()
    {
        if (buildingItem != null)
        {
            if (buildingItem.delimbed || (buildingItem.status == Status.Full && buildingItem.type == Type.Board && buildingItem.finished))
            {
                hitsToSplit--;
            }
        }
    }
    public void SplitLog()
    {
        spawned = true;
        easyFX?.PlayPFX((int)PFX, transform.position, transform.rotation, 1);
        easyFX?.PlaySFX((int)SFX, transform.position, 1);
        float newY = this.gameObject.transform.rotation.y + 180f;
        Instantiate(spawnable, this.gameObject.transform.position, this.gameObject.transform.rotation);
        Instantiate(spawnable, this.gameObject.transform.position, Quaternion.Euler(this.gameObject.transform.rotation.x, newY, this.gameObject.transform.rotation.z));
        Destroy(this.gameObject);
    }
}
