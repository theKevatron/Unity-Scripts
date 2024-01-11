using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[ExecuteInEditMode]
public class Deactivatable : MonoBehaviour
{
    public string[] definitions;
    public string[] targetTags;
    private bool deactivatableTriggered = false;

    [Header("Customizable Options")]
    public float requiredHitSpeed = 1.0f;
    public int HP = 1;
    public int HPSeparationLimit = 0;
    public int deactivationDelay = 5;
    public int mass = 5;

    [Header("Optional")]
    public DeactivatableContainer parent;
    public Collider trigger;
    public HintMaterials hintMaterials;
    public EasyFX easyFX;
    public PFX PFX;
    public SFX SFX;
    public GameObject spawnable;
    public int spawnCount = 0;

    // AUTO DEFINE
    void OnEnable()
    {
        DefineQualities();
    }
    public void Reset()
    {
        definitions = this.gameObject.name.Split(char.Parse("_"));
        targetTags = null;
        deactivatableTriggered = false;
        requiredHitSpeed = 1.0f;
        HP = 1;
        HPSeparationLimit = 0;
        deactivationDelay = 5;
        mass = 5;
        parent = transform.parent.gameObject.GetComponent<DeactivatableContainer>();
        trigger = GetComponent<BoxCollider>();
        hintMaterials = GetComponentInChildren<HintMaterials>();
        easyFX = FindObjectOfType<EasyFX>();
        PFX = PFX.Undefined;
        SFX = SFX.Undefined;
        spawnable = null;
    }
    public void DefineQualities()
    {
        Reset();
        foreach (string definition in definitions)
        {
            if (definition == "RockLarge")
            {
                targetTags = new string[1];
                targetTags[0] = "Pick";
                requiredHitSpeed = 3.0f;
                HP = 3;
                deactivationDelay = 0;
                mass = 30;
                PFX = PFX.Stone;
                SFX = SFX.StoneHit;
                spawnable = Resources.Load("RockSmall_X") as GameObject;
                spawnCount = 3;
            }
            else if (definition == "TreeSmallStanding")
            {
                targetTags = new string[1];
                targetTags[0] = "Axe";
                requiredHitSpeed = 3.0f;
                HP = 3;
                deactivationDelay = 0;
                mass = 60;
                PFX = PFX.Wood;
                SFX = SFX.WoodChop;
                spawnable = Resources.Load("TreeSmallFelled_") as GameObject;
                spawnCount = 1;
            }
            else if (definition == "TreeMediumStanding")
            {
                targetTags = new string[1];
                targetTags[0] = "Axe";
                requiredHitSpeed = 3.0f;
                HP = 3;
                deactivationDelay = 0;
                mass = 60;
                PFX = PFX.Wood;
                SFX = SFX.WoodChop;
                spawnable = Resources.Load("TreeMediumFelled_") as GameObject;
                spawnCount = 1;
            }
            else if (definition == "TreeLargeStanding")
            {
                targetTags = new string[1];
                targetTags[0] = "Axe";
                requiredHitSpeed = 3.0f;
                HP = 3;
                deactivationDelay = 0;
                mass = 60;
                PFX = PFX.Wood;
                SFX = SFX.WoodChop;
                spawnable = Resources.Load("TreeLargeFelled_") as GameObject;
                spawnCount = 1;
            }
            else if (definition == "Stump")
            {
                targetTags = new string[1];
                targetTags[0] = "Axe";
                requiredHitSpeed = 2.0f;
                HP = 3;
                deactivationDelay = 0;
                PFX = PFX.Wood;
                SFX = SFX.WoodChop;
            }
            else if (definition == "Branch")
            {
                targetTags = new string[1];
                targetTags[0] = "Axe";
                PFX = PFX.Wood;
                SFX = SFX.WoodChop;
            }
            else if (definition == "Top")
            {
                targetTags = new string[1];
                targetTags[0] = "Axe";
                requiredHitSpeed = 2.0f;
                HP = 3;
                mass = 30;
                PFX = PFX.Wood;
                SFX = SFX.WoodChop;
            }
            else if (definition == "Bark")
            {
                targetTags = new string[1];
                targetTags[0] = "BarkSpud";
                PFX = PFX.Wood;
                SFX = SFX.BarkScrape;
            }
            else if (definition == "Hewable1")
            {
                targetTags = new string[1];
                targetTags[0] = "Adze";
                requiredHitSpeed = 2.0f;
                mass = 10;
                PFX = PFX.Wood;
                SFX = SFX.WoodChop;
            }
            else if (definition == "Hewable2")
            {
                targetTags = new string[1];
                targetTags[0] = "Adze";
                requiredHitSpeed = 2.0f;
                PFX = PFX.Wood;
                SFX = SFX.WoodChop;
            }
            else if (definition == "Notch")
            {
                targetTags = new string[1];
                targetTags[0] = "Adze";
                requiredHitSpeed = 2.0f;
                mass = 10;
                PFX = PFX.Wood;
                SFX = SFX.WoodChop;
            }
        }
    }

    public void Disable()
    {
        if (hintMaterials != null)
            hintMaterials.enabled = false;
        if (trigger != null)
            trigger.enabled = false;
        this.enabled = false;
    }
    public void Enable()
    {
        if (hintMaterials != null)
            hintMaterials.enabled = true;
        if (trigger != null)
            trigger.enabled = true;
        this.enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (deactivatableTriggered == false)
        {
            if (other.gameObject.layer == 8)
            {
                if (other.CompareTag("PlayerRangeTrigger"))
                {
                    hintMaterials?.ActivateMaterialsHintNotches();
                }
            }
            else if (other.gameObject.layer == 20)
            {
                if (targetTags != null)
                {
                    foreach (string child in targetTags)
                    {
                        if (other.gameObject.CompareTag(child))
                        {
                            if (other.GetComponent<Rigidbody>().velocity.magnitude > requiredHitSpeed)
                            {
                                // Decrement
                                DecrementHP();
                            }
                        }
                    }
                }
            }
        }    
    }
    private void OnTriggerExit(Collider other)
    {
        if (deactivatableTriggered == false)
        {
            if (other.gameObject.layer == 8)
            {
                if (other.CompareTag("PlayerRangeTrigger"))
                {
                    hintMaterials?.ActivateMaterialsOriginal();
                }
            }
        }
    }
    public void DecrementHP()
    {
        if (HP > 0)
        {
            HP--;
            easyFX?.PlayPFX((int)PFX, transform.position, transform.rotation, 1);
            easyFX?.PlaySFX((int)SFX, transform.position, 1);
        }
        if (HP <= HPSeparationLimit)
        {
            Rigidbody m_Rigidbody = gameObject.AddComponent<Rigidbody>();
            if (m_Rigidbody != null)
                m_Rigidbody.mass = mass;
            hintMaterials?.ActivateMaterialsOriginal();
        }
        if (HP <= 0)
        {
            Invoke("DeactivateObject", deactivationDelay);
            deactivatableTriggered = true;
        }
    }
    private void DeactivateObject()
    {
        this.gameObject.SetActive(false);
        if (spawnable != null)
        {
            for (int i = 0; i < spawnCount; i++)
                Instantiate(spawnable, gameObject.transform.position, gameObject.transform.rotation);
        }
        parent?.UpdateCount();
    }
}