using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UndertakerController : MonoBehaviour 
{
	StackFSM brain;
	TextMesh textMesh;
	PathSeeker pathSeeker;

	public Transform graveyardPoint;

	public Queue<Transform> corpses = new Queue<Transform>();
	
	// Use this for initialization
	void Start () 
	{
		brain = GetComponent<StackFSM>();
		textMesh = GetComponentInChildren<TextMesh>();
		pathSeeker = GetComponent<PathSeeker>();

		brain.PushState(WaitState);
	}

	void WaitState()
	{
		textMesh.text = "Undertaker: Just waiting for someone to die.";

		if (corpses.Count > 0)
		{
			Debug.Log("Undertaker exiting WaitState");
			brain.PushState(FoundBodyState);
		}
	}

	void FoundBodyState()
	{
		Debug.Log("Undertaker: found " + corpses.Peek().gameObject.name + "'s corpse");
		pathSeeker.SetTarget(corpses.Peek().position);

		brain.PopState();
		brain.PushState(WalkToBodyState);
	}

	void WalkToBodyState()
	{
		textMesh.text = "Undertaker: Going to get " + corpses.Peek().gameObject.name + "'s corpse.";

		if (pathSeeker.SeekPath())
		{
			Debug.Log("Undertaker: Got to " + corpses.Peek().gameObject.name + "'s corpse");

			brain.PopState();
			brain.PushState(StartReturnToGraveyardState);
		}
	}

	void StartReturnToGraveyardState()
	{
		Debug.Log("Undertaker: returning to graveyard");
		pathSeeker.SetTarget(graveyardPoint.position);

		brain.PopState();
		brain.PushState(ReturnBodyState);
	}

	void ReturnBodyState()
	{
		textMesh.text = "Undertaker: Returning " + corpses.Peek().gameObject.name + "'s corpse to the graveyard.";

		if (pathSeeker.SeekPath())
		{
			Debug.Log("Undertaker: Got to graveyard");
			corpses.Dequeue();
			brain.PopState();
        }
	}
}
