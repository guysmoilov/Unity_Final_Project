using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BirdController : MonoBehaviour, IBoid 
{
	protected StackFSM brain;
	protected SteeringManager steering;

	protected Vector3 velocity;
	public float maxVelocity = float.PositiveInfinity;
	public float mass = 1;

	public float seekSlowingRadius;

	public float wanderCircleDistance = 2;
	public float wanderCircleRadius = 1;
	public float wanderAngleChange = 45;

	protected List<BirdController> otherBirds;
	protected List<IBoid> otherBirdBoids; 
	protected BirdController leaderBird;

	public float leaderFollowDistance;
	public float separationRadius = float.PositiveInfinity;
	public float maxSeparation = 1;

	public Vector3 minBounds = new Vector3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);
	public Vector3 maxBounds = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);

	#region IBoidMethods

	public virtual Vector3 GetVelocity()
	{
		return velocity;
	}
	public virtual float GetMaxVelocity()
	{
		return maxVelocity;
	}
	public virtual Vector3 GetPosition()
	{
		return transform.position;
	}
	public virtual float GetMass()
	{
		return mass;
	}

	#endregion

	// Use this for initialization
	void Start () 
	{
		// Initialize steering
		steering = new SteeringManager(this);
		maxVelocity *= Time.fixedDeltaTime;
		steering.maxForce = maxVelocity;
		velocity = Vector3.forward;

		// Initialize flocking
		brain = this.GetComponent<StackFSM>();
		
		otherBirds = new List<BirdController>();
		otherBirdBoids = new List<IBoid>();
		var birds = GameObject.FindGameObjectsWithTag("Bird");
		
		foreach (var bird in birds) 
		{
			if (bird != this.gameObject)
			{
				var birdController = bird.GetComponent<BirdController>();
				otherBirds.Add(birdController);
				otherBirdBoids.Add(birdController);
			}
		}
		
		if (this.tag == "LeaderBird")
		{
			leaderBird = this;
			brain.PushState(LeadingState);
		}
		else
		{
			var leader = GameObject.FindGameObjectWithTag("LeaderBird");
			
			if (leader != null)
			{
				leaderBird = leader.GetComponent<BirdController>();
				otherBirdBoids.Add(leaderBird);
				brain.PushState(FollowingState);
			}
			else
			{
				Debug.LogWarning("Could not find leader! wandering instead");
				brain.PushState(WanderingState);
			}
		}
	}

	void LeadingState()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			// State transition
			brain.PushState(WanderingState);
		}
		else
		{
			steering.Wander(wanderCircleRadius, wanderCircleDistance, wanderAngleChange);
			steering.Bounds(minBounds, maxBounds);
			
			velocity = steering.Update();
			transform.LookAt(transform.position + velocity);
			transform.Translate(velocity, Space.World);
		}
	}

	void FollowingState()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			// State transition
			brain.PushState(WanderingState);
		}
		else
		{
			var seekTarget = leaderBird.GetVelocity().normalized * (-1) * leaderFollowDistance;
			seekTarget += leaderBird.GetPosition();
			steering.Seek(seekTarget, 0);
			steering.Separation(otherBirdBoids, separationRadius, maxSeparation);
			
			velocity = steering.Update();
			transform.LookAt(transform.position + velocity);
			transform.Translate(velocity, Space.World);
		}
	}

	void WanderingState()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			// State transition
			brain.PopState();

			if (brain.PeekState() == null)
			{
				Debug.LogWarning("Tried to pop the last state!");
				brain.PushState(WanderingState);
			}
		}
		else
		{
			steering.Wander(wanderCircleRadius, wanderCircleDistance, wanderAngleChange);
			steering.Separation(otherBirdBoids, separationRadius, maxSeparation);
			steering.Bounds(minBounds, maxBounds);
			
			velocity = steering.Update();
			transform.LookAt(transform.position + velocity);
			transform.Translate(velocity, Space.World);
		}
	}
}
