using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class AnswerState : AState
{
    Text[] values;
    public AnswerState():base("Slots")
    {
        go.SetActive(false);
        values = go.GetComponentsInChildren<Text>();      
    }

    public override void Enter()
    {
        int correct = Random.Range(0,values.Length+1);
        int count = 0;
        foreach(Text value in values)
        {            
            value.text = count==correct?gameManager.GetValue()+"":gameManager.GetRandomValue()+"";
            count++;
        }
        base.Enter();
    }

    public override void Leave()
    {
        base.Leave();
    }

    private void FadeOut()
    {
        go.GetComponent<Animator>().SetBool("Leave", true);
    }

}
