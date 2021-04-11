using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnswerState : AState
{
    public AnswerState():base("Slots")
    {
        go.SetActive(false);
    }

    public override void Enter()
    {
        base.Enter();
    }

}
