using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[DisallowMultipleComponent]
public class WorkStation : MonoBehaviour
{
    /// CREATE SIDE PANEL TO SHOW DESCRIPTIONS OF SELECTED OPTIONS
    /// Colors for button changing functions
    [Header("Colors")]
    Color blackTransparent = new Color(0, 0, 0, 0.64f);
    Color white = new Color(1, 1, 1, 1);
    [Header("Log")]
    public BuildingItem inspector;
    public List<BuildingItem> miscLogs;

    public Status selectedStatusEnum = Status.Undefined;
    public Quality selectedQualityEnum = Quality.Undefined;
    public Style selectedStyleEnum = Style.Undefined;
    public Type selectedTypeEnum = Type.Undefined;

    [Header("Workable")]
    public BuildingItem workable;
    public bool stationOccupied = false;

    /// SET ON START
    [Header("ASSIGN ON START - EasyPlace")]
    public bool easyPlaceOn = true;
    public EasyPlace easyPlace;
    [Header("ASSIGN ON START - Log Prefabs")]
    public GameObject[] workableLogsPulpSquare;
    public GameObject[] workableLogsSawD;
    public GameObject[] workableLogsVeneerRound;
    private string workableLogsPulpSquareAddress = "LogTypes/Workable/Pulp_Square";
    private string workableLogsSawDAddress = "LogTypes/Workable/Saw_D";
    private string workableLogsVeneerRoundAddress = "LogTypes/Workable/Veneer_Round";

    [Header("ASSIGN ON START - UI")]
    public TextMeshProUGUI subheadingStatus;
    public Toggle selectFull;
    public Toggle selectSplit;
    public TextMeshProUGUI subheadingQuality;
    public Toggle selectPulp;
    public Toggle selectSaw;
    public Toggle selectVeneer;
    public TextMeshProUGUI subheadingStyle;
    public Toggle selectSquare;
    public Toggle selectD;
    public Toggle selectRound;
    public TextMeshProUGUI subheadingType;
    public Toggle selectSill;
    public Toggle selectWall;
    public Toggle selectTop;
    public Toggle selectGable;
    public Toggle selectRidge;
    public Toggle selectBoard;
    public Button confirm;
    public Button resetOpen;
    public TextMeshProUGUI textResetStation;
    public Button resetConfirm;
    public Button resetCancel;
    public TextMeshProUGUI textStationOccupied;
    public Button recenter;
    public GameObject stationBarriers;

