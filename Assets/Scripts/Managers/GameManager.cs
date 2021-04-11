using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(APIController))]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance = null;

    [SerializeField]
    private bool isDebug;

    public bool IsDebug { get { return Console.isDebug; }  private set { isDebug = value; Console.isDebug = isDebug; } }

    private StateManager stateManager;

    [SerializeField]
    float timeStatement = 2;

    public float GetTimeStatement { get { return timeStatement; } }

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        Init();
    }

    void Init()
    {
        IsDebug = isDebug;

        stateManager = StateManager.Instance;

        stateManager.RegisterState(State.STATEMENT,new StatementState());
        stateManager.RegisterState(State.ANSWER, new AnswerState());
        
        stateManager.OnStateChange += HandleOnStateChange;
    }

    // Start is called before the first frame update
    void Start()
    {
        stateManager.ChangeTo(State.STATEMENT);
    }

    // Update is called once per frame
    void Update()
    {
        stateManager.Update(Time.deltaTime);


    }

    public void HandleOnStateChange()
    {

    }

    public GameObject FindInScene(string TAG)
    {

        return GameObject.FindGameObjectWithTag(TAG);
    }

    public StateManager GetStateManager()
    {
        return stateManager;
    }

}
