using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public string date = "";
    public string time = "";

    public List<string> storageItems = new List<string>();

    public int building1Progress = 0;
    public int building1LastIndex = 0;
    public int building2Progress = 0;
    public int building2LastIndex = 0;
    public int building3Progress = 0;
    public int building3LastIndex = 0;

    //public List<string> buildingsProgress = new List<string>();
}
