using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditorInternal;
using System.IO;

public class GameManager : MonoBehaviour
{
    [SerializeField] [Range (1f, 5f)] private float timescale = 1f;
    [Space]
    [SerializeField] private NewPlaneAI plane;
    [SerializeField] private GameObject planeObj;
    [SerializeField] private GameObject aIPlanePrefab;
    [SerializeField] public int populationSize = 50;
    [SerializeField] private List<NeuralNetworkFeedForward> nets;
    [SerializeField] public int generationNumber = 0;
    [SerializeField] private List<NewPlaneAI> aIPlanesList;
    [SerializeField] private GameObject planeTarget;
    [SerializeField] public float closestDistance;
    [Space]
    [Header("Text Stuff")]
    [SerializeField] private UnityEngine.UI.Text genText; 
    private GameObject fitnessTextClone;
    [SerializeField] private GameObject fitnessTextPrefab;
    [SerializeField] private GameObject fitnessTextContainer;

    private bool isTraning = false;
    private float planeXLocation;

    [Header("GenCard Variables")]
    public int numGoalHits = 0; //Value to be changed by BulletHitScript to keep track of shots on target
    public int shotsFired = 0;
    public bool goal;
    private List<NeuralNetworkFeedForward> netsForGenCard;

    [SerializeField] private int[] layers = new int[] { 11, 3, 3 }; 
    void Start()
    {
        //Debug.LogError(layers.Length);
        //for (int i = 0; i < layers.Length; i++)
            //Debug.LogError("Layer " + i + ": " + layers[i].ToString());
        InitPlaneNeuralNetworks();
        planeXLocation = planeObj.transform.position.x;
        
    }
    private void Timer()
    {
        isTraning = false;
    }
    void Update()
    {
        if (Time.timeScale != timescale)
            Time.timeScale = timescale;

        
        if (isTraning == false)
        {
            if (generationNumber == 0)
            {
                InitPlaneNeuralNetworks();
                genText.text = $"Generation: {generationNumber}";
            }
            else
            {
                netsForGenCard = new List<NeuralNetworkFeedForward>();
                for (int i = 0; i < populationSize; i++)
                {
                    netsForGenCard.Add(aIPlanesList[i].net);
                }
                nets.Sort();
                nets[populationSize - 1].Save("Assets/Save.txt");
                for (int i = 0; i < populationSize / 2; i++)
                {
                    nets[i] = new NeuralNetworkFeedForward(nets[i + (populationSize / 2)]);
                    nets[i].Mutate();

                    nets[i + (populationSize / 2)] = new NeuralNetworkFeedForward(nets[i + (populationSize / 2)]); //too lazy to write a reset neuron matrix values method....so just going to make a deepcopy lol
                }

                //for (int i = 0; i < populationSize; i++)
                //{
                //    nets[i].SetFitness(0f);
                //}
            }
            
            genText.text = $"Generation: {generationNumber}";

            isTraning = true;
            Invoke("Timer", 20f);
            GenerationCard("Assets/GenerationCard.txt");
            CreatePlaneBodies();
           generationNumber++;
        }
        DisplayFitness();
    }
    public void GenerationCard(string path)//this is used for saving the biases and weights within the network to a file.
    {
        if (File.Exists("Assets/GenerationCard.txt") && generationNumber > 0)
        {
            StreamWriter writer = new StreamWriter(path, true);
            float avgFitness = 0;
            float temp = 0;
            int numAlive = 0;
            foreach (var varNet in netsForGenCard)
            {
                //int count = 0;
                //temp += aIPlanesList[count].fitness;
                
                //varNet = aIPlanesList[count].net;
                temp += varNet.GetFitness();
                avgFitness = temp / populationSize;
            }
            foreach (var plane in aIPlanesList)
            {
                if (plane.gameObject.activeInHierarchy)
                    numAlive++;
            }
            writer.WriteLine($"Generation: {generationNumber-1} \n Avg Fitness = {avgFitness} \n" +
                $" Shots on Target/Shots Fired = {numGoalHits}/{shotsFired} \n Alive: {numAlive}/{populationSize}");

            writer.Close();
            numGoalHits = 0;
            shotsFired = 0;
        }
        else
            File.Create(path).Close();
    }
    private void ClearConsole()
    {
        // This simply does "LogEntries.Clear()" the long way:
        var logEntries = System.Type.GetType("UnityEditor.LogEntries,UnityEditor.dll");
        var clearMethod = logEntries.GetMethod("Clear", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
        clearMethod.Invoke(null, null);
    }
    private void DisplayFitness()
    {
        if (fitnessTextContainer.transform.childCount == 0) 
        {
            nets.Sort();
            for (int i = 0; i < populationSize / 2; i++)
            {
                fitnessTextClone = Instantiate(fitnessTextPrefab, fitnessTextContainer.transform);
                fitnessTextClone.GetComponent<UnityEngine.UI.Text>().text = $"Plane {i + 1}: {nets[i].GetFitness()}";
                Destroy(fitnessTextClone, 30f);
            }
        }
        if (fitnessTextContainer.transform.childCount != 0)
            for (int i = 0; i < populationSize / 2; i++)
                fitnessTextContainer.transform.GetChild(i).GetComponent<UnityEngine.UI.Text>().text = $"Plane {i + 1}: {nets[i].GetFitness()}";
    }
    private void CreatePlaneBodies()
    {
        //ClearConsole();
        if (aIPlanesList != null)
        {
            for (int i = 0; i < aIPlanesList.Count; i++)
            {
                if(aIPlanesList[i] != null)
                    Destroy(aIPlanesList[i].gameObject);
            }

        }

        aIPlanesList = new List<NewPlaneAI>();
        planeXLocation = 30f;
        for (int i = 0; i < populationSize; i++)
        {
            NewPlaneAI plane = ((GameObject)Instantiate(aIPlanePrefab, new Vector3(planeXLocation, planeObj.transform.position.y, planeObj.transform.position.z), aIPlanePrefab.transform.rotation)).GetComponent<NewPlaneAI>();
            planeXLocation += 30f;
            plane.Init(nets[i], planeTarget.transform);
            plane.name = $"Plane: {i + 1}";
            aIPlanesList.Add(plane);

        }
        
    }
    void InitPlaneNeuralNetworks()
    {
        //population must be even, just setting it to 20 incase it's not
        if (populationSize % 2 != 0)
        {
            populationSize = 20;
        }

        nets = new List<NeuralNetworkFeedForward>();
        

        for (int i = 0; i < populationSize; i++)
        {
            NeuralNetworkFeedForward net = new NeuralNetworkFeedForward(layers);
            net.Load("Assets/Save.txt");
            net.Mutate();
            nets.Add(net);
        }
    }

    public void RemoveFromList(string name, GameObject gameObject)
    {
        for (int i = 0; i < populationSize; i++)
        {
            if (name.Contains($"{i}"))
            {
                aIPlanesList.Remove(gameObject.GetComponent<NewPlaneAI>());
                //aIPlanesList[i] = null;
                nets.Remove(gameObject.GetComponent<NewPlaneAI>().net);
                Destroy(gameObject);
            }
        }

    }
}
