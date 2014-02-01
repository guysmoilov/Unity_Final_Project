using UnityEngine;
using System.Collections;

public class MinerController : MonoBehaviour
{
	private StackFSM brain;
	private PathSeeker seeker;
	private Animator animator;
	public GameObject Home;
	public GameObject Mine;
	private float timeIdle;
	private TextMesh textMesh;

	void Start ()
	{
		brain = this.GetComponent<StackFSM> ();
		seeker = this.GetComponent<PathSeeker> ();
		animator = this.GetComponent<Animator> ();
		textMesh = GetComponentInChildren<TextMesh>();
		brain.PushState (CalcStateToMine);
	    brain.PushState (Idle);
		animator.Play ("Idle");
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
		textMesh.text = "Miner: Going Mine";
		if (seeker.SeekPath ()) // If we reached destination, go idle
		{	
			brain.PopState();
			brain.PushState(CalcStateToHome);
			brain.PushState(Idle);
			animator.Play ("Idle");
			textMesh.text = "Miner: Mining";
		}
	}

	private void WalkToHome()
	{
		textMesh.text = "Miner: Going Home";
		if (seeker.SeekPath ()) // If we reached destination, go idle
		{	
			brain.PopState();
			brain.PushState(CalcStateToMine);
			brain.PushState(Idle);
			animator.Play ("Idle");
			textMesh.text = "Miner: Zzz...";
		}
	}

	public void Dead()
	{
	}

	// NOT A STATE, just set target as home and set WalkingPath State
	void CalcStateToHome()
	{
		animator.Play ("Moving");
		seeker.SetTarget (Home.transform.position);
		brain.PopState ();
		brain.PushState (WalkToHome);
	}

	// NOT A STATE, just set target  as mine and set WalkingPath State
	void CalcStateToMine()
	{
		animator.Play ("Moving");
		seeker.SetTarget (Mine.transform.position);
		brain.PopState ();
		brain.PushState (WalkToMine);
	}
}
