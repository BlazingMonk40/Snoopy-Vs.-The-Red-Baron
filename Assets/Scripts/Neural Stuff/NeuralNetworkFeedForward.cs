﻿
using System.Collections.Generic;
using System;
using System.IO;

/// <summary>
/// Neural Network C# (Unsupervised)
/// </summary>
public class NeuralNetworkFeedForward : IComparable<NeuralNetworkFeedForward>
{
    public int[] layers; //layers
    private float[][] neurons; //neuron matix
    private float[][] biases;
    private float[][][] weights; //weight matrix
    private float fitness; //fitness of the network


    /// <summary>
    /// Initilizes and neural network with random weights
    /// </summary>
    /// <param name="layers">layers to the neural network</param>
    public NeuralNetworkFeedForward(int[] layers)
    {
        //deep copy of layers of this network 
        this.layers = new int[layers.Length];
        for (int i = 0; i < layers.Length; i++)
        {
            this.layers[i] = layers[i];
        }


        //generate matrix
        InitNeurons();
        InitBiases();
        InitWeights();
    }

    /// <summary>
    /// Deep copy constructor 
    /// </summary>
    /// <param name="copyNetwork">Network to deep copy</param>
    public NeuralNetworkFeedForward(NeuralNetworkFeedForward copyNetwork)
    {
        this.layers = new int[copyNetwork.layers.Length];
        for (int i = 0; i < copyNetwork.layers.Length; i++)
        {
            this.layers[i] = copyNetwork.layers[i];
        }

        InitNeurons();
        InitBiases();
        InitWeights();
        CopyWeights(copyNetwork.weights);
        CopyBiases(copyNetwork.biases);
    }

    private void CopyWeights(float[][][] copyWeights)
    {
        for (int i = 0; i < weights.Length; i++)
        {
            for (int j = 0; j < weights[i].Length; j++)
            {
                for (int k = 0; k < weights[i][j].Length; k++)
                {
                    weights[i][j][k] = copyWeights[i][j][k];
                }
            }
        }
    }
    private void CopyBiases(float[][] copyBiases)
    {
        for (int i = 0; i < biases.Length; i++)
        {
            for (int j = 0; j < biases[i].Length; j++)
            {
                biases[i][j] = copyBiases[i][j];
            }
        }
    }

    /// <summary>
    /// Create neuron matrix
    /// </summary>
    private void InitNeurons()
    {
        //Neuron Initilization
        List<float[]> neuronsList = new List<float[]>();

        for (int i = 0; i < layers.Length; i++) //run through all layers
        {
            neuronsList.Add(new float[layers[i]]); //add layer to neuron list
        }

        neurons = neuronsList.ToArray(); //convert list to array
    }

    /// <summary>
    /// Create Bias matrix
    /// initializes and populates array for the biases being held within the network.
    /// </summary>
    private void InitBiases()
    {
        List<float[]> biasList = new List<float[]>();
        for (int i = 0; i < layers.Length; i++)
        {
            float[] bias = new float[layers[i]];
            for (int j = 0; j < layers[i]; j++)
            {
                bias[j] = UnityEngine.Random.Range(-0.5f, 0.5f);
            }
            biasList.Add(bias);
        }
        biases = biasList.ToArray();
    }

    /// <summary>
    /// Create weights matrix.
    /// </summary>
    private void InitWeights()
    {

        List<float[][]> weightsList = new List<float[][]>(); //weights list which will later will converted into a weights 3D array

        //itterate over all neurons that have a weight connection
        for (int i = 1; i < layers.Length; i++)
        {
            List<float[]> layerWeightsList = new List<float[]>(); //layer weight list for this current layer (will be converted to 2D array)

            int neuronsInPreviousLayer = layers[i - 1];

            //itterate over all neurons in this current layer
            for (int j = 0; j < neurons[i].Length; j++)
            {
                float[] neuronWeights = new float[neuronsInPreviousLayer]; //neruons weights

                //itterate over all neurons in the previous layer and set the weights randomly between 0.5f and -0.5
                for (int k = 0; k < neuronsInPreviousLayer; k++)
                {
                    //give random weights to neuron weights
                    neuronWeights[k] = UnityEngine.Random.Range(-0.5f, 0.5f);
                }

                layerWeightsList.Add(neuronWeights); //add neuron weights of this current layer to layer weights
            }

            weightsList.Add(layerWeightsList.ToArray()); //add this layers weights converted into 2D array into weights list
        }

        weights = weightsList.ToArray(); //convert to 3D array
    }

