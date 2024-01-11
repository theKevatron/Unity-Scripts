using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[ExecuteInEditMode]
public class JsonData : MonoBehaviour
{
    string filename = "save1.json";
    string path;
    GameData gameData = new GameData();

    public Storage storage;
    public Building building1;
    public Building building2;
    public Building building3;

    //public Building[] buildings

    private void Start()
    {
        path = Application.persistentDataPath + "/" + filename;
        Debug.Log(path);

        if (storage == null)
            storage = GameObject.FindObjectOfType<Storage>();

        if (building1 == null)
            building1 = GameObject.FindGameObjectWithTag("Building_1").GetComponent<Building>();
        if (building2 == null)
            building2 = GameObject.FindGameObjectWithTag("Building_2").GetComponent<Building>();
        if (building3 == null)
            building3 = GameObject.FindGameObjectWithTag("Building_3").GetComponent<Building>();

        //buildings = FindObjectsOfType<Building>();
    }

    public void SaveData()
    {
        gameData.date = System.DateTime.Now.ToShortDateString();
        gameData.time = System.DateTime.Now.ToShortTimeString();

        gameData.storageItems.Clear();
        foreach (BuildingItem item in storage.items)
        {
            if (item != null)
            {
                gameData.storageItems.Add(item.gameObject.name);
            }
        }

        if (building1 != null)
        {
            gameData.building1Progress = building1.currentActivatableIndex;
            gameData.building1LastIndex = building1.lastIndex;
        }
        if (building2 != null)
        {
            gameData.building2Progress = building2.currentActivatableIndex;
            gameData.building2LastIndex = building2.lastIndex;
        }
        if (building2 != null)
        {
            gameData.building3Progress = building3.currentActivatableIndex;
            gameData.building3LastIndex = building3.lastIndex;
        }

        /*
        foreach (Building building in buildings)
        {
            string nameAndProgress = building.gameObject.name + "_" + building.currentActivatableIndex;
            gameData.buildingsProgress.Add(nameAndProgress);
        }
        */

        string contents = JsonUtility.ToJson(gameData, true);
        System.IO.File.WriteAllText(path, contents);
    }
    public void ReadData()
    {
        //try
        {
            if (System.IO.File.Exists(path))
            {
                // Date and Time
                string contents = System.IO.File.ReadAllText(path);
                gameData = JsonUtility.FromJson<GameData>(contents);
                Debug.Log(gameData.date + ", " + gameData.time);

                // Storage
                foreach (BuildingItem item in storage?.items)
                {
                    Destroy(item.gameObject);
                }
                storage.items.Clear();
                foreach (string itemName in gameData.storageItems)
                {
                    storage.LoadLog(itemName);
                }

                // Buildings
                if (building1 != null)
                {
                    building1.currentActivatableIndex = gameData.building1Progress;
                    building1.lastIndex = gameData.building1LastIndex;
                    building1.LoadCompleted();
                }
                if (building2 != null)
                {
                    building2.currentActivatableIndex = gameData.building2Progress;
                    building2.lastIndex = gameData.building2LastIndex;
                    building2.LoadCompleted();
                }
                if (building3 != null)
                {
                    building3.currentActivatableIndex = gameData.building3Progress;
                    building3.lastIndex = gameData.building3LastIndex;
                    building3.LoadCompleted();
                }
            }
            else
            {
                Debug.Log("Unable to read the save data, file does not exist");
                gameData = new GameData();
            }
        }
        //catch (System.Exception ex)
        {
            //Debug.Log(ex.Message);
        }
    }
}
