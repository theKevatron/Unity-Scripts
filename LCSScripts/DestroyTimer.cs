using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class DestroyTimer : MonoBehaviour
{
    public bool timerStarted = false;
    public float timeToDestroy = 30.0f;
    public float timer;

    void Start()
    {
        this.enabled = false;
        timer = timeToDestroy;
    }
    private void Update()
    {
        if (timerStarted == true)
        {
            timer -= Time.deltaTime;
        }
        CheckTimer();
    }
    public void EnableScript()
    {
        this.enabled = true;
    }
    public void CheckTimer()
    {
        if (timer <= 0.0f)
        {
            Destroy(this.gameObject);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 8)
        {
            if (other.gameObject.CompareTag("PlayerRangeTrigger"))
            {
                timerStarted = false;
                timer = timeToDestroy;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 8)
        {
            if (other.gameObject.CompareTag("PlayerRangeTrigger"))
            {
                timerStarted = true;
            }
        }
    }
}
