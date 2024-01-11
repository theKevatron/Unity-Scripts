using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[ExecuteInEditMode]
public class DeactivatableQueue : MonoBehaviour
{
    public DeactivatableContainer[] stages;
    public int activeChildren;
    public int currentIndex;

    void OnEnable()
    {
        stages = null;
        stages = GetComponentsInChildren<DeactivatableContainer>();
        currentIndex = 0;

        foreach (DeactivatableContainer stage in stages)
        {
            stage.DisableDeactivatables();
        }

        stages[currentIndex].EnableDeactivatables();
        UpdateCount();
    }
    public void UpdateCount()
    {
        ActivateNextInList();

        activeChildren = 0;
        foreach (DeactivatableContainer stage in stages)
        {
            if (stage.gameObject.activeSelf == true)
            {
                activeChildren++;
            }
        }

        if (activeChildren <= 0)
            this.gameObject.SetActive(false);
    }

    public void ActivateNextInList()
    {
        if (stages?[currentIndex].gameObject.activeSelf == false)
        {
            currentIndex++;
            int lastIndex = stages.Length - 1;
            if (currentIndex <= lastIndex)
            {
                stages[currentIndex].EnableDeactivatables();
            }
        }
    }
}