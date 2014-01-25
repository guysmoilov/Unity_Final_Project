using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class StackFSM : MonoBehaviour 
{
	protected Stack<Action> stack;

	// Use this for initialization
	void Start () 
	{
		stack = new Stack<Action>();
		stack.Push(null);
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (stack.Peek() != null)
		{
			stack.Peek().Invoke();
		}
	}

	public Action PopState()
	{
		if (stack.Peek() != null)
		{
			return stack.Pop();
		}

		return null;
	}

	public void PushState(Action state)
	{
		if (stack.Peek() != state)
		{
			stack.Push(state);
		}
	}
}
