using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageIndicator : MonoBehaviour
{
    [SerializeField] int numberOfHitsBeforeDamage;
    int hits = 0;
    private void Start()
    {
        transform.GetComponentInChildren<ParticleSystem>().Pause();
    }
    private void OnTriggerEnter(Collider other)
    {
        hits++;
        if (hits >= numberOfHitsBeforeDamage)
            transform.GetComponentInChildren<ParticleSystem>().Play();
        
    }
}
