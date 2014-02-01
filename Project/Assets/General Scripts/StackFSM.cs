using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class StackFSM : MonoBehaviour 
{
	protected Stack<Action> stack;

	void Awake () 
	{
		stack = new Stack<Action>();
		stack.Push(null);
	}

	void FixedUpdate () 
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

	public Action PeekState()
	{
		return stack.Peek();
	}

	public void PushState(Action state)
	{
		if (stack.Peek() != state)
		{
			stack.Push(state);
		}
	}
}
