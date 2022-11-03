using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystem : MonoBehaviour
{
    Transform position;
    [SerializeField] private int gridSize = 100;
    [SerializeField] private int gridStep = 10;
    [SerializeField] private int[][][] grid;
    // Start is called before the first frame update
    void Start()
    {

        position = transform;
        //StartCoroutine(logPositionOverTime(10));
        Grid_3D grid_3D = new Grid_3D(gameObject, 3, 3, 3, 100);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator logPositionOverTime(float seconds)
    {
        Debug.Log(gameObject.name + "'s position is: " + position.position);
        yield return new WaitForSeconds(seconds);
        StartCoroutine(logPositionOverTime(10));
    }
}
