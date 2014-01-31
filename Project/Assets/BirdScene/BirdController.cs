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
	public float separationRadius = float.PositiveInfinity;
	public float maxSeparation = 1;

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
		steering.Wander(wanderCircleRadius, wanderCircleDistance, wanderAngleChange);

		velocity = steering.Update();
		transform.LookAt(transform.position + velocity);
		transform.Translate(velocity, Space.World);
	}

	void FollowingState()
	{
		steering.Seek(leaderBird.GetPosition(), seekSlowingRadius);
		steering.Separation(otherBirdBoids, separationRadius, maxSeparation);
		
		velocity = steering.Update();
		transform.LookAt(transform.position + velocity);
		transform.Translate(velocity, Space.World);
	}

	void WanderingState()
	{
		steering.Wander(wanderCircleRadius, wanderCircleDistance, wanderAngleChange);
		
		velocity = steering.Update();
		transform.LookAt(transform.position + velocity);
		transform.Translate(velocity, Space.World);
	}
}
