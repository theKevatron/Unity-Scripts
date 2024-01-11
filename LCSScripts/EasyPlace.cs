using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class EasyPlace : MonoBehaviour
{
    public Transform targetLocation;

    private void Start()
    {
        if (targetLocation == null)
            targetLocation = this.gameObject.transform;
    }

    public void PlaceGameObject(Collider other)
    {
        if (targetLocation != null)
        {
            other.gameObject.transform.position = targetLocation.transform.position;
            other.gameObject.transform.rotation = targetLocation.transform.rotation;
        }
    }
}
