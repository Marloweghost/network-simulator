using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Graph
{
    private int[,] adjacencyMatrix;
    private string[] addressArray;
    private string[] subnetMaskArray;
    public int vertexCount { get; private set; }

    public Graph(int vertices)
    {
        vertexCount = vertices;
        adjacencyMatrix = new int[vertexCount, vertexCount];
        addressArray = new string[vertexCount];
        subnetMaskArray = new string[vertexCount];
    }

    public void AddEdge(int i, int j)
    {
        if (i == j)
        {
            Debug.LogError("Cannot set the edge between the same node!");
            return;
        }

        adjacencyMatrix[i, j] = 1;
        adjacencyMatrix[j, i] = 1;
    }

    public void AddIPAddress(int nodeIndex, string IPAddress)
    {
        addressArray[nodeIndex] = IPAddress;
    }

    public void AddSubnetMask(int nodeIndex, string subnetMask)
    {
        subnetMaskArray[nodeIndex] = subnetMask;
    }

    public bool IsIPAddressTaken(string ipAddress)
    {
        return addressArray.Contains(ipAddress);
    }

    /// <summary>
    /// Возвращает массив с индексами узлов, дочерних для данного узла
    /// </summary>
    /// <param name="nodeIndex"></param>
    /// <returns></returns>
    public int[] GetChildNodes(int nodeIndex)
    {
        List<int> childNodes = new List<int>();

        for (int i = nodeIndex + 1; i < vertexCount; i++)
        {
            if (adjacencyMatrix[i, nodeIndex] == 1)
            {
                childNodes.Add(i);
            }            
        }
       
        // Debug
        int[] array = childNodes.ToArray();
        string output = "";

        for (int i = 0; i < array.Length; i++)
        {
            output += array[i] + ", ";
        }

        Debug.Log($"Child nodes to node {nodeIndex} are: ");
        Debug.Log(output);
        

        return childNodes.ToArray();
    }

    public void AssignLevel(int nodeNumber, int level)
    {
        adjacencyMatrix[nodeNumber, nodeNumber] = level;
    }

    public string GetIPAddress(int nodeIndex)
    {
        return addressArray[nodeIndex];
    }

    public string GetSubnetMask(int nodeIndex)
    {
        return subnetMaskArray[nodeIndex];
    }

    public int GetLevel(int nodeNumber)
    {
        return adjacencyMatrix[nodeNumber, nodeNumber];
    }

    public int GetUniqueLevelsCount()
    {
        int[] levelArray = new int[vertexCount];

        for (int nodeIndex = 0; nodeIndex < vertexCount; nodeIndex++)
        {
            levelArray[nodeIndex] = adjacencyMatrix[nodeIndex, nodeIndex];
        }

        HashSet<int> uniqueNumbers = new HashSet<int>(levelArray);

        int count = uniqueNumbers.Count;

        return count;
    }

    public int[] GetNodesFromLevel(int level)
    {
        List<int> nodes = new List<int>();

        for (int nodeIndex = 0; nodeIndex < vertexCount; nodeIndex++)
        {
            if (adjacencyMatrix[nodeIndex, nodeIndex] == level)
            {
                nodes.Add(nodeIndex);
            }
        }

        return nodes.ToArray();
    }

    public int[] GetAllNodes()
    {
        List<int> nodes = new List<int>();

        for (int nodeIndex = 0; nodeIndex < vertexCount; nodeIndex++)
        {
            nodes.Add(nodeIndex);
        }

        return nodes.ToArray();
    }

    public void PrintTree()
    {
        string output = "";

        Debug.Log("Graph as a Tree (Adjacency Matrix):");
        for (int i = 0; i < vertexCount; i++)
        {
            for (int j = 0; j < vertexCount; j++)
            {
                output = output + adjacencyMatrix[i, j] + " ";
            }
            Debug.Log(output);
            output = "";
        }
    }

    public void PrintAddresses()
    {
        for (int nodeIndex = 0; nodeIndex < vertexCount; nodeIndex++)
        {
            Debug.Log($"Узел {nodeIndex}: {addressArray[nodeIndex]}");
        }
    }
}
