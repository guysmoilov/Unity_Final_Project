using UnityEngine;
using System.Collections;

public class SheriffController : MonoBehaviour 
{
	StackFSM brain;
	TextMesh textMesh;
	Animator animator;
	PathSeeker seeker;

	public float banditDetectionDistance;
	public GameObject bandit;

	// Use this for initialization
	void Start () 
	{
		brain = GetComponent<StackFSM>();
		animator = GetComponent<Animator>();
		textMesh = GetComponentInChildren<TextMesh>();
		seeker = GetComponent<PathSeeker>();

		bandit = GameObject.Find("Bandit");

		brain.PushState(StartPatrolState);
	}

	protected void GenerateNewSeekTarget()
	{
		seeker.SetTarget (new Vector3(Random.Range (-120, 120), Random.Range (-120, 120), Random.Range (-120, 120)));
	}

	void StartPatrolState()
	{
		Debug.Log("Sheriff: Starting patrol");
		animator.Play("Moving");
		GenerateNewSeekTarget();

		brain.PopState();
		brain.PushState(PatrolState);
	}

	void PatrolState()
	{
		textMesh.text = "Sheriff: Patrolling the perimeter.";

		if (seeker.SeekPath())
		{
			GenerateNewSeekTarget();
		}
		else
		{
			if(Vector3.Distance(transform.position, bandit.transform.position) < banditDetectionDistance &&
			   bandit.GetComponent<StackFSM>().PeekState() != bandit.GetComponent<BanditController>().Dead)
			{
				brain.PopState();
				brain.PushState(StartPatrolState);
				brain.PushState(ChaseState);
			}
		}
	}

	void ChaseState()
	{
		textMesh.text = "Sheriff: Chasing bandit!";
	}
}
