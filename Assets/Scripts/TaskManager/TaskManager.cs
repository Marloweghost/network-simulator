using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
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

    private void Awake()
    {
        onActionCompleted.AddListener(OnActionCompleted);
    }

    private void Start()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        Activate(currentScene);
        InitializeTasks();
        UI.ActivateNotificationStartPanel(taskDictionary.GetValueOrDefault(levelName).NotificationStartText);
    }

    private void InitializeTasks()
    {
        
        taskDictionary.Add("Level_1", new TaskInfo("// Необходимо выполнить Ping от PC1 до PC0\r\n//-// Достаточно ввести адрес целевого компьютера в соответствующее поле и нажать кнопку Ping!\r\n\r\n[F] - открыть свойства компьютера",
            "// Отличная работа!\r\n\r\nС помощью Ping можно проверять, есть ли сетевое соединение между двумя узлами.\r\n\r\nЭта функция пригодится Вам в будущем!"));
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

    public ActionInfoPing(string replyReceiverName, string replySenderName)
    {
        ReplyReceiverName = replyReceiverName;
        ReplySenderName = replySenderName;
    }
}

[System.Serializable]
public class ActionCompletedEvent : UnityEvent<ActionInfo> { }
