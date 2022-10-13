using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsGrounded : MonoBehaviour
{
    private NeuralNetworkFeedForward net;
    private void Start()
    {
        //this.net = GetComponentInParent<PlaneAI2>().net;
    }
    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log($"Ground Hit: {collision.gameObject.name}");
        if (collision.gameObject.GetComponent<PlaneAI2>())
        {
            //NeuralNetworkFeedForward net = collision.gameObject.GetComponentInParent<PlaneAI2>().net;
            //net.SetFitness(-100f);
            try
            {
                if(collision.gameObject.GetComponentInParent<PlaneAI2>() == null)
                Debug.Log(collision.gameObject.name);
                collision.gameObject.GetComponentInParent<PlaneAI2>().net.SetFitness(-100);
                collision.gameObject.SetActive(false);
            }
            catch(Exception e)
            {
                Debug.Log(e);
            }
        }
    }
    //private void OnTriggerStay(Collider other)
    //{
    //    if (other.gameObject.name.Equals("Ground"))
    //    {
    //        net.SetFitness(-2f);
    //        gameObject.SetActive(false);
    //    }
    //}
}
