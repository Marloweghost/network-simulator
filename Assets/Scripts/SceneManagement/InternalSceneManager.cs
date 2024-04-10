using UnityEngine.SceneManagement;
using UnityEngine;
using System.Linq;

public class InternalSceneManager : MonoBehaviour
{
    private static InternalSceneManager instance;

    private static InternalSceneManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject gameObject = new GameObject("SceneManager");
                instance = gameObject.AddComponent<InternalSceneManager>();
            }

            return instance;
        }
    }

    private float generatorComplexity;
    private int generatorDifficulty;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += ChangeGeneratorProperties;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= ChangeGeneratorProperties;
    }

    public void SwitchScene(string newSceneName)
    {
        if (SceneManager.GetSceneByName(newSceneName) == null)
        {
            Debug.Log("—цена с заданным названием не найдена!");
        }
        else
        {
            SceneManager.LoadScene(newSceneName);
        }
    }

    public void SwitchScene(string newSceneName, float complexity, int difficulty)
    {
        if (SceneManager.GetSceneByName(newSceneName) == null)
        {
            Debug.Log("—цена с заданным названием не найдена!");
        }
        else
        {
            generatorComplexity = complexity;
            generatorDifficulty = difficulty;
            SceneManager.LoadScene(newSceneName);
        }
    }

    private void ChangeGeneratorProperties(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Level_Generated")
        {
            NetworkModelGenerator generator = GameObject.FindGameObjectWithTag("Generator").GetComponent<NetworkModelGenerator>();
            generator.complexity = generatorComplexity;
            generator.skillLevel = generatorDifficulty;
            generator.Activate();
        }
    }
}
