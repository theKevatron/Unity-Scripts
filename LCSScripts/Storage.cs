using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

[DisallowMultipleComponent]
public class Storage: MonoBehaviour
{
    public bool easyPlaceOn = true;
    public EasyPlace easyPlace;

    private BuildingItem inspector;
    public List<BuildingItem> items = new List<BuildingItem>();

    [Header("UI Info")]
    public TextMeshProUGUI storageCount;
    public string textDescription = "Logs In Storage: ";
    public int itemCount;

    void Start()
    {
        TryGetEasyPlace();

        if (storageCount == null)
            storageCount = this.gameObject.GetComponentInChildren<TextMeshProUGUI>();

        if (storageCount == null)
            storageCount = GameObject.Find("Text_StorageCount").GetComponent<TextMeshProUGUI>();

        if (storageCount != null)
            storageCount.text = textDescription + itemCount;
    }
    private void TryGetEasyPlace()
    {
        if (easyPlaceOn == true)
        {
            if (easyPlace == null)
                easyPlace = this.GetComponentInChildren<EasyPlace>();
            if (easyPlace == null)
                easyPlace = GameObject.Find("EasyPlace_Storage").GetComponent<EasyPlace>();
            if (easyPlace == null)
            {
                Debug.Log("No Easyplace Component found, default setting will not allow EasyPlace! - Storage.cs, Start()");
                easyPlaceOn = false;
            }
        }
    }
    public void LoadLog(string itemName)
    {
        string[] definitions = itemName.Split(char.Parse("X"));
        GameObject logPrefab = Resources.Load("LogTypes/Raw/Log_Raw_Veneer_Full_X") as GameObject;
        // Rock
        if (definitions[0] == "RockSmall_")
            logPrefab = Resources.Load("RockSmall_X") as GameObject;
        // Raw
        else if (definitions[0] == "Log_Raw_Pulp_Full_")
            logPrefab = Resources.Load("LogTypes/Raw/Log_Raw_Pulp_Full_X") as GameObject;
        else if (definitions[0] == "Log_Raw_Pulp_Split_")
            logPrefab = Resources.Load("LogTypes/Raw/Log_Raw_Pulp_Split_X") as GameObject;
        else if (definitions[0] == "Log_Raw_Saw_Full_")
            logPrefab = Resources.Load("LogTypes/Raw/Log_Raw_Saw_Full_X") as GameObject;
        else if (definitions[0] == "Log_Raw_Saw_Split_")
            logPrefab = Resources.Load("LogTypes/Raw/Log_Raw_Saw_Split_X") as GameObject;
        else if (definitions[0] == "Log_Raw_Veneer_Full_")
            logPrefab = Resources.Load("LogTypes/Raw/Log_Raw_Veneer_Full_X") as GameObject;
        else if (definitions[0] == "Log_Raw_Veneer_Split_")
            logPrefab = Resources.Load("LogTypes/Raw/Log_Raw_Veneer_Split_X") as GameObject;
        // Finished
        // Pulp
        else if (definitions[0] == "Log_Workable_Pulp_Square_1Sill_")
            logPrefab = Resources.Load("LogTypes/Finished/Pulp_Square/Log_Workable_Pulp_Square_1Sill_X") as GameObject;
        else if (definitions[0] == "Log_Workable_Pulp_Square_2Wall_")
            logPrefab = Resources.Load("LogTypes/Finished/Pulp_Square/Log_Workable_Pulp_Square_2Wall_X") as GameObject;
        else if (definitions[0] == "Log_Workable_Pulp_Square_3Top_")
            logPrefab = Resources.Load("LogTypes/Finished/Pulp_Square/Log_Workable_Pulp_Square_3Top_X") as GameObject;
        else if (definitions[0] == "Log_Workable_Pulp_Square_4Gable_")
            logPrefab = Resources.Load("LogTypes/Finished/Pulp_Square/Log_Workable_Pulp_Square_4Gable_X") as GameObject;
        else if (definitions[0] == "Log_Workable_Pulp_Square_5Ridge_")
            logPrefab = Resources.Load("LogTypes/Finished/Pulp_Square/Log_Workable_Pulp_Square_5Ridge_X") as GameObject;
        else if (definitions[0] == "Log_Workable_Pulp_Square_6Board_Full_")
            logPrefab = Resources.Load("LogTypes/Finished/Pulp_Square/Log_Workable_Pulp_Square_6Board_Full_X") as GameObject;
        else if (definitions[0] == "Log_Workable_Pulp_Square_6Board_")
            logPrefab = Resources.Load("LogTypes/Finished/Pulp_Square/Log_Workable_Pulp_Square_6Board_X") as GameObject;
        // Saw
        else if (definitions[0] == "Log_Workable_Saw_D_1Sill_")
            logPrefab = Resources.Load("LogTypes/Finished/Saw_D/Log_Workable_Saw_D_1Sill_X") as GameObject;
        else if (definitions[0] == "Log_Workable_Saw_D_2Wall_")
            logPrefab = Resources.Load("LogTypes/Finished/Saw_D/Log_Workable_Saw_D_2Wall_X") as GameObject;
        else if (definitions[0] == "Log_Workable_Saw_D_3Top_")
            logPrefab = Resources.Load("LogTypes/Finished/Saw_D/Log_Workable_Saw_D_3Top_X") as GameObject;
        else if (definitions[0] == "Log_Workable_Saw_D_4Gable_")
            logPrefab = Resources.Load("LogTypes/Finished/Saw_D/Log_Workable_Saw_D_4Gable_X") as GameObject;
        else if (definitions[0] == "Log_Workable_Saw_D_5Ridge_")
            logPrefab = Resources.Load("LogTypes/Finished/Saw_D/Log_Workable_Saw_D_5Ridge_X") as GameObject;
        else if (definitions[0] == "Log_Workable_Saw_D_6Board_Full_")
            logPrefab = Resources.Load("LogTypes/Finished/Saw_D/Log_Workable_Saw_D_6Board_Full_X") as GameObject;
        else if (definitions[0] == "Log_Workable_Saw_D_6Board_")
            logPrefab = Resources.Load("LogTypes/Finished/Saw_D/Log_Workable_Saw_D_6Board_X") as GameObject;
        // Veneer
        else if (definitions[0] == "Log_Workable_Veneer_Round_1Sill_")
            logPrefab = Resources.Load("LogTypes/Finished/Veneer_Round/Log_Workable_Veneer_Round_1Sill_X") as GameObject;
        else if (definitions[0] == "Log_Workable_Veneer_Round_2Wall_")
            logPrefab = Resources.Load("LogTypes/Finished/Veneer_Round/Log_Workable_Veneer_Round_2Wall_X") as GameObject;
        else if (definitions[0] == "Log_Workable_Veneer_Round_3Top_")
            logPrefab = Resources.Load("LogTypes/Finished/Veneer_Round/Log_Workable_Veneer_Round_3Top_X") as GameObject;
        else if (definitions[0] == "Log_Workable_Veneer_Round_4Gable_")
            logPrefab = Resources.Load("LogTypes/Finished/Veneer_Round/Log_Workable_Veneer_Round_4Gable_X") as GameObject;
        else if (definitions[0] == "Log_Workable_Veneer_Round_5Ridge_")
            logPrefab = Resources.Load("LogTypes/Finished/Veneer_Round/Log_Workable_Veneer_Round_5Ridge_X") as GameObject;
        else if (definitions[0] == "Log_Workable_Veneer_Round_6Board_Full_")
            logPrefab = Resources.Load("LogTypes/Finished/Veneer_Round/Log_Workable_Veneer_Round_6Board_Full_X") as GameObject;
        else if (definitions[0] == "Log_Workable_Veneer_Round_6Board_")
            logPrefab = Resources.Load("LogTypes/Finished/Veneer_Round/Log_Workable_Veneer_Round_6Board_X") as GameObject;

        Instantiate(logPrefab, easyPlace.transform.position, easyPlace.transform.rotation);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 20)
        {
            if (other.gameObject.CompareTag("BuildingItem"))
            {
                inspector = other.gameObject.GetComponent<BuildingItem>();
                if (inspector?.delimbed == true || inspector?.finished == true || inspector?.rockSmall == true)
                {
                    items.Add(other.gameObject.GetComponent<BuildingItem>());
                    if (easyPlace != null)
                        other.transform.SetParent(easyPlace.transform);
                    RemoveNull();
                    itemCount = items.Count;
                    storageCount.text = textDescription + itemCount;

                    if (inspector?.isHeld == false)
                    {
                        // Easy place can be removed
                        if (easyPlaceOn == true)
                        {
                            easyPlace?.PlaceGameObject(other);
                        }
                    }
                }
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 20)
        {
            if (other.gameObject.CompareTag("BuildingItem"))
            {
                inspector = other.gameObject.GetComponent<BuildingItem>();
                if (inspector?.delimbed == true || inspector?.finished == true || inspector?.rockSmall == true)
                {
                    items.Remove(other.gameObject.GetComponent<BuildingItem>());
                    RemoveNull();
                    itemCount = items.Count;
                    storageCount.text = textDescription + itemCount;
                }
            }
        }
    }
    public void OnRelease(Collider other)
    {
        if (other.gameObject.layer == 20)
        {
            if (other.gameObject.CompareTag("BuildingItem"))
            {
                if (inspector?.isTransportable == true && inspector?.isHeld == false)
                {
                    // Easy place can be removed
                    if (easyPlaceOn == true)
                    {
                        easyPlace?.PlaceGameObject(other);
                    }
                }
            }
        }
    }
    public void RemoveNull()
    {
        for (int i = items.Count - 1; i >= 0; i--)
        {
            if (items[i] == null)
            {
                items.RemoveAt(i);
            }
        }
    }
}
