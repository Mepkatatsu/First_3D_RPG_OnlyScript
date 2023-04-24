using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class StateMachine<T>
{
    #region Variables

    private T _context;

    private State<T> _currentState;
    public State<T> CurrentState => _currentState;

    private float _elapsedTimeInState = 0;
    public float ElapsedTimeInState => _elapsedTimeInState;

    private Dictionary<System.Type, State<T>> _states = new();

    #endregion Variabels

    public StateMachine(T context, State<T> initialState)
    {
        _context = context;

        // Setup our initial state
        AddState(initialState);
        _currentState = initialState;
        _currentState.OnEnter();
    }

    #region Methods

    // State 추가
    public void AddState(State<T> state)
    {
        state.SetMachineAndContext(this, _context);
        _states[state.GetType()] = state;
    }

    public void Update(float deltaTime)
    {
        _elapsedTimeInState += deltaTime;

        _currentState.PreUpdate();
        _currentState.Update(deltaTime);
    }

    // 현재 State를 변경
    public R ChangeState<R>() where R : State<T>
    {
        var newType = typeof(R);
        if (_currentState.GetType() == newType)
        {
            return _currentState as R;
        }

        if (_currentState != null)
        {
            _currentState.OnExit();
        }


#if UNITY_EDITOR
        if (!_states.ContainsKey(newType))
        {
            var error = GetType() + ": state " + newType + " does not exist. Did you forget to add it by calling addState?";
            Debug.LogError("error");
            throw new Exception(error);
        }
#endif

        _currentState = _states[newType];
        _currentState.OnEnter();
        _elapsedTimeInState = 0.0f;

        return _currentState as R;
    }

    #endregion Methods
}
