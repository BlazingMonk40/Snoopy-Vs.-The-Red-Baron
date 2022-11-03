using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlphaBeta
{
    private GameObject plane;
    private Vector3 state;

    public AlphaBeta(string planeName)
    {
        plane = GameObject.Find(planeName);
        state = plane.transform.position;
    }
    public void setState(Vector3 state)
    {
        this.state = state;
    }
}