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
        int correct = Random.Range(0, values.Length);
        int count = 0;
        Console.Log(" Result : " +correct);
        foreach (Text value in values)
        {
            value.text = count == correct ? gameManager.GetValue() + "" : GetRandomNumber().value + "";
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
        if (gameManager.GetStateManager().state == State.ANSWER && gameManager.NextStatement)
        {
            currentTimeStatement -= Time.deltaTime;

            if (currentTimeStatement < 0)
            {
                currentTimeStatement = gameManager.GetTimeStatement;
                StateManager.Instance.ChangeTo(State.STATEMENT); 
                
            }
            else if (!isFading)
            {
                gameManager.ExecuteCoroutine(FadeOut());
                isFading = true;
            }
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
