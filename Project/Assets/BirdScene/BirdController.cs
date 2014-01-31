using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BirdController : MonoBehaviour, IBoid 
{
	protected SteeringManager steering;

	protected Vector3 velocity;
	public float maxVelocity = float.PositiveInfinity;
	public float mass;

	public float seekSlowingRadius;

	public float wanderCircleDistance = 2;
	public float wanderCircleRadius = 1;
	public float wanderAngleChange = 5;

	public Transform seekTarget;

	public List<IBoid> otherBirds;
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
		steering = new SteeringManager(this);
		maxVelocity *= Time.fixedDeltaTime;
		steering.maxForce = maxVelocity;
		//Debug.Log("Max velocity: " + maxVelocity);
		velocity = Vector3.forward;

	}

	void Awake()
	{
		otherBirds = new List<IBoid>();
		var birds = GameObject.FindGameObjectsWithTag("Bird");

		foreach (var bird in birds) 
		{
			otherBirds.Add(bird.GetComponent<BirdController>());
		}
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		if (seekTarget != null)
		{
			steering.Seek(seekTarget.position, seekSlowingRadius);
		}

		//steering.Wander(wanderCircleRadius, wanderCircleDistance, wanderAngleChange);

		steering.Separation(otherBirds, separationRadius, maxSeparation);

		velocity = steering.Update();
		transform.LookAt(transform.position + velocity);
		transform.Translate(velocity, Space.World);
	}
}
