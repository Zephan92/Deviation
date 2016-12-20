using Assets.Scripts.Enum;
using Assets.Scripts.Library;
using System.Collections.Generic;
using UnityEngine;

public class Module
{
	//Module Name
	public string Name;

	//This is the Module Type
	public ModuleType Type;

	//this is the module ui texture use in the player controls
	public Color ModuleTexture;

	//this is the current number of actions on this module
	public int ActionCount;

	//this is the max number of actions allowed on this module
	private int _maxActions;

	//this is the list of actions on this module
	private LinkedList<Action> _actions;

	//this is the current action on this module
	private LinkedListNode<Action> _currentAction;

	public Module(string name, string [] actionNames, ModuleType type, Color moduleTexture, int maxActions)
	{
		Name = name;
		Type = type;
		ModuleTexture = moduleTexture;
		_maxActions = maxActions;
		_actions = new LinkedList<Action>();
		ActionCount = 0;
		
		//for each action name, find corresponding action in the Action Library Table
		foreach (string actionName in actionNames)
		{
			ActionCount++;
			if (ActionLibrary.ActionLibraryTable.ContainsKey(actionName))
			{
				SetAction(ActionLibrary.ActionLibraryTable[actionName]);
			}
			else
			{
				//if not found, use generic
				SetAction(ActionLibrary.ActionLibraryTable["default"]);
				Debug.LogError(name + " - Module: The \"" + actionName + "\" Action was not in the Action Dictionary");
			}
		}
	}

	//adds the specified action to the list
	private void SetAction(Action action)
	{
		if (_actions.First != null)
		{
			_actions.AddAfter(_actions.Last, action);
		}
		else
		{
			_actions.AddFirst(action);
			_currentAction = _actions.First;
		}
	}

	//returns the current action
	public Action GetCurrentAction()
	{
		return _currentAction.Value;
	}

	//returns the previous action in the list
	public Action GetLeftAction()
	{
		if (_currentAction.Previous != null)
		{
			return _currentAction.Previous.Value;
		}
		else
		{
			return _actions.Last.Value;
		}
	}

	//returns the next action in the list
	public Action GetRightAction()
	{
		if (_currentAction.Next != null)
		{
			return _currentAction.Next.Value;
		}
		else
		{
			return _actions.First.Value;
		}
	}

	//changes the current action to the previous one
	public void CycleActionLeft()
	{
		if (_currentAction.Previous != null)
		{
			_currentAction = _currentAction.Previous;
		}
		else
		{
			_currentAction = _actions.Last;
		}
	}

	//changes the current action to the next one
	public void CycleActionRight()
	{
		if (_currentAction.Next != null)
		{
			_currentAction = _currentAction.Next;
		}
		else
		{
			_currentAction = _actions.First;
		}
	}
}