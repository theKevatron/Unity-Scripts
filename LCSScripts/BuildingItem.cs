using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[ExecuteInEditMode]
public class BuildingItem : MonoBehaviour
{
    [SerializeField]
    public string[] definitions;
    private Rigidbody Rigidbody;

    [Header("Transform Area Info")]
    public bool isHeld = false;
    public bool isTransportable = false;
    private Cart cart;
    private Storage storage;
    private WorkStation workStation;
    public Splittable splittable;

    [Header("Rock Traits")]
    public bool rockSmall = false;
    public bool rockLarge = false;

    [Header("Log Traits")]
    public Status status = Status.Undefined;
    public Quality quality = Quality.Undefined;
    public Style style = Style.Undefined;
    public Type type = Type.Undefined;

    [Header("Log Stage 1 - Raw")]
    public DeactivatableContainer delimbablesContainer;
    public bool delimbed = false;
    [Header("Log Stage 2 - Workable")]
    public DeactivatableQueue workables;
    public bool finished = false;
    //public ParticleSystem actionHint;

    void OnEnable()
    {
        DefineTraits();
    }
    public void Reset()
    {
        definitions = this.gameObject.name.Split(char.Parse("_"));
        Rigidbody = GetComponent<Rigidbody>();
        isHeld = false;
        isTransportable = false;
        cart = null;
        storage = null;
        workStation = null;
        splittable = null;
        rockSmall = false;
        rockLarge = false;
        status = Status.Undefined;
        quality = Quality.Undefined;
        style = Style.Undefined;
        type = Type.Undefined;
        delimbablesContainer = null;
        delimbed = false;
        workables = null;
        finished = false;
    }
    public void DefineTraits()
    {
        Reset();
        foreach (string definition in definitions)
        {
            if (definition == "RockSmall")
                rockSmall = true;
            else if (definition == "RockLarge")
                rockLarge = true;
            else if (definition == "Full")
            {
                status = Status.Full;

                splittable = this.gameObject.AddComponent<Splittable>();
                if (splittable == null)
                    splittable = GetComponent<Splittable>();
                splittable.enabled = false;
                // Full Logs require 'HVRStabbableController(enabled)','HVRStabbable(disabled)', and 'Splittable(disabled)' Components
            }
            else if (definition == "Split")
                status = Status.Split;

            if (definition == "Raw")
            {
                // Get delimblesContainer component
                delimbablesContainer = GetComponentInChildren<DeactivatableContainer>();
                if (delimbablesContainer != null || delimbablesContainer?.enabled == true)
                {
                    //actionHint = GetComponentInChildren<ParticleSystem>();
                    //actionHint?.Play();
                }
                else if (delimbablesContainer == null)
                    delimbed = true;
            }
            if (definition == "Workable")
            {
                // Get workablesContainer component
                workables = GetComponentInChildren<DeactivatableQueue>();
                if (workables != null || workables?.enabled == true)
                {
                    //actionHint = GetComponentInChildren<ParticleSystem>();
                    //actionHint?.Play();
                }
                else if (workables == null)
                    finished = true;
            }

            if (definition == "Pulp")
            {
                quality = Quality.Pulp;
                transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            }
            else if (definition == "Saw")
            {
                quality = Quality.Saw;
                transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
            }
            else if (definition == "Veneer")
            {
                quality = Quality.Veneer;
                transform.localScale = new Vector3(1f, 1f, 1f);
            }

            if (definition == "Square")
                style = Style.Square;
            else if (definition == "D")
                style = Style.D;
            else if (definition == "Round")
                style = Style.Round;

            if (definition == "1Sill")
                type = Type.Sill;
            else if (definition == "2Wall")
                type = Type.Wall;
            else if (definition == "3Top")
                type = Type.Top;
            else if (definition == "4Gable")
                type = Type.Gable;
            else if (definition == "5Ridge")
                type = Type.Ridge;
            else if (definition == "6Board")
                type = Type.Board;
        }
    }

    private void Update()
    {
        UpdateCompletionStatus();
    }
    private void UpdateCompletionStatus()
    {
        if (delimbablesContainer?.gameObject.activeSelf == false)
            delimbed = true;
        if (delimbed == true)
        {
            //if (actionHint != null)
                //Destroy(actionHint);
        }
        if (workables?.gameObject.activeSelf == false)
            finished = true;
        if (finished == true)
        {
            if (Rigidbody != null)
                Rigidbody.constraints = RigidbodyConstraints.None;
            //if (actionHint != null)
                //Destroy(actionHint);
        }
    }
    public void OnGrabConstraints()
    {
        isHeld = true;
    }
    public void OnReleaseConstraints()
    {
        isHeld = false;
        AttemptToActivateAreaProcess();
    }
    private void AttemptToActivateAreaProcess()
    {
        if (isTransportable == true && isHeld == false)
        {
            cart?.OnRelease(this.gameObject.GetComponent<Collider>());
            storage?.OnRelease(this.gameObject.GetComponent<Collider>());
            workStation?.OnRelease(this.gameObject.GetComponent<Collider>());
            if (Rigidbody != null)
            {
                Rigidbody.constraints = RigidbodyConstraints.FreezeAll;
                Rigidbody.constraints = RigidbodyConstraints.None;
            }
            isTransportable = false;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        // If other gameObject layer = "TransformArea", (stations, transportCart, storage)
        if (other.gameObject.layer == 12)
        {
            if (other.gameObject.CompareTag("Cart"))
            {
                cart = other.GetComponent<Cart>();
                isTransportable = true;
            }
            else if (other.gameObject.CompareTag("Storage"))
            {
                storage = other.GetComponent<Storage>();
                isTransportable = true;
            }
            else if (other.gameObject.CompareTag("WorkStation"))
            {
                workStation = other.GetComponent<WorkStation>();
                isTransportable = true;
            }
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 12)
        {
            AttemptToActivateAreaProcess();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 12)
        {
            isTransportable = false;

            cart = null;
            storage = null;
            workStation = null;
        }
    }
}