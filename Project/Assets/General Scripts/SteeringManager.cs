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
		}
		
		force.Normalize();
		force *= maxSeparation;
		Debug.Log(force);
		return force;
	}
	
	#endregion
	
	public Vector3 Update ()
	{
		var velocity = host.GetVelocity();
		//Debug.Log("Initial velocity: " + velocity);
		
		//Debug.Log("steering before:" + steering);
		steering = Vector3.ClampMagnitude(steering, maxForce);
		steering /= host.GetMass();
		//Debug.Log("steering after:" + steering);
		
		velocity += steering;
		//Debug.Log("velocity before: " + velocity);
		velocity = Vector3.ClampMagnitude(velocity, host.GetMaxVelocity());
		//Debug.Log("velocity after:" + velocity);
		
		return velocity;
	}
	
	public void Reset()
	{
		this.steering = Vector3.zero;
	}
}