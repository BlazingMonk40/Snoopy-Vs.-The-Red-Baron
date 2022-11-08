using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewPlaneAI : MonoBehaviour
{

    private float currentDistance;
    public LayerMask raycastMask;
    private Rigidbody rigid;
    public Vector3 rotation;
    public float forceMult = 1000f;

    public GameManager manager;
    public NeuralNetworkFeedForward net;
    public GameObject ground;

    public float altitude;
    public float currentPitchRotation;
    public float currentYawRotation;
    public float currentRollRotation;
    public float distanceToTarget;
    

    /// <summary>
    /// Deactivates the plane and sets fitness to neg infinity
    /// </summary>
    internal void Kill()
    {
        if (gameObject.activeInHierarchy)
        {
            net.SetFitness(Mathf.NegativeInfinity);

            gameObject.SetActive(false);
        }
    }

    public GameObject target;
    public float rayDistance = 10f;
    public int feedForwardCount;

    [Header("AI Outputs")]
    [SerializeField] [Range(0f, 120f)] private float thrust = 0f;
    [SerializeField] [Range(-1f, 1f)] private float pitch = 0f;
    [SerializeField] [Range(-1f, 1f)] private float yaw = 0f;
    [SerializeField] [Range(-1f, 1f)] private float roll = 0f;

    [Header("Input/Output Arrays")]
    [Tooltip("\nElement 5: Altitude\nElement 6: Current Pitch Rotation\nElement 7: Current Yaw Rotation\nElement 8: Current Roll Rotation\nElement 9: Angle to Target\nElement 10: Distance to target")]
    [SerializeField] private float[] input = new float[8];
    [SerializeField] private float[] output;
    

    public float Thrust { set { thrust = Mathf.Clamp(value, 0f, 120f); } get { return thrust; } }
    public float Pitch { set { pitch = Mathf.Clamp(value, -1f, 1f); } get { return pitch; } }
    public float Yaw { set { yaw = Mathf.Clamp(value, -1f, 1f); } get { return yaw; } }
    public float Roll { set { roll = Mathf.Clamp(value, -1f, 1f); } get { return roll; } }
    public float getAltitude { get { return Vector3.Distance(transform.position, ground.transform.position); } }
    public float getCurrentPitchRotation { get { return transform.rotation.x; } }
    public float getCurrentYawRotation { get { return transform.rotation.y; } }
    public float getCurrentRollRotation { get { return transform.rotation.z; } }
    
    public float getAngleToTarget { get { return Vector3.Angle(transform.position, target.transform.position); } }
    public float getDistanceToTarget { get { return Vector3.Distance(transform.position, target.transform.position); } }
    public float getFitness { get { return net.GetFitness(); } }
    //{ Standard Ai to Network variables
        public Vector3 spawnLocation;
        public Vector3 currentLocation;
    //}
    private bool increaseThrustPercentBool;
    public bool isDead = false;

    private void Start()
    {
        rigid = GetComponent<Rigidbody>();
        if (manager == null)
            manager = FindObjectOfType<GameManager>();
        spawnLocation = gameObject.transform.position;
        currentLocation = gameObject.transform.position;
    }
    public void Init(NeuralNetworkFeedForward net, Transform AITarget)
    {
        this.net = net;
    }
    private void Update()
    {
        currentLocation = gameObject.transform.position;

        currentDistance = getDistanceToTarget;
        if (currentDistance <= manager.closestDistance)
        {
            manager.closestDistance = currentDistance;
            Debug.LogWarning($"New Closest Distance: {manager.closestDistance}");
        }
            
        altitude = getAltitude;
        distanceToTarget = getDistanceToTarget;
        currentPitchRotation = transform.rotation.x;
        currentYawRotation = transform.rotation.y;
        currentRollRotation = transform.rotation.z;
        float prevDistance = getDistanceToTarget;
        IsCloser(prevDistance);

        if(increaseThrustPercentBool)
        {
            forceMult += 20f * Time.deltaTime;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="previousDist"></param>
    /// <returns></returns>
    private void IsCloser(float previousDist)
    {
        float currentDistance = getDistanceToTarget;
        if (currentDistance < manager.closestDistance)
            net.SetFitness(Mathf.Infinity);
    }
    private void FixedUpdate()
    {
        //the first 5 inputs are from ray casts
        input[5] = getAltitude;
        //input[6] = getCurrentPitchRotation;
        //input[7] = getCurrentYawRotation;
        //input[8] = getCurrentRollRotation;
        input[6] = getAngleToTarget;
        input[7] = getDistanceToTarget;
        
        #region RayCasting //Ray Drawing commented out
        //Test ray, points forward
        Vector3 forward = transform.TransformDirection(Vector3.forward) * 5;
        //Debug.DrawRay(transform.position, forward, Color.cyan);
        if (!isDead)
        {
            RaycastHit hit;
            Ray ray;
            Vector3[] newVector = new Vector3[3];
            Debug.DrawRay(transform.position, ge)
            for (int i = 0; i < 5; i++)//12 if [3]
            {
                //newVector[3] = Quaternion.AngleAxis(i * 30, transform.forward) * Quaternion.Euler(rotation) * -transform.up * rayDistance; //Point rays in a spread of 45 degrees, rotating around the z-axis (below)
                if (i < 5)
                {
                    newVector[0] = Quaternion.AngleAxis(i * 45 - 180, transform.up) * transform.right * rayDistance; //Point rays in a spread of 45 degrees, rotating around the y-axis
                    if (i >= 1 && i <= 3) //To avoid overlapping rays
                    {
                        newVector[1] = Quaternion.AngleAxis(i * 45 - 90, transform.forward) * transform.up * rayDistance; //Point rays in a spread of 45 degrees, rotating around the z-axis (above)
                        newVector[2] = Quaternion.AngleAxis(i * 45 - 90, transform.forward) * -transform.up * rayDistance; //Point rays in a spread of 45 degrees, rotating around the z-axis (above)
                    }
                }
                for (int j = 0; j < newVector.Length; j++)
                {
                    ray = new Ray(transform.position, newVector[j]);
                    //Debug.DrawRay(transform.position, newVector[j], Color.red);
                    if (Physics.Raycast(ray, out hit, rayDistance, raycastMask))
                    {
                        input[i] = (rayDistance - hit.distance) / rayDistance;//return distance, 1 being close
                    }
                    else
                    {
                        input[i] = 0;//if nothing is detected, will return 0 to network
                    }
                }
            }
        }
        #endregion

        #region Movement
        // 4 outputs  Forward Thrust, Pitch (-1, 1), Yaw (-1, 1), Roll (-1, 1)
         if (Vector3.Distance(spawnLocation, currentLocation) >= 100)
        {
            //Debug.Log("Starting distance surpassed: Calling Feed Forward");
            CallFeedFoward();
        }else
        {
            StandardAI();
        }
        
        #endregion
    } 
    private void StandardAI()
    {
        Thrust = 100f;
        rigid.AddRelativeForce(Vector3.forward * (Thrust * forceMult), ForceMode.Force);
        Pitch = .1f;
        if (Vector3.Distance(spawnLocation, currentLocation) >= 400)
        {
            Roll = .15f;
        }
        #region Increase Thrust
        if (forceMult < 1000f && !increaseThrustPercentBool)
        {
            increaseThrustPercentBool = true;
        }
        else
            increaseThrustPercentBool = false;
#endregion
    }
    private void IncreaseThrust()
    { 
    }
      
    private void CallFeedFoward()
    {
        output = net.FeedForward(input);
        feedForwardCount++;
        Thrust = 100f;/*output[0];*/
        Pitch = output[0];
        Yaw = output[1];
        Roll = output[2];
        if (rigid.velocity.magnitude >= 35f)
        {
            transform.Rotate(Pitch, 0, 0, Space.Self);
            transform.Rotate(0, Yaw, 0, Space.Self);
            transform.Rotate(0, 0, Roll, Space.Self);
        }
        rigid.AddRelativeForce(Vector3.forward * (Thrust * forceMult), ForceMode.Force);
    }
}
/*
 * for (int i = 0; i < 15; i++)
            {
                Vector3 newVector = Quaternion.AngleAxis(i * 45 - 90, new Vector3(0, 1, 0)) * transform.right;
                RaycastHit hit;
                Ray Ray = new Ray(transform.position, newVector);
                Debug.DrawRay(transform.position, Quaternion.AngleAxis(i * 45 - 180, new Vector3(0, 1, 0)) * transform.right, Color.red);
                if (Physics.Raycast(Ray, out hit, 100, raycastMask))
                {
                    input[i] = (100 - hit.distance) / 100;//return distance, 1 being close
                }
                else
                {
                    input[i] = 0;//if nothing is detected, will return 0 to network
                }
                newVector = Quaternion.AngleAxis(i * 45 - 90, new Vector3(0, 1, 0)) * transform.up;
                Ray = new Ray(transform.position, newVector);
                Debug.DrawRay(transform.position, Quaternion.AngleAxis(i * 45 - 90, new Vector3(0, 0, .5f)) * transform.up, Color.blue);
                if (Physics.Raycast(Ray, out hit, 100, raycastMask))
                {
                    input[i] = (100 - hit.distance) / 100;//return distance, 1 being close
                }
                else
                {
                    input[i] = 0;//if nothing is detected, will return 0 to network
                }
                newVector = Quaternion.AngleAxis(i * 45 - 90, new Vector3(0, 1, 0)) * -transform.up;
                Ray = new Ray(transform.position, newVector);
                Debug.DrawRay(transform.position, Quaternion.AngleAxis(i * 45 - 90, new Vector3(0, 0, 1)) * -transform.up, Color.green);
                if (Physics.Raycast(Ray, out hit, 100, raycastMask))
                {
                    input[i] = (100 - hit.distance) / 100;//return distance, 1 being close
                }
                else
                {
                    input[i] = 0;//if nothing is detected, will return 0 to network
                }
            }
 */
/*for (int i = 0; i < 3; i++)
            {
                if (i == 0) //Cast rays forward
                {
                    for (int j = 0; j < 5; j++)
                    {
                        Vector3 newVector = Quaternion.AngleAxis(j * 45 - 90, new Vector3(0, 1, 0)) * transform.right;
                        RaycastHit hit;
                        Ray Ray = new Ray(transform.position, newVector);
                        Debug.DrawRay(transform.position, Quaternion.AngleAxis(j * 45 - 180, new Vector3(0, 1, 0)) * transform.right, Color.red);
                        if (Physics.Raycast(Ray, out hit, 100, raycastMask))
                        {
                            input[i] = (100 - hit.distance) / 100;//return distance, 1 being close
                        }
                        else
                        {
                            input[i] = 0;//if nothing is detected, will return 0 to network
                        }
                    }
                }
                else if (i == 1) //Cast rays up
                {
                    for (int j = 0; j < 5; j++)
                    {
                        Vector3 newVector = Quaternion.AngleAxis(j * 45 - 90, new Vector3(0, 1, 0)) * transform.up;
                        RaycastHit hit;
                        Ray Ray = new Ray(transform.position, newVector);
                        Debug.DrawRay(transform.position, Quaternion.AngleAxis(j * 45 - 90, new Vector3(0, 0, .5f)) * transform.up, Color.blue);

                    }
                }
                else if (i == 2) //Cast rays down
                {
                    for (int j = 0; j < 5; j++)
                    {
                        Vector3 newVector = Quaternion.AngleAxis(j * 45 - 90, new Vector3(0, 1, 0)) * -transform.up;
                        RaycastHit hit;
                        Ray Ray = new Ray(transform.position, newVector);
                        Debug.DrawRay(transform.position, Quaternion.AngleAxis(j * 45 - 90, new Vector3(0, 0, 1)) * -transform.up, Color.green);
                    }
                }
            }*/