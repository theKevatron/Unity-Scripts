using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class CollisionEventSubscriber : MonoBehaviour
{
    private bool limitCollision = false;
    public static event System.Action<Collision> OnCollisionEvent;

    private void OnCollisionEnter(Collision collision)
    {
        if (limitCollision == false)
        {
            if (OnCollisionEvent != null && (collision.relativeVelocity.magnitude > 2.0f))
            {
                limitCollision = true;
                Debug.Log("OnCollisionEnter called");
                OnCollisionEvent(collision);
                Invoke("ResetLimitCollision", 0.5f);
            }
        }
    }

    public void ResetLimitCollision()
    {
        limitCollision = false;
    }
}
