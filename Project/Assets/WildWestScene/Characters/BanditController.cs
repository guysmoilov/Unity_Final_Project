using UnityEngine;
using System.Collections;

public class BanditController : MonoBehaviour
{
	public float MinDistanceForChase = 20.0f;
	private const float MIN_TIME_TO_RECALC_PATH = 0.2f;
	private StackFSM brain;
	private PathSeeker seeker;
	private Animator animator;
	private TextMesh textMesh;
	private GameObject MinerToChase;
	private float deltaTimeToRecalcPath;


	void Start () 
	{
		brain = this.GetComponent<StackFSM> ();
		seeker = this.GetComponent<PathSeeker> ();
		animator = this.GetComponent<Animator> ();
		textMesh = GetComponentInChildren<TextMesh>();

		MinerToChase = GameObject.Find ("Miner");
		RegenrateNewTarget ();
		textMesh.text = "Bandit: Roaming";
		brain.PushState (Wander);
		animator.Play ("Moving");

		deltaTimeToRecalcPath = 0;

	}

	// Regenerate random points to walk to
	private void RegenrateNewTarget()
	{
		seeker.SetTarget (new Vector3(Random.Range (-120, 120), Random.Range (-120, 120), Random.Range (-120, 120)));
	}

	void Wander()
	{
		if (seeker.SeekPath ())// if there is no path, or we reached the end of the path
		{ 
						RegenrateNewTarget ();
		}
		else 
		{
			// If miner is close and alive, chase him
			if(Vector3.Distance(MinerToChase.transform.position,transform.position) < MinDistanceForChase &&
			   MinerToChase.GetComponent<StackFSM>().PeekState() != MinerToChase.GetComponent<MinerController>().Dead)
			{
				seeker.SetTarget(MinerToChase.transform.position);
				animator.Play("Running");
				textMesh.text = "Bandit: Chasing Miner";
				brain.PushState(Chase);
			}
		}
	}

	void Chase()
	{
		// If miner is far or dead, abandon the chase
		if (Vector3.Distance (MinerToChase.transform.position, transform.position) > MinDistanceForChase + 2.0f ||
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

	}

	void Flee()
	{
	}
}
