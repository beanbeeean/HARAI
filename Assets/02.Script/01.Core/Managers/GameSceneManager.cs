using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager Instance;

    public Action<string> changeSceneEvent;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }

    public void LoadSceneByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        changeSceneEvent?.Invoke(sceneName);
    }


    public void ExitGame()
    {
        Application.Quit();
    }
    
}
