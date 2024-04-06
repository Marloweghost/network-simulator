using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NetworkModelGenerator : MonoBehaviour
{
    private System.Random random;

    [Header("References")]
    [SerializeField] private GameObject nodePrefab;
    [SerializeField] private GameObject switchPrefab;
    [SerializeField] private GameObject routerPrefab;
    [SerializeField] private GameObject connectionCablePrefab;
    [SerializeField] private Transform spawnPosition;

    [Header("Parameters")]
    public int defaultNodeCount = 7;
    public string networkSubnetMask = "255.255.255.0";

    private int nodeCount;
    
    [Range(0f, 1f)] public float complexity = 0.5f;
    [Range(1, 3)] public int skillLevel = 1;

    [SerializeField] private int[] levelNodesCountLowerBounds = new int[4];
    [SerializeField] private int[] levelNodesCountUpperBounds = new int[4];

    private int[,] levelNodesCountBounds;

    private void Start()
    {
        levelNodesCountBounds = new int[4, 2];
        for (int i = 0; i < levelNodesCountLowerBounds.Length; i++)
        {
            levelNodesCountBounds[i, 0] = levelNodesCountLowerBounds[i];
        }

        for (int i = 0; i < levelNodesCountUpperBounds.Length; i++)
        {
            levelNodesCountBounds[i, 1] = levelNodesCountUpperBounds[i];
        }

        random = new System.Random();

        
        nodeCount = random.Next(Mathf.RoundToInt(2 + skillLevel * Mathf.Lerp(1f, 2f, complexity)), Mathf.RoundToInt(6 + skillLevel * Mathf.Lerp(2f, 3f, complexity)));
        Debug.Log("Общее кол-во нод: " + nodeCount);

        //nodeCount = defaultNodeCount;

        Graph graph = new Graph(nodeCount);
        SetNodeLevels(graph);
        SetNodeConnections(graph);
        SetNodesProperties(graph);

        InstantiateNetworkByGraph(graph);
    }

    // Починить
    private void SetNodeLevels(Graph _graph)
    {

        //
        int groupsCount = 3;
        if (_graph.vertexCount < levelNodesCountBounds[0,0] + levelNodesCountBounds[1,0] + levelNodesCountBounds[2,0] + levelNodesCountBounds[3,0])
        {
            groupsCount = 3;
        }
        else if (_graph.vertexCount < levelNodesCountBounds[0,0] + levelNodesCountBounds[1,0] + levelNodesCountBounds[2,0])
        {
            groupsCount = 2;
        }

        int[] levelNodesCount = new int[groupsCount];

        int _nodesRemaining = _graph.vertexCount;

        for (int groupIndex = 0; groupIndex < levelNodesCount.Length; groupIndex++)
        {
            if (groupIndex == 0)
            {
                levelNodesCount[groupIndex] = 1;
                _nodesRemaining -= 1;
            }
            else
            {
                int localGroupNodeCount = random.Next(levelNodesCountBounds[groupIndex, 0], levelNodesCountBounds[groupIndex, 1] + 1);

                Debug.Log($"Кол-во нод в группе {groupIndex} = {localGroupNodeCount}");

                levelNodesCount[groupIndex] = localGroupNodeCount;
                _nodesRemaining -= localGroupNodeCount;
            }
            
            if (levelNodesCount.Length == groupIndex + 1)
            {
                levelNodesCount[groupIndex] += _nodesRemaining;
                _nodesRemaining = 0;
            }
        }

        int currentNodeIndex = 0;

        for (int currentLevel = 0; currentLevel < levelNodesCount.Length; currentLevel++)
        {
            for (int nodeInGroup = 0; nodeInGroup < levelNodesCount[currentLevel]; nodeInGroup++)
            {
                _graph.AssignLevel(currentNodeIndex, currentLevel);
                Debug.Log($"{currentNodeIndex}, {currentLevel}");
                currentNodeIndex++;
            }
        }

        
        

        // TODO: Автоматизировать
        //_graph.AssignLevel(0, 0);
        //_graph.AssignLevel(1, 1);
        //_graph.AssignLevel(2, 1);
        //_graph.AssignLevel(3, 2);
        //_graph.AssignLevel(4, 2);
        //_graph.AssignLevel(5, 2);
        //_graph.AssignLevel(6, 2);
    }

    private int[][] DivideArray(int groupsCount, int[] source)
    {
        int nodesInGroupCount = source.Length / groupsCount;
        int remainder = source.Length % groupsCount;

        int[][] groupsArray = new int[groupsCount][];

        int sourceIndex = 0;

        for (int groupIndex = 0; groupIndex < groupsCount; groupIndex++)
        {
            if (groupIndex + 1 == groupsCount)
            {
                nodesInGroupCount += remainder;
            }

            groupsArray[groupIndex] = new int[nodesInGroupCount];
            
            for (int inGroupIndex = 0; inGroupIndex < groupsArray[groupIndex].Length; inGroupIndex++)
            {
                groupsArray[groupIndex][inGroupIndex] = source[sourceIndex++];
            }
        }

        return groupsArray;
    }

    private void SetNodeConnections(Graph _graph)
    {
        int levelsCount = _graph.GetUniqueLevelsCount();

        for (int currentLevel = 0; currentLevel < levelsCount; currentLevel++)
        {
            int[] currentLevelNodes = _graph.GetNodesFromLevel(currentLevel);
            int[] nextLevelNodes = _graph.GetNodesFromLevel(currentLevel + 1);

            int n; // Количество частей
            n = currentLevelNodes.Length;

            int[][] dividedArray = new int[n][];

            if (nextLevelNodes.Length == 0) break;

            if (currentLevel == 0)
            {
                for (int j = 0; j < nextLevelNodes.Length; j++)
                {
                    _graph.AddEdge(currentLevel, nextLevelNodes[j]);
                }

                continue;
            }

            dividedArray = DivideArray(n, nextLevelNodes);

            //for (int i = 0; i < n; i++)
            //{
            //    dividedArray[i] = new int[k];
            //    Array.Copy(nextLevelNodes, i * k, dividedArray[i], 0, k);
            //}

            for (int i = 0; i < currentLevelNodes.Length; i++)
            {
                for (int j = 0; j < dividedArray[i].Length; j++)
                {
                    Debug.Log(currentLevelNodes[i] + ", " + dividedArray[i][j]);
                    _graph.AddEdge(currentLevelNodes[i], dividedArray[i][j]);
                }
            }

            _graph.PrintTree();
        }
    }

    private void InstantiateNetworkByGraph(Graph _graph)
    {
        Vector3 startPosition = spawnPosition.position;

        Vector3 levelPositionDelta = new Vector3(0, -2f, 0);
        Vector3 nodePositionDelta = new Vector3(0, 0, 1f);

        GameObject[] spawnedNodes = new GameObject[_graph.vertexCount];

        int levelsCount = _graph.GetUniqueLevelsCount();

        for (int nodeIndex = 0; nodeIndex < _graph.vertexCount; nodeIndex++)
        {
            int nodeLevel = _graph.GetLevel(nodeIndex);

            GameObject spawnedNode = null;

            if (nodeLevel == levelsCount - 1)
            {
                spawnedNode = Instantiate(nodePrefab, new Vector3(startPosition.x + levelPositionDelta.x * nodeLevel + nodePositionDelta.x * nodeIndex * Mathf.Pow(-1f, nodeIndex),
                startPosition.y + levelPositionDelta.y * nodeLevel + nodePositionDelta.y * nodeIndex * Mathf.Pow(-1f, nodeIndex),
                startPosition.z + levelPositionDelta.z * nodeLevel + nodePositionDelta.z * nodeIndex * Mathf.Pow(-1f, nodeIndex)),
                Quaternion.identity);
            }
            else if (nodeLevel != 0)
            {
                spawnedNode = Instantiate(switchPrefab, new Vector3(startPosition.x + levelPositionDelta.x * nodeLevel + nodePositionDelta.x * nodeIndex * Mathf.Pow(-1f, nodeIndex),
                startPosition.y + levelPositionDelta.y * nodeLevel + nodePositionDelta.y * nodeIndex * Mathf.Pow(-1f, nodeIndex),
                startPosition.z + levelPositionDelta.z * nodeLevel + nodePositionDelta.z * nodeIndex * Mathf.Pow(-1f, nodeIndex)),
                Quaternion.identity);
            }
            else
            {
                spawnedNode = Instantiate(routerPrefab, new Vector3(startPosition.x + levelPositionDelta.x * nodeLevel + nodePositionDelta.x * nodeIndex * Mathf.Pow(-1f, nodeIndex),
                startPosition.y + levelPositionDelta.y * nodeLevel + nodePositionDelta.y * nodeIndex * Mathf.Pow(-1f, nodeIndex),
                startPosition.z + levelPositionDelta.z * nodeLevel + nodePositionDelta.z * nodeIndex * Mathf.Pow(-1f, nodeIndex)),
                Quaternion.identity);
            }

            spawnedNodes[nodeIndex] = spawnedNode;

            NetworkAdapter spawnedNodeNetworkAdapter = spawnedNode.GetComponentInChildren<NetworkAdapter>();
            spawnedNodeNetworkAdapter.SetIPAddress(_graph.GetIPAddress(nodeIndex));
            spawnedNodeNetworkAdapter.SetSubnetMask(_graph.GetSubnetMask(nodeIndex));
        }

        for (int _nodeIndex = 0; _nodeIndex < spawnedNodes.Length; _nodeIndex++)
        {
            int[] _childNodesIndexes = _graph.GetChildNodes(_nodeIndex);

            foreach (int _childNodeIndex in _childNodesIndexes)
            {
                GameObject connectionInstance = Instantiate(connectionCablePrefab,
                    (spawnedNodes[_nodeIndex].transform.position + spawnedNodes[_childNodeIndex].transform.position) / 2f,
                    Quaternion.identity);
                connectionInstance.GetComponent<WireStateHandler>().ChangeState(WireStateHandler.State.Connected);

                spawnedNodes[_nodeIndex].GetComponentInChildren<NetworkAdapter>().GetFreePhysicalInterface().PhysicalPort = connectionInstance.transform.GetChild(0).gameObject;
                spawnedNodes[_childNodeIndex].GetComponentInChildren<NetworkAdapter>().GetFreePhysicalInterface().PhysicalPort = connectionInstance.transform.GetChild(1).gameObject;
            }
        }
    }

    private void SetNodesProperties(Graph _graph)
    {
        SetNodeProperties(0, _graph);

        _graph.PrintAddresses();
    }

    // T_D: Учесть случай, когда IP-адресов не хватает
    // Рекурсивный вызов
    private void SetNodeProperties(int nodeIndex, Graph _graph)
    {
        string newAddress = GenerateIPAddress("192.168.0.1", networkSubnetMask, _graph);
        _graph.AddIPAddress(nodeIndex, newAddress);
        _graph.AddSubnetMask(nodeIndex, networkSubnetMask);

        int[] childNodes = _graph.GetChildNodes(nodeIndex);

        for (int localNodeIndex = 0; localNodeIndex < childNodes.Length; localNodeIndex++)
        {
            SetNodeProperties(childNodes[localNodeIndex], _graph);
        }
    }

    private string GenerateIPAddress(string baseIPAddress, string subnetMask, Graph _graph)
    {
        // Преобразование IP-адреса в числовой формат
        string[] baseIPAddressParts = baseIPAddress.Split('.');
        uint baseIP = (uint.Parse(baseIPAddressParts[0]) << 24) |
                  (uint.Parse(baseIPAddressParts[1]) << 16) |
                  (uint.Parse(baseIPAddressParts[2]) << 8) |
                  uint.Parse(baseIPAddressParts[3]);

        // Преобразование маски подсети в числовой формат
        string[] subnetMaskParts = subnetMask.Split('.');
        uint subnetMaskIP = (uint.Parse(subnetMaskParts[0]) << 24) |
                            (uint.Parse(subnetMaskParts[1]) << 16) |
                            (uint.Parse(subnetMaskParts[2]) << 8) |
                            uint.Parse(subnetMaskParts[3]);

        // Вычисление диапазона адресов подсети
        uint networkAddress = baseIP & subnetMaskIP;
        uint broadcastAddress = networkAddress | ~subnetMaskIP;

        // Поиск первого доступного IP-адреса в диапазоне
        for (uint i = networkAddress + 1; i < broadcastAddress; i++)
        {
            string ipAddress = $"{(i >> 24) & 0xFF}.{(i >> 16) & 0xFF}.{(i >> 8) & 0xFF}.{i & 0xFF}";

            // Проверка, что полученный IP-адрес не занят другим узлом
            if (!_graph.IsIPAddressTaken(ipAddress))
            {
                Debug.Log(ipAddress);
                return ipAddress;
            }
        }

        // Возвращение null, если не удалось найти доступный IP-адрес
        return null;
    }
}
