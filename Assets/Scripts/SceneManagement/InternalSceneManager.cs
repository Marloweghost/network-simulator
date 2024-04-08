using UnityEngine.SceneManagement;
using UnityEngine;

public class InternalSceneManager : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }


}
