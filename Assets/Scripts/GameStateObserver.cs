using System;
using System.Collections.Generic;

/// <summary>
/// Main helper class to ensure coherent data reading from GameState
/// </summary>
public class GameStateObserver
{
    private readonly Dictionary<string, List<Action>> _mSubscribedActions = new Dictionary<string, List<Action>>();
    private readonly List<string> _mChangedObjects = new List<string>();
    private readonly List<Action> _mExecutedActions = new List<Action>();

    public void SubscribeObject(string targetName)
    {
        if (_mSubscribedActions.ContainsKey(targetName))
        {
            return;
        }
        _mSubscribedActions.Add(targetName, new List<Action>());
    }

    public void RegisterOnChangedAction(string target, Action a)
    {
        if (!_mSubscribedActions.ContainsKey(target))
        {
            return;
        }

        _mSubscribedActions[target].Add(a);
    }

    public void OnValueChanged(string target)
    {
        if (_mChangedObjects.Contains(target))
        {
            return;
        }

        _mChangedObjects.Add(target);
    }

    /// <summary>
    /// Must be called manually whenever you want to all the onValueChanged flow
    /// Can be used at the end of an async operation and it will ensure coherent data reading (lock the data first!)
    /// Only iterates thru changed objects, no changes means no logic executed
    /// Checks for potential null actions
    /// Only execute non repeated actions
    /// </summary>
    public void ExecuteActions()
    {
        if (_mChangedObjects.Count == 0)
        {
            return;
        }

        foreach (var target in _mChangedObjects)
        {
            foreach (var actions in _mSubscribedActions[target])
            {
                if (actions != null && !_mExecutedActions.Contains(actions))
                {
                    _mExecutedActions.Add(actions);
                    actions.Invoke();
                }
            }
        }
        _mChangedObjects.Clear();
        _mExecutedActions.Clear();
    }
}
