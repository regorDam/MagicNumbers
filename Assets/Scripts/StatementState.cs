using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatementState : AState
{
    float currentTimeStatement;
    bool isFading = false;

    Text label;

    public StatementState():base("Statement")
    {
        go.SetActive(false);
        label = go.GetComponent<Text>();
        currentTimeStatement = gameManager.GetTimeStatement + 2;        
    }

    public override void Enter()
    {
        string text = gameManager.GetRandomLabel();
        label.text = text.Substring(0, 1).ToUpper() + text.Substring(1);

        base.Enter();
        isFading = false;
    }


    public override void Leave()
    {        
        base.Leave();
    }

    private void FadeOut()
    {
        go.GetComponent<Animator>().SetBool("Leave", true);
    }

    public override void Update(float deltaTime)
    {
        if (gameManager.GetStateManager().state == State.STATEMENT)
        {
            currentTimeStatement -= Time.deltaTime;

            if (currentTimeStatement <= -2)
            {
                Console.Log("Leave");
                StateManager.Instance.ChangeTo(State.ANSWER);
                currentTimeStatement = gameManager.GetTimeStatement + 2;
            }
            else if (currentTimeStatement < 0 && !isFading)
            {
                FadeOut();
                isFading = true;
            }
        }
    }
}
