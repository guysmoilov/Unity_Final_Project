using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathSeeker : MonoBehaviour
{

	public GatherPathFindingNodes AStarSceneObject;
	public float WalkSpeed = 130f;
	private Vector3 toVisit; // Point to reach
	
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

			currPathNode++; // start from path node 1 (node zero is probably close anyway..)
		}

		if (path != null && currPathNode < path.Length)
		{
						Vector3 diff = path [currPathNode].Position - transform.position;
						diff.y = 0;

						Vector3 toadd = diff.normalized * Time.deltaTime;
						transform.forward = toadd;
						toadd *= WalkSpeed;
						float distance = diff.magnitude;
						if (distance < 2) { // we reached a node in the path
								currPathNode++;
						}

						CharacterController controller = GetComponent<CharacterController> ();
			//print ("Adding " + toadd.ToString());
						//controller.Move (toadd);
						controller.SimpleMove (toadd);
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
