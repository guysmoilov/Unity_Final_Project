using UnityEngine;
using System.Collections;

public class PathSeeker : MonoBehaviour
{
	public  float  Speed = 1f;

	public GatherPathFindingNodes AStarSceneObject;
	public GameObject[] toVisit; // Array of points to visit during the life of the character, if not set, randomly roams the map.

	private int currGoal; // which target is currently being visited
	private int currPathNode; // the path on the way to the target
	private PathNode[] path;
	private bool pathCalculated = false;


	// Use this for initialization
	void Start ()
	{
		currPathNode = 0;
		currGoal = 0;
	}
	

	 void Update ()
	{
		if (!pathCalculated) 
		{
			path = AStarSceneObject.AStarPath (transform.position, toVisit[currGoal].transform.position);		
			 foreach(PathNode node in path)
			{
				print ("Node at " + node.Position.x + ":" + node.Position.z);
			} 
			pathCalculated = true;
		}

		if (path != null && currPathNode < path.Length)
		{
						Vector3 diff = path [currPathNode].Position - transform.position;
						diff.y = 0;
						Vector3 toadd = diff.normalized * Time.deltaTime * Speed;
				
						transform.forward = toadd;

						float distance = diff.magnitude;
						if (distance < 2) { // we reached a node in the path
								currPathNode++;
						}

						CharacterController controller = GetComponent<CharacterController> ();
						controller.Move (toadd);
				} else
				{ // We reached our goal, get the next one
					currPathNode = 0;
					currGoal++;
					pathCalculated = false;
				
					if(currGoal >= toVisit.Length) currGoal = 0;

				}
	}
}
