using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aimcone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other != this && (other.tag == "Enemy" || other.tag == "Player") && other != null)
            try
            {
                Debug.Log(gameObject.transform.parent.name + " is looking at: " + other.gameObject.transform.parent.name + "'s " + other.gameObject.name);
            }
            catch(NullReferenceException e)
            {
                Debug.LogWarning("Error other: " + other + "\n" + e.StackTrace);
                Debug.LogWarning("Error this: " + this + "\n" + e.StackTrace);
            }

            
    }
}
