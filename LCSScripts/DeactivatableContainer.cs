using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[ExecuteInEditMode]
public class DeactivatableContainer : MonoBehaviour
{
    public Deactivatable[] deactivatables;
    public int active;
    [Header("Optional")]
    public DeactivatableQueue parent;

    void OnEnable()
    {
        deactivatables = null;
        deactivatables = GetComponentsInChildren<Deactivatable>();

        parent = this.transform.parent.gameObject.GetComponent<DeactivatableQueue>();

        UpdateCount();
    }

    public void UpdateCount()
    {
        active = 0;
        foreach (Deactivatable deactivatable in deactivatables)
        {
            if (deactivatable.gameObject.activeSelf == true)
            {
                active++;
            }
        }

        if (active <= 0)
            this.gameObject.SetActive(false);

        parent?.UpdateCount();
    }

    public void DisableDeactivatables()
    {
        foreach (Deactivatable deactivatable in deactivatables)
        {
            deactivatable.Disable();
        }
    }
    public void EnableDeactivatables()
    {
        foreach (Deactivatable deactivatable in deactivatables)
        {
            deactivatable.Enable();
        }
    }
}