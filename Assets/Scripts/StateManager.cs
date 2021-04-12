using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void OnStateChangeHandler();
public enum State { STATEMENT, ANSWER }
public class StateManager
{

    private static StateManager instance = null;
    public event OnStateChangeHandler OnStateChange;
    public State state { get; private set; }

    private Dictionary<State, IState> m_states = new Dictionary<State, IState>();
    private IState m_current;

    protected StateManager()
    {
        foreach (State name in Enum.GetValues(typeof(State)))
            m_states.Add(name, null);
    }

    public static StateManager Instance
    {
        get
        {
            if (StateManager.instance == null)
            {
                Console.Log("StateManager don't exit");
                StateManager.instance = new StateManager();
            }
            else
            {
                Console.Log("StateManager exist");
            }
            return StateManager.instance;
        }
    }

    public void RegisterState(State name, IState state)
    {
        m_states[name] = state;
    }

    public void ChangeTo(State name)
    {
        if (m_states[name].GetParent() != null && !m_states[name].GetParent().Equals(m_states[state]))
        {
            return;
        }
        /*
        //Check same state
        if (state.Equals(name))
            return;
        */
        if (m_current != null)
        {
            m_current.Leave();
        }
        SetGameState(name);
        m_current = m_states[name];
        m_current.Enter();
        //SetGameState(name);
    }

    private void SetGameState(State state)
    {
        this.state = state;
        OnStateChange();
    }

    public void Update(float deltaTime)
    {
        if (m_current != null)
        {
            m_current.Update(deltaTime);
        }
    }

    public void Pause()
    {
        if (m_current != null)
        {
            m_current.Pause();
        }

    }

    public void Resume()
    {
        if (m_current != null)
        {
            m_current.Resume();
        }

    }

    public void OnGUI()
    {
        if (m_current != null)
        {
            m_current.OnGUI();
        }
    }

    public void OnApplicationQuit()
    {
        Console.Log("Quit app -> StateManager instance null");
        StateManager.instance = null;
    }
}
