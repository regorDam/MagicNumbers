using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class LevelManager: MonoBehaviour
{
    private static LevelManager m_intance = null;

    private int currentGameScene;
    public int CurrentGameScene { get { return currentGameScene; } }

    public static LevelManager Instance
    {
        get
        {
            if (m_intance == null)
            {
                Instantiate(Resources.Load("Prefabs/LevelManager"), Vector3.zero, Quaternion.identity);
            }
            return m_intance;
        }
    }

    void Awake()
    {
        if (m_intance == null)
            m_intance = this;
        else if (m_intance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public void LoadGame(int index)
    {
        currentGameScene = index;
        SceneManager.LoadSceneAsync(index);
    }
}
