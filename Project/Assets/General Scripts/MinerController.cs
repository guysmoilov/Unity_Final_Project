using UnityEngine;
using System.Collections;

public class MinerController : MonoBehaviour
{
	private StackFSM brain;
	private PathSeeker seeker;
	private Animator animation;
	public GameObject Home;
	public GameObject Mine;
	private float timeIdle;

	void Start ()
	{
		brain = this.GetComponent<StackFSM> ();
		seeker = this.GetComponent<PathSeeker> ();
		animation = this.GetComponent<Animator> ();
		brain.PushState (CalcStateToMine);
	    brain.PushState (Idle);
		animation.Play ("Idle");
		timeIdle = 0;
	}

	void Idle() 
	{
			timeIdle += Time.deltaTime;
			if (timeIdle > 3.2f) // second has passed
			{
				timeIdle = 0;
				brain.PopState ();
			}
	}

	private void WalkToMine()
	{
		if (seeker.SeekPath ()) // If we reached destination, go idle
		{	
			brain.PopState();
			brain.PushState(CalcStateToHome);
			brain.PushState(Idle);
			animation.Play ("Idle");
		}
	}

	private void WalkToHome()
	{
		if (seeker.SeekPath ()) // If we reached destination, go idle
		{	
			brain.PopState();
			brain.PushState(CalcStateToMine);
			brain.PushState(Idle);
			animation.Play ("Idle");
		}
	}

	// NOT A STATE, just set target as home and set WalkingPath State
	void CalcStateToHome()
	{
		animation.Play ("Moving");
		seeker.SetTarget (Home.transform.position);
		brain.PopState ();
		brain.PushState (WalkToHome);
	}

	// NOT A STATE, just set target  as mine and set WalkingPath State
	void CalcStateToMine()
	{
		animation.Play ("Moving");
		seeker.SetTarget (Mine.transform.position);
		brain.PopState ();
		brain.PushState (WalkToMine);
	}
}
