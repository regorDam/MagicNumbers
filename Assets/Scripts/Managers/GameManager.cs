using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    private APIController apiController;
    public float GetTimeStatement { get { return timeStatement; } }

    private APINumbers currentNumber;

    [SerializeField]
    Text successCount;
    [SerializeField]
    Text faultCount;

    int intSuccesCount;
    int intFaultCount;
    
    public bool NextStatement { get; private set; }

    public int CountCurrentErrors { get; private set; }
    public GameObject ErrorGO { get; private set; }

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

        apiController = GetComponent<APIController>();

        stateManager = StateManager.Instance;

        stateManager.RegisterState(State.STATEMENT,new StatementState());
        stateManager.RegisterState(State.ANSWER, new AnswerState());
        
        stateManager.OnStateChange += HandleOnStateChange;
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(WaitAPI());
    }

    // Update is called once per frame
    void Update()
    {
        stateManager.Update(Time.deltaTime);
    }

    public void HandleOnStateChange()
    {
        NextStatement = false;
        CountCurrentErrors = 0;
        ErrorGO = null;
    }

    public GameObject FindInScene(string TAG)
    {

        return GameObject.FindGameObjectWithTag(TAG);
    }

    public StateManager GetStateManager()
    {
        return stateManager;
    }

    public string GetRandomLabel()
    {
        currentNumber = apiController.GetRandomNumber();

        return currentNumber.label;
    }

    public int GetValue()
    {
        return currentNumber.value;
    }

    public APINumbers GetCurrentNumber()
    {
        return currentNumber;
    }

    public NumbersCollection GetNumbersCollection()
    {
        return apiController.NumbersCol;
    }

    IEnumerator WaitAPI()
    {
        yield return new WaitForSeconds(1f);

        stateManager.ChangeTo(State.STATEMENT);
    }

    public void OnClickCheckAnswer(Button btn)
    {
        Text text = btn.GetComponentInChildren<Text>();
        string value = text.text;
        
        if(value == GetValue()+"")
        {
            intSuccesCount++;
            successCount.text = intSuccesCount+"";
            NextStatement = true;
        }
        else
        {
            intFaultCount++;
            faultCount.text = intFaultCount + "";

            CountCurrentErrors++;
            ErrorGO = btn.gameObject;
        }        
    }

    public void ExecuteCoroutine(IEnumerator routine)
    {
        StartCoroutine(routine);
    }

    public void ResetErrorGO()
    {
        ErrorGO = null;
    }
}
