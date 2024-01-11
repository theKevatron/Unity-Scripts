using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[ExecuteInEditMode]
public class Tool : MonoBehaviour
{
    public string[] definitions;
    public Rigidbody Rigidbody;

    [Header("Tool Traits")]
    public bool axe = false;
    public bool hammer = false;
    public bool wedge = false;
    public bool barkSpud = false;
    public bool adze = false;
    public bool pick = false;

    [Header("Effects")]
    public EasyFX easyFX;
    public PFX PFX;
    public SFX SFX;

    [Header("Wedge Settings")]
    public Splittable splittable;
    public bool isFullyStabbed = false;

    private void OnEnable()
    {
        Define();
    }
    public void Reset()
    {
        definitions = this.gameObject.name.Split(char.Parse("_"));
        Rigidbody = GetComponent<Rigidbody>();
        axe = false;
        hammer = false;
        wedge = false;
        barkSpud = false;
        adze = false;
        pick = false;
        easyFX = FindObjectOfType<EasyFX>();
        PFX = PFX.Wood;
        SFX = SFX.WoodStab;
        splittable = null;
        isFullyStabbed = false;
}
    public void Define()
    {
        Reset();
        foreach (string definition in definitions)
        {
            if (definition == "Axe")
                axe = true;
            else if (definition == "Hammer")
                hammer = true;
            else if (definition == "Wedge")
                wedge = true;
            else if (definition == "BarkSpud")
                barkSpud = true;
            else if (definition == "Adze")
                adze = true;
            else if (definition == "Pick")
                pick = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 20)
        {
            if (wedge == true)
            {
                if (collision.gameObject.CompareTag("BuildingItem"))
                {
                    splittable = collision.gameObject.GetComponent<Splittable>();
                }
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 20)
        {
            if (wedge == true)
            {
                if (other.gameObject.CompareTag("BuildingItem"))
                {
                    splittable = other.gameObject.GetComponent<Splittable>();
                }

                if (other.gameObject.CompareTag("Hammer"))
                {
                    if (isFullyStabbed == true)
                    {
                        if (other.gameObject.GetComponent<Tool>().Rigidbody.velocity.magnitude >= 2)
                            splittable?.DecrementHitsToSplit();
                    }
                }
            }
        }
    }

    public void Stabbed()
    {
        Rigidbody.useGravity = false;
        easyFX.PlayPFX((int)PFX, transform.position, transform.rotation, 1);
        easyFX.PlaySFX((int)SFX, transform.position, 1);
    }
    public void UnStabbed()
    {
        isFullyStabbed = false;
        Rigidbody.useGravity = true;
    }
    public void FullyStabbed()
    {
        isFullyStabbed = true;
    }
}