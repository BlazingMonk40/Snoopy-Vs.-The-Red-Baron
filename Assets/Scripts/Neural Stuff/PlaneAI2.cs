using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneAI2 : MonoBehaviour
{
    public GameManager manager;
    public NeuralNetworkFeedForward net;
    public float fitness;
    public GameObject ground;
    private Transform target;

    public float[] inputs;   //position, velocity, orientation, bomb landing
    public float[] outputs;   //pitch and roll
    private int[] layers = new int[5] { 14, 72, 72, 72, 2 };
    public Vector3 position;
    public Vector3 velocity;
    public Vector3 rotation;
    public Vector3 bomb;

    [Range(-5f, 5f)] public float pitch;
    [Range(-5f, 5f)] public float roll;
    public Vector3 turnTorque;
    public float forceMult;

    Rigidbody rb;

    private bool fit1 = false, fit2 = false, fit3 = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        inputs = new float[14];
        outputs = new float[2];
        getInputs();

        net = new NeuralNetworkFeedForward(layers);
        
    }
    /** Initializes inputs and fills the input array
     */
    private void getInputs()
    {
        position = gameObject.transform.position;
        velocity = rb.velocity;
        rotation.x = gameObject.transform.rotation.x;
        rotation.y = gameObject.transform.rotation.y;
        rotation.z = gameObject.transform.rotation.z;

        inputs[0] = position.x;
        inputs[1] = position.y;
        inputs[2] = position.z;
        inputs[3] = velocity.x;
        inputs[4] = velocity.y;
        inputs[5] = velocity.z;
        inputs[6] = rotation.x;
        inputs[7] = rotation.y;
        inputs[8] = rotation.z;
        inputs[9] = bomb.x;
        inputs[10] = bomb.z;
        inputs[11] = target.transform.position.x;
        inputs[12] = target.transform.position.y;
        inputs[13] = target.transform.position.z;

    }
    public void Init(NeuralNetworkFeedForward net, Transform target)
    {
        this.net = net;
        this.target = target;
    }
    // Update is called once per frame
    void Update()
    {
        getInputs();
        outputs = net.FeedForward(inputs);

        //pitch = Mathf.Clamp(outputs[0], -5f, 5f);
        //roll = Mathf.Clamp(outputs[1], -5f, 5f);
        pitch = outputs[0];
        roll = outputs[1];
        rb.AddRelativeForce(Vector3.forward * (400) * forceMult, ForceMode.Force);
        //rb.velocity = 100f * transform.forward;
        rb.AddRelativeTorque(new Vector3(turnTorque.x * pitch,
                                                    0,
                                                    -turnTorque.z * roll) * forceMult,
                                                    ForceMode.Force);
        
        if(/*!fit1 && */Vector3.Distance(gameObject.transform.position, target.position) < 50f)
        {
            //Debug.Log($"Adding Fitness 10: {gameObject.name}");
            net.AddFitness(10f);
            fitness = net.GetFitness();
            fit1 = true;
        }
        //else if(!fit2 && Vector3.Distance(gameObject.transform.position, target.position) < 500f)
        //{
        //    Debug.Log($"Adding Fitness 5: {gameObject.name}");
        //    net.AddFitness(5f);
        //    fit2 = true;
        //}
        //else if (!fit3 && Vector3.Distance(gameObject.transform.position, target.transform.position) < 750f)
        //{
        //    Debug.Log($"Adding Fitness 2.5: {gameObject.name}");
        //    net.AddFitness(2.5f);
        //    fit3 = true;
        //}
        if (/*!fit3 && */Vector3.Angle(transform.forward, target.position - transform.position) < 10f)
        {
            //Debug.Log($"Adding Fitness for direction: {gameObject.name}");
            net.AddFitness(5f);
            fitness = net.GetFitness();
            fit3 = true;
        }
        if(Vector3.Angle(transform.forward, target.position - transform.position) < 1f)
        {
            //Debug.Log($"Adding Fitness for firing: {gameObject.name}");
            gameObject.GetComponentInChildren<GunScript>().Fire();
            net.AddFitness(20);
            fitness = net.GetFitness();
        }
    }
}
