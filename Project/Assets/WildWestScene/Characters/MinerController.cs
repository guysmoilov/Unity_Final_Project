using UnityEngine;
using System.Collections;

public class MinerController : MonoBehaviour
{
	private float TIME_FOR_RECALC = 0.3f;
	public float MinDistanceToFlee = 15.0f;
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
		brain.PushState (Idle);
		animator.Play ("Idle");
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
				brain.PushState (Flee);
			}
		}
	}
	
	public void Dead()
	{
		textMesh.text = "Miner: Dead";
		Reset ();
		// Add the undertaker a body to take
		GameObject.Find ("UnderTaker").GetComponent<UndertakerController> ().corpses.Enqueue (this.transform);
		
	}

	void Flee()
	{
		
		if (!isStuckWhileRunning)
		{
			Vector3 runTo = (this.transform.position - Bandit.transform.position).normalized; // The vector to run
			CharacterController controller = GetComponent<CharacterController> ();
			controller.SimpleMove (runTo);
			
			isStuckWhileRunning = Physics.Raycast(transform.position + new Vector3(0,1,0),runTo*3.0f,5f);
			Debug.DrawRay(transform.position,runTo,Color.green,11f);
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
