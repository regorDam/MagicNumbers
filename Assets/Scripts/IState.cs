using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState 
{
    void Enter();
    void Leave();
    void Pause();
    void Resume();

    void Update(float deltaTime);
    void OnGUI();


    IState GetParent();
}
