using UnityEngine;
using System.Collections;

public class MinerController : MonoBehaviour
{
	private float TIME_FOR_RECALC = 0.3f;
	public float MinDistanceToDie = 3.0f;
	public float MinDistanceToFlee = 15.0f;
	public float FleeSpeed = 250f;
	private StackFSM brain;
	private PathSeeker seeker;
	private Animator animator;
	public GameObject Home;
	public GameObject Mine;
	private GameObject Bandit;
	private float timeIdle;
	private float timeRecalcPath;
	private TextMesh textMesh;
	private bool isStuckWhileRunning = false;
	
	void Start ()
	{
		brain = this.GetComponent<StackFSM> ();
		seeker = this.GetComponent<PathSeeker> ();
		animator = this.GetComponent<Animator> ();
		textMesh = GetComponentInChildren<TextMesh>();
		Bandit = GameObject.Find ("Bandit");
		brain.PushState (CalcStateToMine);
		textMesh.text = "Miner: Zzz...";
		animator.Play ("Idle");
		brain.PushState (Idle);
	}
	
	void Reset()
	{
		timeIdle = 0;
		timeRecalcPath = 0;
		isStuckWhileRunning = false;
	}
	
	void Idle() 
	{
		timeIdle += Time.deltaTime;
		if (timeIdle > 3.2f)// second has passed
		{ 
			timeIdle = 0;
			brain.PopState ();
		}
		else
		{
			if (Vector3.Distance (Bandit.transform.position, this.transform.position) < MinDistanceToFlee)
			{
				Reset();
				brain.PopState();
				animator.Play("Running");
				textMesh.text = "Miner: Fleeing";
				brain.PushState(Flee);
			}
		}
	}

	private void WalkToMine()
	{
		textMesh.text = "Miner: Going Mine";
		if (seeker.SeekPath ())  // If we reached destination, go idle
		{	
			brain.PopState ();
			brain.PushState (CalcStateToHome);
			brain.PushState (Idle);
			animator.Play ("Idle");
			textMesh.text = "Miner: Mining";
		} 
		else
		{
			if (Vector3.Distance (Bandit.transform.position, this.transform.position) < MinDistanceToFlee) {
				Reset();
				animator.Play("Running");
				textMesh.text = "Miner: Fleeing";
				brain.PushState (Flee);
			}
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
		else
		{
			if (Vector3.Distance (Bandit.transform.position, this.transform.position) < MinDistanceToFlee) {
				Reset();
				animator.Play("Running");
				textMesh.text = "Miner: Fleeing";
				brain.PushState (Flee);
			}
		}
	}
	
	public void Dead()
	{
		textMesh.text = "Miner: Dead";
		Reset ();
		// Add the undertaker a body to take
		GameObject.Find ("Undertaker").GetComponent<UndertakerController> ().corpses.Enqueue (this.transform);
	}
	
	void Flee()
	{

		if (!isStuckWhileRunning)
		{
			Vector3 runTo = (this.transform.position - Bandit.transform.position).normalized * Time.deltaTime * FleeSpeed; // The vector to run
			transform.forward = runTo;
			CharacterController controller = GetComponent<CharacterController> ();
			controller.SimpleMove (runTo);
			Vector3 rayCastSource = transform.position + new Vector3(0,1,0) + runTo.normalized ;
			isStuckWhileRunning = Physics.Raycast(rayCastSource,runTo,2f);
			if(isStuckWhileRunning)
			{
				animator.Play ("Idle");
			}
		}

		if (Vector3.Distance (Bandit.transform.position, this.transform.position) < MinDistanceToDie)
		{
			print("Miner died");
			Reset();
			brain.PopState ();
			animator.enabled = false;
			brain.PushState(Dead);
		}

		if (Vector3.Distance (Bandit.transform.position, this.transform.position) > MinDistanceToFlee) {
			Reset();
			brain.PopState();
		}
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
