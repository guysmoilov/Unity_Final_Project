using UnityEngine;
using System.Collections;

public class BanditController : MonoBehaviour
{
	public float MinDistanceForChase = 20.0f;
	public float sheriffFleeDistance = 30f;
	public float deathDistance = 3f;
	public float fleeSpeed = 250f;

	private const float MIN_TIME_TO_RECALC_PATH = 0.2f;
	private StackFSM brain;
	private PathSeeker seeker;
	private Animator animator;
	private TextMesh textMesh;
	private GameObject MinerToChase;
	private GameObject sheriff;
	private float deltaTimeToRecalcPath;
	private bool isStuckWhileRunning = false;
	

	void Start () 
	{
		brain = this.GetComponent<StackFSM> ();
		seeker = this.GetComponent<PathSeeker> ();
		animator = this.GetComponent<Animator> ();
		textMesh = GetComponentInChildren<TextMesh>();

		MinerToChase = GameObject.Find ("Miner");
		sheriff = GameObject.Find("Sheriff");
		RegenrateNewTarget ();
		textMesh.text = "Bandit: Roaming";
		brain.PushState (StartWander);

		deltaTimeToRecalcPath = 0;

	}

	// Regenerate random points to walk to
	private void RegenrateNewTarget()
	{
		seeker.SetTarget (new Vector3(Random.Range (-120, 120), Random.Range (-120, 120), Random.Range (-120, 120)));
	}

	void StartWander()
	{
		Debug.Log("Bandit: starts wandering");
		animator.Play("Moving");

		brain.PopState();
		brain.PushState(Wander);
	}

	void Wander()
	{
		if (seeker.SeekPath ())// if there is no path, or we reached the end of the path
		{ 
			RegenrateNewTarget ();
		}
		else 
		{
			// If sheriff is close, run away
			if(Vector3.Distance(sheriff.transform.position,transform.position) < sheriffFleeDistance)
			{
				animator.Play("Running");
				brain.PopState();
				brain.PushState(Wander);
				brain.PushState(Flee);
			}
			// If miner is close and alive, chase him
			else if(Vector3.Distance(MinerToChase.transform.position,transform.position) < MinDistanceForChase &&
			   MinerToChase.GetComponent<StackFSM>().PeekState() != MinerToChase.GetComponent<MinerController>().Dead)
			{
				seeker.SetTarget(MinerToChase.transform.position);
				animator.Play("Running");
				textMesh.text = "Bandit: Chasing Miner";
				brain.PopState();
				brain.PushState(StartWander);
				brain.PushState(Chase);
			}
		}
	}

	void Chase()
	{
		// If sheriff is close, run away
		if(Vector3.Distance(sheriff.transform.position,transform.position) < sheriffFleeDistance)
		{
			brain.PushState(Flee);
		}
		// If miner is far or dead, abandon the chase
		else if (Vector3.Distance (MinerToChase.transform.position, transform.position) > MinDistanceForChase + 2.0f ||
		    MinerToChase.GetComponent<StackFSM>().PeekState() == MinerToChase.GetComponent<MinerController>().Dead)
		{
						brain.PopState ();
						animator.Play("Moving");
						textMesh.text = "Bandit: Roaming";
		}
		else
		{
			seeker.SeekPath();
			deltaTimeToRecalcPath+= Time.deltaTime;
			if(deltaTimeToRecalcPath > MIN_TIME_TO_RECALC_PATH)
			{
				deltaTimeToRecalcPath = 0;
				seeker.SetTarget(MinerToChase.transform.position);
			}
		}
	}

	public void Dead()
	{
		textMesh.text = "Bandit: DEAD";
	}

	void Flee()
	{
		textMesh.text = "Bandit: Running away from the fuzz!";

		// If sheriff is far away, stop running
		if (Vector3.Distance(sheriff.transform.position,transform.position) > sheriffFleeDistance)
		{
			brain.PopState();
		}
		else if (Vector3.Distance(sheriff.transform.position,transform.position) < deathDistance)
		{
			Debug.Log("Bandit: caught by sheriff");

			var undertaker = GameObject.Find("Undertaker").GetComponent<UndertakerController>();
			undertaker.corpses.Enqueue(this.transform);
			brain.PushState(Dead);
		}
		else
		{
			if (!isStuckWhileRunning)
			{
				Vector3 runTo = (this.transform.position - sheriff.transform.position).normalized * Time.deltaTime * fleeSpeed; // The vector to run
				transform.forward = runTo;
				CharacterController controller = GetComponent<CharacterController> ();
				controller.SimpleMove (runTo);
				Vector3 rayCastSource = transform.position + new Vector3(0,1,0) + runTo.normalized ;
				// isStuckWhileRunning = Physics.Raycast(rayCastSource,runTo,2f);
				RaycastHit hitInfo;
				Physics.Raycast(rayCastSource,runTo,out hitInfo,2f);
				if(hitInfo.collider != null && hitInfo.collider.gameObject.name == "Terrain")
				{
					isStuckWhileRunning = true;
				}
				if(isStuckWhileRunning)
				{
					animator.Play ("Idle");
				}
			}
		}
	}
}
