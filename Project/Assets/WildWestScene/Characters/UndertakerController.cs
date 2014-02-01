using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UndertakerController : MonoBehaviour 
{
	StackFSM brain;
	TextMesh textMesh;

	public Queue<Transform> corpses = new Queue<Transform>();
	
	// Use this for initialization
	void Start () 
	{
		brain = GetComponent<StackFSM>();
		textMesh = GetComponentInChildren<TextMesh>();

		brain.PushState(WaitState);
	}

	void WaitState()
	{
		textMesh.text = "Undertaker: Just waiting for someone to die.";

		if (corpses.Count > 0)
		{
			Debug.Log("Undertaker exiting WaitState: found " + corpses.Peek().gameObject.name + "'s corpse");
			brain.PushState(WalkToBodyState);
		}
	}

	void WalkToBodyState()
	{
		textMesh.text = "Undertaker: Going to get " + corpses.Peek().gameObject.name + "'s corpse.";
	}

	void ReturnBodyState()
	{
		textMesh.text = "Undertaker: returning " + corpses.Peek().gameObject.name + "'s corpse to the graveyard.";
	}
}