    void Awake()
    {
        if (easyPlaceOn == true)
            easyPlace = GetComponentInChildren<EasyPlace>();

        LoadWorkableLogs();
        LoadUIElements();
    }
    // LOAD ON START
    public void LoadWorkableLogs()
    {
        object[] loadedLogTypes1 = Resources.LoadAll(workableLogsPulpSquareAddress);
        workableLogsPulpSquare = new GameObject[loadedLogTypes1.Length];
        for (int x = 0; x < loadedLogTypes1.Length; x++)
        {
            workableLogsPulpSquare[x] = (GameObject)loadedLogTypes1[x];
        }

        object[] loadedLogTypes5 = Resources.LoadAll(workableLogsSawDAddress);
        workableLogsSawD = new GameObject[loadedLogTypes5.Length];
        for (int x = 0; x < loadedLogTypes5.Length; x++)
        {
            workableLogsSawD[x] = (GameObject)loadedLogTypes5[x];
        }

        object[] loadedLogTypes9 = Resources.LoadAll(workableLogsVeneerRoundAddress);
        workableLogsVeneerRound = new GameObject[loadedLogTypes9.Length];
        for (int x = 0; x < loadedLogTypes9.Length; x++)
        {
            workableLogsVeneerRound[x] = (GameObject)loadedLogTypes9[x];
        }
    }
    public void LoadUIElements()
    {
        // Assign Buttons by gameObject name
        subheadingStatus = GameObject.Find("Subheading_Status").GetComponent<TextMeshProUGUI>();
        selectFull = GameObject.Find("Toggle_Full").GetComponent<Toggle>();
        selectSplit = GameObject.Find("Toggle_Split").GetComponent<Toggle>();
        subheadingQuality = GameObject.Find("Subheading_Quality").GetComponent<TextMeshProUGUI>();
        selectPulp = GameObject.Find("Toggle_Pulp").GetComponent<Toggle>();
        selectSaw = GameObject.Find("Toggle_Saw").GetComponent<Toggle>();
        selectVeneer = GameObject.Find("Toggle_Veneer").GetComponent<Toggle>();
        subheadingStyle = GameObject.Find("Subheading_Style").GetComponent<TextMeshProUGUI>();
        selectSquare = GameObject.Find("Toggle_Square").GetComponent<Toggle>();
        selectD = GameObject.Find("Toggle_D").GetComponent<Toggle>();
        selectRound = GameObject.Find("Toggle_Round").GetComponent<Toggle>();
        subheadingType = GameObject.Find("Subheading_Type").GetComponent<TextMeshProUGUI>();
        selectSill = GameObject.Find("Toggle_Sill").GetComponent<Toggle>();
        selectWall = GameObject.Find("Toggle_Wall").GetComponent<Toggle>();
        selectTop = GameObject.Find("Toggle_Top").GetComponent<Toggle>();
        selectGable = GameObject.Find("Toggle_Gable").GetComponent<Toggle>();
        selectRidge = GameObject.Find("Toggle_Ridge").GetComponent<Toggle>();
        selectBoard = GameObject.Find("Toggle_Board").GetComponent<Toggle>();
        confirm = GameObject.Find("Button_Confirm").GetComponent<Button>();
        resetOpen = GameObject.Find("Button_ResetOpen").GetComponent<Button>();
        textResetStation = GameObject.Find("Text_RESETSTATION").GetComponent<TextMeshProUGUI>();
        resetConfirm = GameObject.Find("Button_ResetConfirm").GetComponent<Button>();
        resetCancel = GameObject.Find("Button_ResetCancel").GetComponent<Button>();
        textStationOccupied = GameObject.Find("Text_STATIONOCCUPIED").GetComponent<TextMeshProUGUI>();
        recenter = GameObject.Find("Button_Recenter").GetComponent<Button>();
        stationBarriers = GameObject.Find("StationBarriers");

        selectFull?.onValueChanged.AddListener(OnSelectFullToggled);
        selectSplit?.onValueChanged.AddListener(OnSelectSplitToggled);
        selectPulp?.onValueChanged.AddListener(OnSelectPulpToggled);
        selectSaw?.onValueChanged.AddListener(OnSelectSawToggled);
        selectVeneer?.onValueChanged.AddListener(OnSelectVeneerToggled);
        selectSquare?.onValueChanged.AddListener(OnSelectSquareToggled);
        selectD?.onValueChanged.AddListener(OnSelectDToggled);
        selectRound?.onValueChanged.AddListener(OnSelectRoundToggled);
        selectSill?.onValueChanged.AddListener(OnSelectSillToggled);
        selectWall?.onValueChanged.AddListener(OnSelectWallToggled);
        selectTop?.onValueChanged.AddListener(OnSelectTopToggled);
        selectGable?.onValueChanged.AddListener(OnSelectGableToggled);
        selectRidge?.onValueChanged.AddListener(OnSelectRidgeToggled);
        selectBoard?.onValueChanged.AddListener(OnSelectBoardToggled);

        confirm?.onClick.AddListener(OnConfirmClicked);
        resetOpen?.onClick.AddListener(OnResetOpenClicked);
        resetConfirm?.onClick.AddListener(OnResetConfirmClicked);
        resetCancel?.onClick.AddListener(OnResetCancelClicked);

        recenter?.onClick.AddListener(OnRecenterClicked);

        UpdateStation();
    }
    // UI COMPONENT FUNCTIONS
    public void DeactivateToggle(Toggle toggle)
    {
        toggle.isOn = false;
        toggle.image.color = blackTransparent;
        toggle.interactable = false;
    }
    public void ActivateToggle(Toggle toggle)
    {
        toggle.image.color = white;
        toggle.interactable = true;
    }
    public void AutoToggle(Toggle toggle)
    {
        toggle.isOn = true;
        ActivateToggle(toggle);
    }

