using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UndertakerController : MonoBehaviour 
{
	StackFSM brain;
	TextMesh textMesh;
	PathSeeker pathSeeker;
	Animator animator;

	public Transform graveyardPoint;

	public Queue<Transform> corpses = new Queue<Transform>();
	public Transform corpseDragPoint;

	void Start () 
	{
		brain = GetComponent<StackFSM>();
		textMesh = GetComponentInChildren<TextMesh>();
		pathSeeker = GetComponent<PathSeeker>();
		animator = GetComponent<Animator>();

		brain.PushState(EnterWaitState);
	}

	void EnterWaitState()
	{
		Debug.Log("Undertaker: entering wait state");
		animator.Play("Idle");

		brain.PopState();
		brain.PushState(WaitState);
	}

	void WaitState()
	{
		textMesh.text = "Undertaker: Just waiting for someone to die.";

		if (corpses.Count > 0)
		{
			Debug.Log("Undertaker exiting WaitState");
			brain.PushState(EnterWaitState);
			brain.PushState(FoundBodyState);
		}
	}

	void FoundBodyState()
	{
		Debug.Log("Undertaker: found " + corpses.Peek().gameObject.name + "'s corpse");
		pathSeeker.SetTarget(corpses.Peek().position);

		animator.Play("Moving");

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
		animator.Play("Moving");

		brain.PopState();
		brain.PushState(ReturnBodyState);
	}

	void ReturnBodyState()
	{
		var corpse = corpses.Peek();
		textMesh.text = "Undertaker: Returning " + corpse.gameObject.name + "'s corpse to the graveyard.";

		corpse.position = corpseDragPoint.position;

		if (pathSeeker.SeekPath())
		{
			brain.PopState();
        }
	}
}
