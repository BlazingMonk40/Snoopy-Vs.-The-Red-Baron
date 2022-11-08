using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHitScript : MonoBehaviour
{
    bool stop = false;
    private void Update()
    {
        if (!stop && GameObject.Find("GameManager").GetComponent<GameManager>().goal == true)
        {
            Debug.Log($"***GOAL HIT!*** {gameObject.name}");
            GameObject.Find("GameManager").GetComponent<GameManager>().numGoalHits += 1;
            stop = true;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag != "Scenery")
            Debug.Log($"Bullet Hit {collision.gameObject.name}");
        if(collision.gameObject.name.Equals("Goal") || GameObject.Find("GameManager").GetComponent<GameManager>().goal == true)
        {
            Debug.Log($"***GOAL HIT!*** {gameObject.name} - {GameObject.Find("GameManager").GetComponent<GameManager>().generationNumber}");
            GameObject.Find(gameObject.name).GetComponent<NeuralNetworkFeedForward>().AddFitness(50f);
            GameObject.Find("GameManager").GetComponent<GameManager>().numGoalHits += 1;
        }
    }
}