    /// <summary>
    /// Feed forward this neural network with a given input array
    /// </summary>
    /// <param name="inputs">Inputs to network</param>
    /// <returns></returns>
    public float[] FeedForward(float[] inputs)
    {
        //Add inputs to the neuron matrix
        for (int i = 0; i < inputs.Length; i++)
        {
            neurons[0][i] = inputs[i];
        }

        //itterate over all neurons and compute feedforward values 
        for (int i = 1; i < layers.Length; i++)
        {
            for (int j = 0; j < neurons[i].Length; j++)
            {
                float value = 0f;

                for (int k = 0; k < neurons[i - 1].Length; k++)
                {
                    value += weights[i - 1][j][k] * neurons[i - 1][k]; //sum off all weights connections of this neuron weight their values in previous layer
                }

                neurons[i][j] = (float)Math.Tanh(value + biases[i][j]); //Hyperbolic tangent activation
            }
        }

        return neurons[neurons.Length - 1]; //return output layer
    }

    /// <summary>
    /// Mutate neural network weights
    /// </summary>
    public void Mutate()
    {
        for (int i = 0; i < biases.Length; i++)
        {
            for (int j = 0; j < biases[i].Length; j++)
            {
                biases[i][j] = (UnityEngine.Random.Range(0f, (1f/.05f)) <= 5) ? biases[i][j] += UnityEngine.Random.Range(-.5f, .5f) : biases[i][j];
            }
        }
        for (int i = 0; i < weights.Length; i++)
        {
            for (int j = 0; j < weights[i].Length; j++)
            {
                for (int k = 0; k < weights[i][j].Length; k++)
                {
                    float weight = weights[i][j][k];

                    //mutate weight value 
                    float randomNumber = UnityEngine.Random.Range(0f, 100f);

                    if (randomNumber <= 2f)
                    { //if 1
                      //flip sign of weight
                        weight *= -1f;
                    }
                    else if (randomNumber <= 4f)
                    { //if 2
                      //pick random weight between -1 and 1
                        weight = UnityEngine.Random.Range(-0.5f, 0.5f);
                    }
                    else if (randomNumber <= 6f)
                    { //if 3
                      //randomly increase by 0% to 100%
                        float factor = UnityEngine.Random.Range(0f, 1f) + 1f;
                        weight *= factor;
                    }
                    else if (randomNumber <= 8f)
                    { //if 4
                      //randomly decrease by 0% to 100%
                        float factor = UnityEngine.Random.Range(0f, 1f);
                        weight *= factor;
                    }

                    weights[i][j][k] = weight;
                }
            }
        }
    }

    public void AddFitness(float fit)
    {
        fitness += fit;
    }

    public void SetFitness(float fit)
    {
        fitness = fit;
    }

    public float GetFitness()
    {
        return fitness;
    }

