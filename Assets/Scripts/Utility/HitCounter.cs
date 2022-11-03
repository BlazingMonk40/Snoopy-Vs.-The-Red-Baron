using UnityEngine;

public class HitCounter : MonoBehaviour
{
    [Tooltip("Number of times this object is allowed to be hit.")] [SerializeField] private int hitPoints;

    [Tooltip("Number of times this object has been hit.")] [SerializeField] private int timesHit;

    [Tooltip("Death Animation.")] [SerializeField] private Animation deathAnimation;

    [SerializeField] private GameManager manager;

    //[Tooltip("Death Particle Effect.")] [SerializeField] private Parit
    private void OnCollisionEnter(Collision collision)
    {
        //if (collision.gameObject.name.Equals("Ground"))
        //    DestroyThisObject();
        if (collision.gameObject.name.Equals("Bullet(Clone)"))
            timesHit++;
        if (timesHit > hitPoints)
            if (gameObject.name.Equals("Cube"))
                Destroy(gameObject);
    }
    private void DestroyThisObject()
    {
        if (gameObject.GetComponent<PlaneAI2>())
        {
            gameObject.SetActive(false);
            gameObject.GetComponent<PlaneAI2>().net.AddFitness(-10000f);
        }
        //manager = FindObjectOfType<GameManager>();
        //if (gameObject.GetComponent<PlaneAI2>())
        //{
        //    manager.RemoveFromList(gameObject.name, gameObject);
        //    GetComponent<PlaneAI2>().net.AddFitness(-1000f);
        //    //gameObject.GetComponent<PlaneAI2>().isDead = true;
        //}
    }
}
