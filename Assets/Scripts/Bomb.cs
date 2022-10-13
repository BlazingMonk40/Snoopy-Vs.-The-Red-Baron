using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{

    public float speed = 420;
    public int predictionStepsPerFrame = 6;
    public Vector3 bombVelocity;
    // Start is called before the first frame update
    void Start()
    {
        bombVelocity = this.transform.forward * speed;
    }

    // Update is called once per frame
    void Update()
    {

        Vector3 point1 = gameObject.transform.position;
        float stepSize = 1.0f / predictionStepsPerFrame;
        for (float step = 0; step < 1; step += stepSize)
        {
            bombVelocity += Physics.gravity * stepSize * Time.deltaTime;
            Vector3 point2 = point1 + bombVelocity * stepSize * Time.deltaTime;

            Ray ray = new Ray(point1, point2 - point1);
            if (Physics.Raycast(ray, (point2 - point1).magnitude))
            {
                Debug.Log("Hit");
            }
            point1 = point2;
        }
        gameObject.transform.position = point1;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 point1 = this.transform.position;
        Vector3 predictedBombVelocity = bombVelocity;
        float stepSize = 0.01f;
        for (float step = 0; step < 1; step += stepSize)
        {
            predictedBombVelocity += Physics.gravity * stepSize;
            Vector3 point2 = point1 + predictedBombVelocity * stepSize;

            Gizmos.DrawLine(point1, point2);
            point1 = point2;
        }
    }
}
