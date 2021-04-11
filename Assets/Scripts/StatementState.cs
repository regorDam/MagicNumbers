using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatementState : AState
{
    float currentTimeStatement;
    bool isFading = false;

    public StatementState():base("Statement")
    {
        go.SetActive(false);
        currentTimeStatement = gameManager.GetTimeStatement + 2;
    }

    public override void Enter()
    {
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
