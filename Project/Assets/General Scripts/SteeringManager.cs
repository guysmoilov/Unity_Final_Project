using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface IBoid
{
	Vector3 GetVelocity();
	float GetMaxVelocity();
	Vector3 GetPosition();
	float GetMass();
}


public class SteeringManager
{
	public Vector3 steering;
	public IBoid host;
	
	public float maxForce = float.MaxValue;
	
	public SteeringManager(IBoid host)
	{
		this.host = host;
		this.steering = Vector3.zero;
	}
	
	#region Behaviours
	
	public void Seek(Vector3 target, float slowingRadius)
	{
		steering += DoSeek(target, slowingRadius);
	}
	
	public void Wander(float circleRadius, float circleDistance, float angleChange)
	{
		steering += DoWander(circleRadius, circleDistance, angleChange);
	}
	
	public void Separation(IList<IBoid> others, float separationRadius, float maxSeparation)
	{
		steering += DoSeparation(others, separationRadius, maxSeparation);
	}
<<<<<<< HEAD
	
=======

	public void Bounds(Vector3 minBounds, Vector3 maxBounds)
	{
		steering += DoBounds(minBounds, maxBounds);
	}

>>>>>>> b76e98646d6faa10c090f0354d72b130e490e7ac
	#endregion
	
	#region BehaviourImplementation
	
	protected virtual Vector3 DoSeek(Vector3 target, float slowingRadius)
	{
		Vector3 force;
		float distance;
		
		var desired = target - host.GetPosition();
		
		distance = desired.magnitude;
		desired.Normalize();
		
		if (distance <= slowingRadius)
		{
			desired *= host.GetMaxVelocity() * distance / slowingRadius;
		}
		else
		{
			desired *= host.GetMaxVelocity();
		}
		
		force = desired - host.GetVelocity();
		
		return force;
	}
	
	protected float wanderAngleX;
	protected float wanderAngleY;
	protected virtual Vector3 DoWander(float circleRadius, float circleDistance, float angleChange)
	{
		var circleCenter = host.GetVelocity().normalized;
		circleCenter *= circleDistance;
		
		var displacement = Vector3.forward * circleRadius;
		displacement = Quaternion.AngleAxis(wanderAngleX, Vector3.up) * displacement;
		displacement = Quaternion.AngleAxis(wanderAngleY, Vector3.forward) * displacement;
		
		wanderAngleX += Random.value * angleChange - angleChange * 0.5f;
		wanderAngleY += Random.value * angleChange - angleChange * 0.5f;
		
		var wanderForce = circleCenter + displacement;
		return wanderForce;
	}
	
	protected virtual Vector3 DoSeparation(IList<IBoid> others, float separationRadius, float maxSeparation)
	{
		var force = Vector3.zero;
		var closeBoidsCount = 0;
		
		foreach (var boid in others)
		{
			if (boid != host && Vector3.Distance(boid.GetPosition(), host.GetPosition()) <= separationRadius)
			{
				force += host.GetPosition() - boid.GetPosition();
				closeBoidsCount++;
			}
		}
		
		if (closeBoidsCount > 0)
		{
			force /= closeBoidsCount;
			force.Normalize();
			force *= maxSeparation;
		}
<<<<<<< HEAD
		
		force.Normalize();
		force *= maxSeparation;
		Debug.Log(force);
		return force;
	}
	
=======

		return force;
	}

	protected virtual Vector3 DoBounds(Vector3 minBounds, Vector3 maxBounds)
	{
		var minBoundDiff = minBounds - host.GetPosition();
		var maxBoundDiff = maxBounds - host.GetPosition();

		if (minBoundDiff.x < 0)
		{
			minBoundDiff.x = 0;
		}
		if (minBoundDiff.y < 0)
		{
			minBoundDiff.y = 0;
		}
		if (minBoundDiff.z < 0)
		{
			minBoundDiff.z = 0;
		}

		if (maxBoundDiff.x > 0)
		{
			maxBoundDiff.x = 0;
		}
		if (maxBoundDiff.y > 0)
		{
			maxBoundDiff.y = 0;
		}
		if (maxBoundDiff.z > 0)
		{
			maxBoundDiff.z = 0;
		}

		return minBoundDiff + maxBoundDiff;
	}

>>>>>>> b76e98646d6faa10c090f0354d72b130e490e7ac
	#endregion
	
	public Vector3 Update ()
	{
		var velocity = host.GetVelocity();
<<<<<<< HEAD
		//Debug.Log("Initial velocity: " + velocity);
		
		//Debug.Log("steering before:" + steering);
		steering = Vector3.ClampMagnitude(steering, maxForce);
		steering /= host.GetMass();
		//Debug.Log("steering after:" + steering);
		
=======

		steering = Vector3.ClampMagnitude(steering, maxForce);
		steering /= host.GetMass();

>>>>>>> b76e98646d6faa10c090f0354d72b130e490e7ac
		velocity += steering;
		velocity = Vector3.ClampMagnitude(velocity, host.GetMaxVelocity());
<<<<<<< HEAD
		//Debug.Log("velocity after:" + velocity);
		
=======

>>>>>>> b76e98646d6faa10c090f0354d72b130e490e7ac
		return velocity;
	}
	
	public void Reset()
	{
		this.steering = Vector3.zero;
	}
}
