using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunScript : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float fireRate;
    private float nextFireTime;
    private int gunCount;
    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
    public void Fire(string name)
    {
        gameManager.shotsFired += 1;
        if(Time.time > nextFireTime)
        {
            GameObject newBullet = Instantiate(bulletPrefab, transform.GetChild(gunCount % 2).position, transform.rotation);
            newBullet.GetComponent<Rigidbody>().velocity = transform.forward * 300;
            newBullet.gameObject.name = name;
            nextFireTime = Time.time + fireRate;
            gunCount++;
            StartCoroutine(TurnOnCollider(newBullet.GetComponent<BoxCollider>()));
            Destroy(newBullet, 20f);
            
        }
    }
    private IEnumerator TurnOnCollider(BoxCollider collider)
    {
        yield return new WaitForSeconds(.2f);
        collider.enabled = true;
    }
}
