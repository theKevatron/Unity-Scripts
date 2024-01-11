using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

[DisallowMultipleComponent]
public class Cart : MonoBehaviour
{
    public bool easyPlaceOn = true;
    public EasyPlace easyPlace;

    private BuildingItem inspector;
    public List<BuildingItem> transportables = new List<BuildingItem>();

    [Header("UI Info")]
    public TextMeshProUGUI cartCount;
    public string textDescription = "Logs In Cart: ";
    public float itemCount = 0;

    public Transform logYard;

    void Start()
    {
        TryGetEasyPlace();

        if (cartCount == null)
            cartCount = this.GetComponentInChildren<TextMeshProUGUI>();

        if (cartCount == null)
            cartCount = GameObject.Find("Text_CartCount").GetComponent<TextMeshProUGUI>();

        if (cartCount != null)
            cartCount.text = textDescription + itemCount;

        if (logYard == null)
            logYard = GameObject.FindObjectOfType<Storage>().GetComponent<Transform>();
    }

    private void TryGetEasyPlace()
    {
        if (easyPlaceOn == true)
        {
            if (easyPlace == null)
                easyPlace = GetComponentInChildren<EasyPlace>();
            if (easyPlace == null)
                easyPlace = GameObject.Find("EasyPlace_Cart").GetComponent<EasyPlace>();
            if (easyPlace == null)
            {
                Debug.Log("No Easyplace Component found, default setting will not allow EasyPlace! - LogYard.cs, Start()");
                easyPlaceOn = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 20)
        {
            if (other.gameObject.CompareTag("BuildingItem"))
            {
                inspector = other.gameObject.GetComponent<BuildingItem>();
                if (inspector?.delimbed == true || inspector?.rockSmall == true)
                {
                    transportables.Add(other.gameObject.GetComponent<BuildingItem>());
                    if (easyPlace != null)
                        other.transform.SetParent(easyPlace.transform);
                    RemoveNull();
                    itemCount = transportables.Count;
                    cartCount.text = textDescription + itemCount;

                    if (inspector?.isTransportable == true && inspector?.isHeld == false)
                    {
                        // Easy place can be removed
                        if (easyPlaceOn == true)
                        {
                            easyPlace.PlaceGameObject(other);
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
                if (inspector?.delimbed == true || inspector?.rockSmall == true)
                {
                    transportables.Remove(other.gameObject.GetComponent<BuildingItem>());
                    RemoveNull();
                    itemCount = transportables.Count;
                    cartCount.text = textDescription + itemCount;
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
                if (inspector?.delimbed == true || inspector.rockSmall == true)
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

    public void RemoveNull()
    {
        for (int i = transportables.Count - 1; i >= 0; i--)
        {
            if (transportables[i] == null)
            {
                transportables.RemoveAt(i);
            }
        }
    }

    public void TransportLogs()
    {
        if (transportables != null)
        {
            foreach (BuildingItem child in transportables)
            {
                if (child != null)
                {
                    child.gameObject.transform.position = logYard.transform.position;
                    child.gameObject.transform.rotation = logYard.transform.rotation;
                }
            }

            inspector = null;
        }
    }

}