    /// <summary>
    /// Compare two neural networks and sort based on fitness
    /// </summary>
    /// <param name="other">Network to be compared to</param>
    /// <returns></returns>
    public int CompareTo(NeuralNetworkFeedForward other)
    {
        if (other == null) return 1;

        if (fitness > other.fitness)
            return 1;
        else if (fitness < other.fitness)
            return -1;
        else
            return 0;
    }
    public void Load(string path)//this loads the biases and weights from within a file into the neural network.
    {
        TextReader tr = new StreamReader(path);
        int NumberOfLines = (int)new FileInfo(path).Length;
        string[] ListLines = new string[NumberOfLines];
        int index = 1;
        for (int i = 1; i < NumberOfLines; i++)
        {
            ListLines[i] = tr.ReadLine();
        }
        tr.Close();
        if (new FileInfo(path).Length > 0)
        {
            for (int i = 0; i < biases.Length; i++)
            {
                for (int j = 0; j < biases[i].Length; j++)
                {
                    biases[i][j] = float.Parse(ListLines[index]);
                    index++;
                }
            }

            for (int i = 0; i < weights.Length; i++)
            {
                for (int j = 0; j < weights[i].Length; j++)
                {
                    for (int k = 0; k < weights[i][j].Length; k++)
                    {
                        weights[i][j][k] = float.Parse(ListLines[index]); ;
                        index++;
                    }
                }
            }
        }
    }
    public void Save(string path)//this is used for saving the biases and weights within the network to a file.
    {
        File.Create(path).Close();
        StreamWriter writer = new StreamWriter(path, true);

        for (int i = 0; i < biases.Length; i++)
        {
            for (int j = 0; j < biases[i].Length; j++)
            {
                writer.WriteLine(biases[i][j]);
            }
        }

        for (int i = 0; i < weights.Length; i++)
        {
            for (int j = 0; j < weights[i].Length; j++)
            {
                for (int k = 0; k < weights[i][j].Length; k++)
                {
                    writer.WriteLine(weights[i][j][k]);
                }
            }
        }
        writer.Close();
    }
}


//using System.Collections.Generic;
//using System;
//public class NeuralNetworkFeedForward : IComparable<NeuralNetworkFeedForward>
//{
//    private int[] layers; //layers
//    private float[][] neurons; //neuron matrix
//    private float[][][] weights; //weight matrix
//    private float fitness; //fitness of the network

//    private Random random; //random number generator
//    /// <summary>
//    /// Initializes neural network with random weights
//    /// </summary>
//    /// <param name="layers">layers to the neural network</param>
//    public NeuralNetworkFeedForward(int[] layers)
//    {
//        //Initializing how many layers in the network
//        //Deep copy of layers of this network
//        this.layers = new int[layers.Length];

//        for(int i = 0; i < layers.Length; i++)
//        {
//            this.layers[i] = layers[i];
//        }

//        random = new Random(System.DateTime.Today.Millisecond); //random number seeded

//        //Generate Matrixes
//        InitNeurons();
//        InitWeights();
//    }

//    /// <summary>
//    /// Deep copy constructor
//    /// </summary>
//    /// <param name="copyNetwork">Network to deep copy</param>
//    public NeuralNetworkFeedForward(NeuralNetworkFeedForward copyNetwork)
//    {
//        this.layers = new int[copyNetwork.layers.Length];
//        for (int i = 0; i < copyNetwork.layers.Length; i++)
//        {
//            this.layers[i] = copyNetwork.layers[i];

//        }
//        InitNeurons();
//        InitWeights();
//        CopyWeights(copyNetwork.weights);

//    }
//    private void CopyWeights(float[][][] copyWeights)
//    {
//        for (int i = 0; i < weights.Length; i++)
//        {
//            for (int j = 0; j < weights[i].Length; j++)
//            {
//                for (int k = 0; k < weights[i][j].Length; k++)
//                {
//                    weights[i][j][k] = copyWeights[i][j][k];
//                }
//            }
//        }
//    }

//    /// <summary>
//    /// Create neuron matrix
//    /// </summary>
//    private void InitNeurons()
//    {
//        //Neuron Initialization
//        List<float[]> neuronsList = new List<float[]>();

//        for (int i = 0; i < layers.Length; i++) //run through all the layers
//        {
//            neuronsList.Add(new float[layers[i]]); //add layer to neuron list
//        }

//        neurons = neuronsList.ToArray(); //convert list to array
//    }

//    /// <summary>
//    /// Create weight matrix
//    /// </summary>
//    private void InitWeights()
//    {
//        List<float[][]> weightsList = new List<float[][]>(); //weights list which will later convert into a weights 3D array

//        //itterate over all neurons thhat have a weight connection (basically everything except the input layer)
//        for(int i = 1; i < layers.Length; i++)
//        {
//            List<float[]> layerWeightsList = new List<float[]>();// layer weight list for this current layer (will be converted to a 2D array)

