using UnityEngine;
using System.Collections;

public class BirdController : MonoBehaviour, IBoid 
{
	protected SteeringManager steering;

	protected Vector3 velocity;
	public float maxVelocity = float.MaxValue;
	public float mass;

	public float seekSlowingRadius;

	public float wanderCircleDistance = 2;
	public float wanderCircleRadius = 1;
	public float wanderAngleChange = 5;

	public Transform seekTarget;

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
		steering.maxForce = maxVelocity / 10;
		//Debug.Log("Max velocity: " + maxVelocity);
		velocity = Vector3.forward;
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
//		if (seekTarget != null)
//		{
//			steering.Seek(seekTarget.position, seekSlowingRadius);
//		}

		steering.Wander(wanderCircleRadius, wanderCircleDistance, wanderAngleChange);

		velocity = steering.Update();
		transform.LookAt(transform.position + velocity);
		transform.Translate(velocity, Space.World);
	}
}
