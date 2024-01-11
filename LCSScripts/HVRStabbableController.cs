using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HurricaneVR.Framework.Core.Stabbing
{
    [DisallowMultipleComponent]
    public class HVRStabbableController : MonoBehaviour
    {
        HVRStabbable stabbableComponent;
        BuildingItem buildingItem;

        void Start()
        {
            stabbableComponent = GetComponent<HVRStabbable>();
            buildingItem = GetComponent<BuildingItem>();
        }

        private void Update()
        {
            AttemptActivateStabbable();
        }

        private void AttemptActivateStabbable()
        {
            if (buildingItem?.delimbed == true || (buildingItem.status == Status.Full && buildingItem.type == Type.Board && buildingItem.finished))
            {
                if (stabbableComponent != null)
                    stabbableComponent.enabled = true;
                if (buildingItem?.splittable != null)
                    buildingItem.splittable.enabled = true;
            }
            else
            {
                if (stabbableComponent != null)
                    stabbableComponent.enabled = false;
                if (buildingItem?.splittable != null)
                    buildingItem.splittable.enabled = false;
            }
        }
    }
}
