using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathSeeker : MonoBehaviour
{
	public  float  Speed = 1f;

	public GatherPathFindingNodes AStarSceneObject;
	private Vector3 toVisit; // Point to reach

	private int currGoal; // which target is currently being visited
	private int currPathNode; // the path on the way to the target
	private PathNode[] path;
	private bool pathCalculated = false;


	// Use this for initialization
	void Start ()
	{
		Reset ();
	}

	void Reset()
	{
		currPathNode = 0;
		currGoal = 0;
		pathCalculated = false;
	}

	public void SetTarget(Vector3 target)
	{
		Reset ();
		toVisit = target;
	}

	// Seeks the path, if goal is reached, return true
	public bool SeekPath ()
	{
		if (!pathCalculated) 
		{
			path = AStarSceneObject.AStarPath (transform.position, toVisit);
			if(path == null) return true; // we can't reach the target
			// foreach(PathNode node in path)
			//{
			//	print ("Node at " + node.Position.x + ":" + node.Position.z);
			//} 
			pathCalculated = true;
		}

		if (path != null && currPathNode < path.Length)
		{
						Vector3 diff = path [currPathNode].Position - transform.position;
						diff.y = 0;
						//Vector3 toadd = diff.normalized * Time.deltaTime * Speed;
			Vector3 toadd = diff.normalized * Time.deltaTime;
						transform.forward = toadd;

						float distance = diff.magnitude;
						if (distance < 2) { // we reached a node in the path
								currPathNode++;
						}

						CharacterController controller = GetComponent<CharacterController> ();
			//print ("Adding " + toadd.ToString());
						controller.Move (toadd);
			return false;
				} else
		{ // We reached our goal return true;
					Reset();

					return true;
					//currPathNode = 0;
					//currGoal++;
					//pathCalculated = false;
				
					//if(currGoal >= toVisit) currGoal = 0;

				}
	}
}
