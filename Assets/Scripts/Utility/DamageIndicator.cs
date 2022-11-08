using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageIndicator : MonoBehaviour
{
    [SerializeField] int numberOfHitsBeforeDamage;
    NewPlaneAI planeAI;
    int hits = 0;
    private void Start()
    {
        transform.GetComponentInChildren<ParticleSystem>().Pause();
        planeAI = gameObject.GetComponentInParent<NewPlaneAI>();
    }
    private void OnTriggerEnter(Collider other)
    {
        hits++;
        if (hits >= numberOfHitsBeforeDamage)
        {
            if (planeAI != null)
            {
                planeAI.Kill();
            }
            transform.GetComponentInChildren<ParticleSystem>().Play();
        }

        
    }
}