//            int neuronsInPreviousLayer = layers[i - 1];

//            //itterate over all neurons in this layer
//            for(int j = 0; j < neurons[i].Length; i++)
//            {
//                float[] neuronWeights = new float[neuronsInPreviousLayer]; //neurons weights

//                //itterate over all neurons in the previous layer and set the weights randomly between 0.5f and -0.5f
//                for(int k = 0; k < neuronsInPreviousLayer; k++)
//                {
//                    //give random weights to neuron weights
//                    neuronWeights[k] = UnityEngine.Random.Range(-0.5f, 0.5f);
//                }

//                layerWeightsList.Add(neuronWeights); //add neuron weights of this current layer to layer weights
//            }

//            weightsList.Add(layerWeightsList.ToArray());//add this layers weights converted into a 2D array into weights list
//        }
//        weights = weightsList.ToArray(); //convert the 2D weights list into a 3D array
//    }

//    /// <summary>
//    /// Feed forward this neural network with a given input array
//    /// </summary>
//    /// <param name="inputs">Inputs to network</param>
//    /// <returns></returns>
//    public float[] FeedForward(float[] inputs)
//    {
//        //Add inputs to the neurons matrix
//        for(int i = 0; i < inputs.Length; i++)
//        {
//            neurons[0][i] = inputs[i];
//        }

//        //Iterate over all neurons and compute feedforward values
//        for(int i = 1; i< layers.Length; i++)
//        {
//            //Iterate over every neuron in layer[i]
//            for(int j = 0; j <neurons[i].Length; j++)
//            {
//                float value = 0.0f; //0.25f
//                for (int k = 0; k < neurons[i-1].Length; k++)
//                {
//                    value += weights[i - 1][j][k] * neurons[i - 1][k]; //sum of all weights connections of this neuron with their values in the previous layer
//                }

//                neurons[i][j] = (float)Math.Tanh(value); //Hyperbolic tangent activation
//            }
//        }
//        return neurons[neurons.Length - 1]; //return output layer
//    }

//    public void Mutate()
//    {
//        for(int i = 0; i < weights.Length; i++)
//        {
//            for (int j = 0; j < weights[i].Length; j++)
//            {
//                for (int k = 0; k < weights[i][j].Length; k++)
//                {
//                    float weight = weights[i][j][k];

//                    //mutate weight value
//                    float randomNumber = UnityEngine.Random.Range(0f, 100f);

//                    if (randomNumber <= 2f)
//                    {
//                        //flip sign of weight
//                        weight *= -1f;
//                    }
//                    if (randomNumber <= 4f)
//                    {
//                        //pick random weight between -1 and 1
//                        weight = UnityEngine.Random.Range(-0.5f, 0.5f);
//                    }
//                    if (randomNumber <= 6f)
//                    {
//                        //randomly increase by 0% to 100%
//                        float factor = UnityEngine.Random.Range(0f, 1f) + 1f;
//                        weight *= factor;
//                    }
//                    if (randomNumber <= 8f)
//                    {
//                        //randomly decrease by 0% to 100%
//                        float factor = UnityEngine.Random.Range(0f, 1f);
//                        weight *= factor;
//                    }
//                    weights[i][j][k] = weight;
//                }
//            }
//        }
//    }

//    public void AddFitness(float fit)
//    {
//        fitness += fit;
//    }
//    public void SetFitness(float fit)
//    {
//        fitness = fit;
//    }
//    public float GetFitness()
//    {
//        return fitness;
//    }
//    /// <summary>
//    /// Compare two neural networks and sort based on fitness
//    /// </summary>
//    /// <param name="other">Network to be compared to</param>
//    /// <returns></returns>
//    public int CompareTo(NeuralNetworkFeedForward other)
//    {
//        if (other == null) return 1;

//        if (fitness > other.fitness)
//            return 1;
//        else if (fitness < other.fitness)
//            return -1;
//        else
//            return 0;

//    }
//}
