using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class AnswerState : AState
{
    Text[] values;
    Image[] images;

    NumbersCollection numbersCollection;
    public LinkedList<APINumbers> numbersList;

    float currentTimeStatement;
    bool isFading = false;
    int correctIndex;

    bool forceChangeState = false;

    public AnswerState() : base("Slots")
    {
        numbersList = new LinkedList<APINumbers>();
        values = go.GetComponentsInChildren<Text>();

        images = go.GetComponentsInChildren<Image>();

        Init();
    }

    public void Init()
    {
        foreach (Image image in images)
        {
            Color clr = image.color;
            Color newColor = new Color(clr.r, clr.g, clr.b, 0);
            image.color = newColor;
            image.gameObject.GetComponent<Button>().interactable = false;
        }

        foreach (Text value in values)
        {
            value.color = Color.black;
            Color clr = value.color;
            Color newColor = new Color(clr.r, clr.g, clr.b, 0);
            value.color = newColor;
        }
        isFading = false;
        forceChangeState = false;
        currentTimeStatement = gameManager.GetTimeStatement;
    }

    public override void Enter()
    {
        Init();

        numbersCollection = gameManager.GetNumbersCollection();
        numbersList.Clear();
        foreach (APINumbers number in numbersCollection.numbers)
        {
            numbersList.AddFirst(number);
        }
        numbersList.Remove(gameManager.GetCurrentNumber());
        correctIndex = Random.Range(0, values.Length);
        int count = 0;
        Console.Log(" Result : " +correctIndex);
        foreach (Text value in values)
        {
            value.text = count == correctIndex ? gameManager.GetValue() + "" : GetRandomNumber().value + "";
            count++;
        }

        base.Enter();

        gameManager.ExecuteCoroutine(FadeIn());
    }

    public override void Leave()
    {
        base.Leave();        
    }

    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);
        if (gameManager.GetStateManager().state == State.ANSWER && gameManager.NextStatement || forceChangeState)
        {
            currentTimeStatement -= Time.deltaTime;

            if (currentTimeStatement < 0)
            {
                Console.Log("change");                
                StateManager.Instance.ChangeTo(State.STATEMENT);
                currentTimeStatement = gameManager.GetTimeStatement;

            }
            else if (!isFading)
            {
                Console.Log("fadeout");
                values[correctIndex].color = Color.green;
                gameManager.ExecuteCoroutine(FadeOut());
                isFading = true;
            }
        }

        if (gameManager.ErrorGO && gameManager.CountCurrentErrors == 1)
        {
            gameManager.ErrorGO.GetComponentInChildren<Text>().color = Color.red;
            gameManager.ExecuteCoroutine(FadeOut(gameManager.ErrorGO));
            gameManager.ResetErrorGO();
            
        }
        else if (gameManager.ErrorGO && gameManager.CountCurrentErrors == 2)
        {
            //gameManager.ExecuteCoroutine(FadeOut(gameManager.ErrorGO));

            //gameManager.ExecuteCoroutine(FadeOut(values[correctIndex].transform.parent.gameObject));
            gameManager.ErrorGO.GetComponentInChildren<Text>().color = Color.red;
            gameManager.ResetErrorGO();
            forceChangeState = true;
        }
    }

    IEnumerator FadeIn()
    {
        float duration = gameManager.GetTimeStatement;
        float startTime = Time.time;

        while (Time.time < startTime + duration)
        {
            foreach (Image image in images)
            {
                Color clr = image.color;
                Color newColor = new Color(clr.r, clr.g, clr.b, clr.a + (Time.deltaTime / duration));
                image.color = newColor;
                if(clr.a >= 0.99)
                    image.gameObject.GetComponent<Button>().interactable = true;
            }
            foreach (Text value in values)
            {
                Color clr = value.color;
                Color newColor = new Color(clr.r, clr.g, clr.b, clr.a + (Time.deltaTime / duration));
                value.color = newColor;
            }

            yield return null;
        }
    }

    IEnumerator FadeOut()
    {
        //go.GetComponent<Animator>().SetBool("Leave", true);

        float duration = gameManager.GetTimeStatement;
        float startTime = Time.time;

        while (Time.time < startTime + duration)
        {
            foreach (Image image in images)
            {
                Color clr = image.color;
                Color newColor = new Color(clr.r, clr.g, clr.b, clr.a - (Time.deltaTime / duration));
                image.color = newColor;
                image.gameObject.GetComponent<Button>().interactable = false;
            }
            foreach (Text value in values)
            {
                Color clr = value.color;
                Color newColor = new Color(clr.r, clr.g, clr.b, clr.a - (Time.deltaTime / duration));
                value.color = newColor;
            }
            
            yield return null;
        }
        yield return new WaitForSeconds(duration);
        isFading = false;
    }

    IEnumerator FadeOut(GameObject gameObject)
    {
        float duration = gameManager.GetTimeStatement;
        float startTime = Time.time;

        Image image = gameObject.GetComponentInChildren<Image>();
        Text value = gameObject.GetComponentInChildren<Text>();

        while (Time.time < startTime + duration)
        {

            Color clr = image.color;
            Color newColor = new Color(clr.r, clr.g, clr.b, clr.a - (Time.deltaTime / duration));
            image.color = newColor;
            image.gameObject.GetComponent<Button>().interactable = false;

            clr = value.color;
            newColor = new Color(clr.r, clr.g, clr.b, clr.a - (Time.deltaTime / duration));
            value.color = newColor;
            
            yield return null;
        }

    }

    public APINumbers GetRandomNumber()
    {        
        int l = numbersList.Count();
        int rand = Random.Range(0, l);

        APINumbers n = numbersList.ElementAt<APINumbers>(rand);

        numbersList.Remove(n);
        return n;
    }
}
