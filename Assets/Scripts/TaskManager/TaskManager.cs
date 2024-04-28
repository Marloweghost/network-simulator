using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class TaskManager : MonoBehaviour
{
    public static ActionCompletedEvent onActionCompleted = new ActionCompletedEvent();

    [SerializeField] private TaskManagerUI UI;
    private string levelName;
    private Dictionary<string, TaskInfo> taskDictionary = new Dictionary<string, TaskInfo>();
    [SerializeField] private GameObject nodeInteractionCanvas;
    [SerializeField] private GameObject generatorInstance;
    private System.Random randomGenerator = new System.Random();
    private int taskType;
    private int taskCount = 2;
    private Graph currentGraph;
    private string maskToConnectTo;
    private string subnetToConnectTo;
    private string ipToConnectTo;

    private void Awake()
    {
        onActionCompleted.AddListener(OnActionCompleted);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            UI.ActivateNotificationStartPanel(taskDictionary.GetValueOrDefault(levelName).NotificationStartText);
        }
    }
    private void Start()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        Activate(currentScene);
        
        if (currentScene.name == "Level_Generated")
        {
            RandomizeGeneratedTask(taskCount);
            SetupGeneratedTask(taskType);
        }
        InitializeTasks();

        UI.ActivateNotificationStartPanel(taskDictionary.GetValueOrDefault(levelName).NotificationStartText);
    }
    private void InitializeTasks()
    {
        taskDictionary.Add("Level_1", new TaskInfo("// ���������� ��������� Ping �� PC1 �� PC0\r\n//-// ���������� ������ ����� �������� ���������� � ��������������� ���� � ������ ������ Ping!\r\n\r\n[F] - ������� �������� ����������\r\n[O] - ������� �������",
            "// �������� ������!\r\n\r\n� ������� Ping ����� ���������, ���� �� ������� ���������� ����� ����� ������.\r\n\r\n��� ������� ���������� ��� � �������!"));

        string notificationStartGeneratedTaskText = "";
        string notificationSuccessGeneratedTaskText = "";

        switch (taskType)
        {
            case 0:
                notificationStartGeneratedTaskText = $"// ���������� ������������ ��������� � ������ ARM � ������� {subnetToConnectTo}, � ��������� �� ���� Ping �� ���� � ������� {ipToConnectTo}";
                notificationSuccessGeneratedTaskText = $"// �������� ������!";
                break;
            case 1:
                break;
            default:
                break;
        }

        taskDictionary.Add("Level_Generated", new TaskInfo(notificationStartGeneratedTaskText, notificationSuccessGeneratedTaskText));
    }
    private void RandomizeGeneratedTask(int _taskCount)
    {
        // taskType = randomGenerator.Next(0, taskCount);
        taskType = 0;
    }
    private void SetupGeneratedTask(int taskType)
    {
        NetworkModelGenerator networkModelGenerator = generatorInstance.GetComponent<NetworkModelGenerator>();

        currentGraph = networkModelGenerator.GetGraph();

        switch (taskType)
        {
            case 0:
                // �������� �������� :( ���� �� �������
                GameObject spawnedNode = networkModelGenerator.InstantiateSingleNode();
                // ���� ��� ��� ARM
                spawnedNode.name = "ARM";
                int[] nodesIndexOnSwitchLevel = currentGraph.GetNodesFromLevel(currentGraph.GetUniqueLevelsCount() - 2);
                int nodeIndex = randomGenerator.Next(0, nodesIndexOnSwitchLevel.Length);
                maskToConnectTo = currentGraph.GetSubnetMask(nodesIndexOnSwitchLevel[nodeIndex]);
                subnetToConnectTo = networkModelGenerator.CalculateNetworkAddress(currentGraph.GetIPAddress(nodesIndexOnSwitchLevel[nodeIndex]), maskToConnectTo);
                ipToConnectTo = currentGraph.GetIPAddress(randomGenerator.Next(0, currentGraph.vertexCount));
                Debug.Log(subnetToConnectTo + " " + maskToConnectTo);
                break;
            default:
                break;
        }
    }
    private void OnDisable()
    {
        onActionCompleted.RemoveListener(OnActionCompleted);
    }
    public void Activate(Scene scene)
    {
        levelName = scene.name;
    }
    private void OnActionCompleted(ActionInfo actionInfo)
    {
        switch (levelName)
        {
            case "Level_1":
                if (actionInfo is ActionInfoPing actionInfoPing)
                {
                    if (actionInfoPing.ReplyReceiverName == "PC1" && actionInfoPing.ReplySenderName == "PC0")
                    {
                        StartCoroutine(OnTaskCompleted());
                    }
                }
                break;
            case "Level_2":
                break;
            case "Level_Generated":
                HandleGeneratedTask(actionInfo);
                break;
            default:
                break;
        }
    }
    private IEnumerator OnTaskCompleted()
    {
        yield return new WaitForSeconds(1f);
        nodeInteractionCanvas.GetComponentInChildren<UINodeInterface>().Deactivate();
        UI.ActivateNotificationSuccessPanel(taskDictionary.GetValueOrDefault(levelName).NotificationSuccessText);
    }
    private void HandleGeneratedTask(ActionInfo actionInfo)
    {
        switch (taskType)
        {
            case 0:
                if (actionInfo is ActionInfoPing actionInfoPing)
                {
                    // ����� ������������ �������� �� � ����� �� ��������:
                        // ��������� �� �������������� � �������
                        // ����������� ��� Ping �������� ���������� �� ���� ����
                    if (actionInfoPing.ReplyReceiverName == "ARM" && IsIpInSubnet(actionInfoPing.ReplyReceiverIP, subnetToConnectTo, maskToConnectTo) && actionInfoPing.ReplySenderIP == ipToConnectTo)
                    {
                        StartCoroutine(OnTaskCompleted());
                    }
                }
                break;
            case 1:
                break;
            default:
                break;
        }
    }
    private bool IsIpInSubnet(string ip, string subnet, string mask)
    {
        // ��������� IP-�����, ������� � ����� �� ������
        string[] ipOctets = ip.Split('.');
        string[] subnetOctets = subnet.Split('.');
        string[] maskOctets = mask.Split('.');

        // ����������� ������ � ����� �����
        int[] ipNumbers = Array.ConvertAll(ipOctets, int.Parse);
        int[] subnetNumbers = Array.ConvertAll(subnetOctets, int.Parse);
        int[] maskNumbers = Array.ConvertAll(maskOctets, int.Parse);

        // ��������� ����� ����
        int[] networkNumbers = new int[4];
        for (int i = 0; i < 4; i++)
        {
            networkNumbers[i] = subnetNumbers[i] & maskNumbers[i];
        }

        // ��������� ����� IP-������ � ����
        int[] ipInNetworkNumbers = new int[4];
        for (int i = 0; i < 4; i++)
        {
            ipInNetworkNumbers[i] = ipNumbers[i] & maskNumbers[i];
        }

        // ���������, ��� ����� IP-������ � ���� ��������� � ������� ����
        for (int i = 0; i < 4; i++)
        {
            if (ipInNetworkNumbers[i] != networkNumbers[i])
            {
                return false;
            }
        }

        return true;
    }
}

public struct TaskInfo
{
    private string _notificationStartText;
    private string _notificationSuccessText;

    public string NotificationStartText
    {
        get { return _notificationStartText; }
        set { _notificationStartText = value; }
    }

    public string NotificationSuccessText
    {
        get { return _notificationSuccessText; }
        set { _notificationSuccessText = value; }
    }

    public TaskInfo(string notificationStartText, string notificationSuccessText)
    {
        _notificationStartText = notificationStartText;
        _notificationSuccessText = notificationSuccessText;
    }
}
public abstract class ActionInfo
{
    
}
public class ActionInfoPing : ActionInfo
{
    public string ReplyReceiverName;
    public string ReplySenderName;
    public string ReplyReceiverIP;
    public string ReplySenderIP;

    public ActionInfoPing(string replyReceiverName, string replySenderName, string replyReceiverIP, string replySenderIP)
    {
        ReplyReceiverName = replyReceiverName;
        ReplySenderName = replySenderName;
        ReplyReceiverIP = replyReceiverIP;
        ReplySenderIP = replySenderIP;
    }
}
[System.Serializable]
public class ActionCompletedEvent : UnityEvent<ActionInfo> { }
