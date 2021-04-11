using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AState : IState
{
    protected GameObject go;
    protected string TAG;

    protected GameManager gameManager;

    public AState(string TAG)
    {
        this.TAG = TAG;
        go = GameManager.Instance.FindInScene(TAG);
        go.SetActive(false);
        gameManager = GameManager.Instance;
    }


    virtual public void Enter()
    {
        go.SetActive(true);
    }

    virtual public IState GetParent()
    {
        return null;
    }

    virtual public void Leave()
    {
        go.SetActive(false);
    }

    virtual public void OnGUI()
    {
        Console.Log("Not implemented");
    }

    virtual public void Pause()
    {
        Console.Log("Not implemented");
    }

    virtual public void Resume()
    {
        Console.Log("Not implemented");
    }

    virtual public void Update(float deltaTime)
    {

    }
}