    private void Update()
    {
        if (workable == null)
        {
            stationOccupied = false;
            textStationOccupied?.gameObject.SetActive(false);
            recenter?.gameObject.SetActive(false);
            stationBarriers.SetActive(false);
        }
        else if (workable != null)
        {
            stationOccupied = true;
            textStationOccupied?.gameObject.SetActive(true);
            recenter?.gameObject.SetActive(true);
            stationBarriers.SetActive(true);

            if (workable.finished)
            {
                workable = null;
            }

            /*
            // SET STATUS BASED ON WORKABLE(For description board)
            if (workable.full)
                selectedStatus = 1;
            else if (workable.split)
                selectedStatus = 2;
            if (workable.pulp)
                selectedQuality = 1;
            else if (workable.saw)
                selectedQuality = 2;
            else if (workable.veneer)
                selectedQuality = 3;
            if (workable.round)
                selectedStyle = 1;
            else if (workable.d)
                selectedStyle = 2;
            else if (workable.square)
                selectedStyle = 3;
            if (workable.sill)
                selectedType = 1;
            else if (workable.wall)
                selectedType = 2;
            else if (workable.top)
                selectedType = 3;
            else if (workable.gable)
                selectedType = 4;
            else if (workable.ridge)
                selectedType = 5;
            else if (workable.board)
                selectedType = 6;
            */
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 20)
        {
            if (stationOccupied == false)
            {
                if (other.gameObject.CompareTag("BuildingItem"))
                {
                    inspector = other.GetComponent<BuildingItem>();
                    if (inspector?.delimbed == true)
                    {
                        miscLogs.Add(inspector);
                        RemoveNull();
                        OnRelease(other);
                        UpdateStation();
                    }
                }
            }
        }
    }
    public void UpdateStation()
    {
        selectSill.transform.parent.gameObject.SetActive(false);
        selectWall.transform.parent.gameObject.SetActive(false);
        selectTop.transform.parent.gameObject.SetActive(false);
        selectGable.transform.parent.gameObject.SetActive(false);
        selectRidge.transform.parent.gameObject.SetActive(false);
        selectBoard.transform.parent.gameObject.SetActive(false);
        resetOpen?.gameObject.SetActive(true);
        textResetStation.gameObject.SetActive(false);
        resetConfirm?.gameObject.SetActive(false);
        resetCancel?.gameObject.SetActive(false);
        subheadingType.enabled = false;
        confirm?.gameObject.SetActive(false);
        // RESET SELECTED TRAITS AND UNTOGGLE ALL
        selectedStatusEnum = Status.Undefined;
        selectedQualityEnum = Quality.Undefined;
        selectedStyleEnum = Style.Undefined;
        selectedTypeEnum = Type.Undefined;
        if (selectFull != null)
            DeactivateToggle(selectFull);
        if (selectSplit != null)
            DeactivateToggle(selectSplit);
        if (selectPulp != null)
            DeactivateToggle(selectPulp);
        if (selectSaw != null)
            DeactivateToggle(selectSaw);
        if (selectVeneer != null)
            DeactivateToggle(selectVeneer);
        if (selectRound != null)
            DeactivateToggle(selectRound);
        if (selectD != null)
            DeactivateToggle(selectD);
        if (selectSquare != null)
            DeactivateToggle(selectSquare);
        if (selectSill != null)
            DeactivateToggle(selectSill);
        if (selectWall != null)
            DeactivateToggle(selectWall);
        if (selectTop != null)
            DeactivateToggle(selectTop);
        if (selectGable != null)
            DeactivateToggle(selectGable);
        if (selectRidge != null)
            DeactivateToggle(selectRidge);
        if (selectBoard != null)
            DeactivateToggle(selectBoard);

        if (inspector != null)
        {
            // AUTO TOGGLE CORRECT TRAITS BASED ON INSPECTOR
            if (inspector.status == Status.Full && selectFull != null)
                AutoToggle(selectFull);
            else if (inspector.status == Status.Split && selectSplit != null)
                AutoToggle(selectSplit);
            if (inspector.quality == Quality.Pulp && selectPulp != null)
            {
                AutoToggle(selectPulp);
                AutoToggle(selectSquare);
            }
            else if (inspector.quality == Quality.Saw && selectSaw != null)
            {
                AutoToggle(selectSaw);
                AutoToggle(selectD);
            }
            else if (inspector.quality == Quality.Veneer && selectVeneer != null)
            {
                AutoToggle(selectVeneer);
                AutoToggle(selectRound);
            }
        }
        // ACTIVATE TYPE OPTIONS
        CheckFor3TraitsSelected();
        UpdateSelectedTraitsText();
    }
    public void OnRelease(Collider other)
    {
        if (other.gameObject.layer == 20)
        {
            if (stationOccupied == false)
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
    }
    private void RemoveNull()
    {
        for (int i = miscLogs.Count - 1; i >= 0; i--)
        {
            if (miscLogs[i] == null)
            {
                miscLogs.RemoveAt(i);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 20)
        {
            if (stationOccupied == false)
            {
                if (other.gameObject.CompareTag("BuildingItem"))
                {
                    inspector = other.GetComponent<BuildingItem>();
                    if (inspector?.delimbed == true)
                    {
                        miscLogs.Remove(inspector);
                        RemoveNull();
                        inspector = null;
                        UpdateStation();
                    }
                }
            }
        }
    }

    // Called OnTriggerEnter/Exit AND on each toggle/button click 
    public void CheckFor3TraitsSelected()
    {
        if (selectedStatusEnum != Status.Undefined)
            if (selectedQualityEnum != Quality.Undefined)
                if (selectedStyleEnum != Style.Undefined)
                    ActivateTogglesType();
    }
    public void ActivateTogglesType()
    {
        // Split
        if (selectedStatusEnum == Status.Full)
        {
            ActivateTogglesTypeFull();
        }
        // Full
        else if (selectedStatusEnum == Status.Split)
        {
            ActivateTogglesTypeSplit();
        }
    }
    public void ActivateTogglesTypeSplit()
    {
        subheadingType.enabled = true;
        confirm?.gameObject.SetActive(true);
        if (selectSill?.gameObject.activeSelf == true)
        {
            selectSill.transform.parent.gameObject.SetActive(true);
            ActivateToggle(selectSill);
        }
        if (selectWall?.gameObject.activeSelf == true)
        {
            selectWall.transform.parent.gameObject.SetActive(false);
            DeactivateToggle(selectWall);
        }
        if (selectTop?.gameObject.activeSelf == true)
        {
            selectTop.transform.parent.gameObject.SetActive(false);
            DeactivateToggle(selectTop);
        }
        if (selectGable?.gameObject.activeSelf == true)
        {
            selectGable.transform.parent.gameObject.SetActive(false);
            DeactivateToggle(selectGable);
        }
        if (selectRidge?.gameObject.activeSelf == true)
        {
            selectRidge.transform.parent.gameObject.SetActive(false);
            DeactivateToggle(selectRidge);
        }
        if (selectBoard?.gameObject.activeSelf == true)
        {
            selectBoard.transform.parent.gameObject.SetActive(true);
            ActivateToggle(selectBoard);
        }
    }
    public void ActivateTogglesTypeFull()
    {
        subheadingType.enabled = true;
        confirm?.gameObject.SetActive(true);
        if (selectSill?.gameObject.activeSelf == true)
        {
            selectSill.transform.parent.gameObject.SetActive(false);
            DeactivateToggle(selectSill);
        }
        if (selectWall?.gameObject.activeSelf == true)
        {
            selectWall.transform.parent.gameObject.SetActive(true);
            ActivateToggle(selectWall);
        }
        if (selectTop?.gameObject.activeSelf == true)
        {
            selectTop.transform.parent.gameObject.SetActive(true);
            ActivateToggle(selectTop);
        }
        if (selectGable?.gameObject.activeSelf == true)
        {
            selectGable.transform.parent.gameObject.SetActive(true);
            ActivateToggle(selectGable);
        }
        if (selectRidge?.gameObject.activeSelf == true)
        {
            selectRidge.transform.parent.gameObject.SetActive(true);
            ActivateToggle(selectRidge);
        }
        if (selectBoard?.gameObject.activeSelf == true)
        {
            selectBoard.transform.parent.gameObject.SetActive(false);
            DeactivateToggle(selectBoard);
        }
    }

    // TOGGLE FUNCTIONS
    public void UpdateSelectedTraitsText()
    {
        if (subheadingStatus != null)
        {
            if (selectedStatusEnum == 0)
                subheadingStatus.text = "Log Status: ";
            else if (selectedStatusEnum == Status.Full)
                subheadingStatus.text = "Log Status: FULL";
            else if (selectedStatusEnum == Status.Split)
                subheadingStatus.text = "Log Status: SPLIT";
        }
        if (subheadingQuality != null)
        {
            if (selectedQualityEnum == 0)
                subheadingQuality.text = "Log Quality: ";
            else if (selectedQualityEnum == Quality.Pulp)
                subheadingQuality.text = "Log Quality: PULP";
            else if (selectedQualityEnum == Quality.Saw)
                subheadingQuality.text = "Log Quality: SAW";
            else if (selectedQualityEnum == Quality.Veneer)
                subheadingQuality.text = "Quality: VENEER";
        }
        if (subheadingStyle != null)
        {
            if (selectedStyleEnum == 0)
                subheadingStyle.text = "Log Style: ";
            else if (selectedStyleEnum == Style.Square)
                subheadingStyle.text = "Log Style: SQUARE";
            else if (selectedStyleEnum == Style.D)
                subheadingStyle.text = "Log Style: D";
            else if (selectedStyleEnum == Style.Round)
                subheadingStyle.text = "Log Style: ROUND";
        }
        if (subheadingType != null)
        {
            if (selectedTypeEnum == 0)
                subheadingType.text = "Select Type: ";
            else if (selectedTypeEnum == Type.Sill)
                subheadingType.text = "Type: SILL";
            else if(selectedTypeEnum == Type.Wall)
                subheadingType.text = "Type: WALL";
            else if(selectedTypeEnum == Type.Top)
                subheadingType.text = "Type: TOP";
            else if(selectedTypeEnum == Type.Gable)
                subheadingType.text = "Type: GABLE";
            else if(selectedTypeEnum == Type.Ridge)
                subheadingType.text = "Type: RIDGE";
            else if(selectedTypeEnum == Type.Board)
                subheadingType.text = "Type: BOARD";
        }
    }
    public void OnSelectFullToggled(bool statusSelected)
    {
        selectedStatusEnum = statusSelected ? Status.Full : Status.Undefined;
        CheckFor3TraitsSelected();
        UpdateSelectedTraitsText();
    }
    public void OnSelectSplitToggled(bool statusSelected)
    {
        selectedStatusEnum = statusSelected ? Status.Split : Status.Undefined;
        CheckFor3TraitsSelected();
        UpdateSelectedTraitsText();
    }
    public void OnSelectPulpToggled(bool qualitySelected)
    {
        selectedQualityEnum = qualitySelected ? Quality.Pulp : Quality.Undefined;
        CheckFor3TraitsSelected();
        UpdateSelectedTraitsText();
    }
    public void OnSelectSawToggled(bool qualitySelected)
    {
        selectedQualityEnum = qualitySelected ? Quality.Saw : Quality.Undefined;
        CheckFor3TraitsSelected();
        UpdateSelectedTraitsText();
    }
    public void OnSelectVeneerToggled(bool qualitySelected)
    {
        selectedQualityEnum = qualitySelected ? Quality.Veneer : Quality.Undefined;
        CheckFor3TraitsSelected();
        UpdateSelectedTraitsText();
    }
    public void OnSelectSquareToggled(bool styleSelected)
    {
        selectedStyleEnum = styleSelected ? Style.Square : Style.Undefined;
        CheckFor3TraitsSelected();
        UpdateSelectedTraitsText();
    }
    public void OnSelectDToggled(bool styleSelected)
    {
        selectedStyleEnum = styleSelected ? Style.D: Style.Undefined;
        CheckFor3TraitsSelected();
        UpdateSelectedTraitsText();
    }
    public void OnSelectRoundToggled(bool styleSelected)
    {
        selectedStyleEnum = styleSelected ? Style.Round : Style.Undefined;
        CheckFor3TraitsSelected();
        UpdateSelectedTraitsText();
    }
    public void OnSelectSillToggled(bool typeSelected)
    {
        selectedTypeEnum = typeSelected ? Type.Sill : Type.Undefined;
        CheckFor3TraitsSelected();
        UpdateSelectedTraitsText();
    }
    public void OnSelectWallToggled(bool typeSelected)
    {
        selectedTypeEnum = typeSelected ? Type.Wall : Type.Undefined;
        CheckFor3TraitsSelected();
        UpdateSelectedTraitsText();
    }
    public void OnSelectTopToggled(bool typeSelected)
    {
        selectedTypeEnum = typeSelected ? Type.Top : Type.Undefined;
        CheckFor3TraitsSelected();
        UpdateSelectedTraitsText();
    }
    public void OnSelectGableToggled(bool typeSelected)
    {
        selectedTypeEnum = typeSelected ? Type.Gable : Type.Undefined;
        CheckFor3TraitsSelected();
        UpdateSelectedTraitsText();
    }
    public void OnSelectRidgeToggled(bool typeSelected)
    {
        selectedTypeEnum = typeSelected ? Type.Ridge : Type.Undefined;
        CheckFor3TraitsSelected();
        UpdateSelectedTraitsText();
    }
    public void OnSelectBoardToggled(bool typeSelected)
    {
        selectedTypeEnum = typeSelected ? Type.Board : Type.Undefined;
        CheckFor3TraitsSelected();
        UpdateSelectedTraitsText();
    }
    public void OnConfirmClicked()
    {
        AttemptInstantiateWorkable();
    }
    public void OnResetOpenClicked()
    {
        resetOpen?.gameObject.SetActive(false);
        textResetStation.gameObject.SetActive(true);
        resetConfirm?.gameObject.SetActive(true);
        resetCancel?.gameObject.SetActive(true);
    }
    public void OnResetConfirmClicked()
    {
        ResetStation();
    }
    public void OnResetCancelClicked()
    {
        resetOpen?.gameObject.SetActive(true);
        textResetStation.gameObject.SetActive(false);
        resetConfirm?.gameObject.SetActive(false);
        resetCancel?.gameObject.SetActive(false);
    }
    public void OnRecenterClicked()
    {
        if (workable != null && easyPlace != null)
        {
            workable.transform.position = easyPlace.transform.position;
            workable.transform.rotation = easyPlace.transform.rotation;
        }
    }

    // CONFIRM - Called on confirm selection button
    public void AttemptInstantiateWorkable()
    {
        if (stationOccupied == false && easyPlace != null)
        {
            if (selectedQualityEnum == Quality.Pulp)
            {
                if (selectedStyleEnum == Style.Square)
                    if (selectedTypeEnum != Type.Undefined)
                        InstantiateWorkable(workableLogsPulpSquare);
            }
            else if (selectedQualityEnum == Quality.Saw)
            {
                if (selectedStyleEnum == Style.D)
                    if (selectedTypeEnum != Type.Undefined)
                        InstantiateWorkable(workableLogsSawD);
            }
            else if (selectedQualityEnum == Quality.Veneer)
            {
                if (selectedStyleEnum == Style.Round)
                    if (selectedTypeEnum != Type.Undefined)
                        InstantiateWorkable(workableLogsVeneerRound);
            }
        }
    }
    public void InstantiateWorkable(GameObject[] workableLogs)
    {
        workable = Instantiate(workableLogs[((int)selectedTypeEnum) - 1], easyPlace.transform.position, easyPlace.transform.rotation).GetComponent<BuildingItem>();
        DestroyInspector();
        ResetMiscLogs();
        UpdateStation();
    }
    // RESET - Called on instantiation and reset button clicked
    public void ResetStation()
    {
        ResetMiscLogs();
        DestroyInspector();
        DestroyWorkable();
        UpdateStation();
    }
    public void ResetMiscLogs()
    {
        // Reset miscLogs list
        if (miscLogs != null && easyPlace != null)
        {
            foreach (BuildingItem child in miscLogs)
            {
                child?.gameObject.transform.SetPositionAndRotation(new Vector3(easyPlace.transform.position.x, easyPlace.transform.position.y, easyPlace.transform.position.z + 8), easyPlace.transform.rotation);
            }
        }
        // Move Misc Logs
        miscLogs?.Clear();
        miscLogs?.TrimExcess();
    }
    public void DestroyInspector()
    {
        // Reset inspector
        Destroy(inspector?.gameObject);
        inspector = null;
    }
    public void DestroyWorkable()
    {
        // Reset workable
        Destroy(workable?.gameObject);
        workable = null;
    }
}