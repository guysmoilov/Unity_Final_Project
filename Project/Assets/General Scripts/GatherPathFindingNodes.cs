using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GatherPathFindingNodes : MonoBehaviour
{
	private const float NODE_SIZE = 2.5f; // How much the each node covers in pixel
	private bool[,] GraphOfTerrain;
	private PathNode[,] GraphOfNodes;
	public Terrain TerrainObj;
	public float MaxSlope;

	// On Start we create the matrix of where we can step upon the terrain
	void Start ()
	{
		// Initiate the terrian to be the size of the terrain / NODE_SIZE
		GraphOfTerrain = new bool[(int)(TerrainObj.terrainData.size.x / NODE_SIZE),(int)(TerrainObj.terrainData.size.z / NODE_SIZE)];
		GraphOfNodes = new PathNode[GraphOfTerrain.GetLength(0),GraphOfTerrain.GetLength(1)];

		// Get Terrain start position
		Vector3 terrPosition = TerrainObj.transform.position;

		// Refernece to the data of a hit
		RaycastHit data;

		// Save the RayCaster Source Position
		Vector3 SourcePosition = terrPosition;

		// Set the raycaster to be above all the mountains on the terain
		SourcePosition.y += maxHeightOfTerrain() + 5;

		// Run on all the terrain and sets booleans to indicate passable blocks for AI
		for (int nRow = 0; nRow < GraphOfTerrain.GetLength(0); nRow++)
		{
			for (int nCol = 0; nCol < GraphOfTerrain.GetLength(1); nCol++)
			{
				// Shoot a raycast down on the terrain
				Physics.Raycast(SourcePosition,Vector3.down,out data,SourcePosition.y + 10);

				float angle = Mathf.Abs(Vector3.Angle(Vector3.up,data.normal));

				// If the normal of the hit was smaller than the slope, it means AI can walk here, set to true
				GraphOfTerrain[nRow,nCol] = angle < MaxSlope;

				// If we can walk here, define a Pathnode here.
				if(GraphOfTerrain[nRow,nCol])
					GraphOfNodes[nRow,nCol] = new PathNode(new Vector3(SourcePosition.x,terrPosition.y,SourcePosition.z));
				// Advance in the z axis (columns)
				SourcePosition.z += NODE_SIZE;

				// print("At " + nRow * NODE_SIZE + ":" + nCol * NODE_SIZE +  " value : " + GraphOfTerrain[nRow,nCol]);
			}

			// Start A new row on the terrain
			SourcePosition.z = terrPosition.z;
			SourcePosition.x += NODE_SIZE;
		}
		// Rerun on the boolean terrain and build a graph of neighbours
		for (int nRow = 0; nRow < GraphOfTerrain.GetLength(0); nRow++)
		{
			for (int nCol = 0; nCol < GraphOfTerrain.GetLength(1); nCol++)
			{
				if(GraphOfNodes[nRow,nCol] != null)
				{
				// Check bounds
				// Lines
				if (nRow > 0 && GraphOfNodes [nRow - 1, nCol] != null)
						GraphOfNodes [nRow, nCol].Neightbours.Add(GraphOfNodes [nRow - 1, nCol]);

				if (nRow < GraphOfTerrain.GetLength(0) - 1 && GraphOfNodes [nRow + 1, nCol] != null)
						GraphOfNodes [nRow, nCol].Neightbours.Add(GraphOfNodes [nRow +1, nCol]);

				if (nCol > 0 && GraphOfNodes [nRow, nCol - 1] != null)
						GraphOfNodes [nRow, nCol].Neightbours.Add(GraphOfNodes [nRow, nCol - 1]);

				if (nCol < GraphOfTerrain.GetLength(1) - 1 && GraphOfNodes [nRow, nCol + 1] != null)
						GraphOfNodes [nRow, nCol].Neightbours.Add(GraphOfNodes [nRow, nCol + 1]);
						
				// Crosses
				if (nRow > 0 && nCol > 0  && GraphOfNodes [nRow - 1, nCol - 1] != null)
						GraphOfNodes [nRow, nCol].Neightbours.Add(GraphOfNodes [nRow - 1, nCol - 1]);

				if (nRow < GraphOfTerrain.GetLength(0) - 1 && nCol < GraphOfTerrain.GetLength(1) - 1 && GraphOfNodes [nRow + 1, nCol + 1] != null)
						GraphOfNodes [nRow, nCol].Neightbours.Add(GraphOfNodes [nRow + 1, nCol + 1]);

				if (nRow > 0 && nCol < GraphOfTerrain.GetLength(1) - 1 && GraphOfNodes [nRow - 1, nCol + 1] != null)
						GraphOfNodes [nRow, nCol].Neightbours.Add(GraphOfNodes [nRow - 1, nCol + 1]);

				if (nRow < GraphOfTerrain.GetLength(0) - 1 && nCol > 0 && GraphOfNodes [nRow + 1, nCol - 1] != null)
						GraphOfNodes [nRow, nCol].Neightbours.Add(GraphOfNodes [nRow + 1, nCol - 1]);


					//print("GraphOfNodes[ " + nRow * NODE_SIZE + "," + nCol * NODE_SIZE +  "] added");
				}
			}
		}

	}

	// Returns the highest mountain height on the terrain
	private float maxHeightOfTerrain()
	{
		float maxHeight = 0;
		for(int i=0;i<TerrainObj.terrainData.size.x;i++)
		{
			for(int j=0;j<TerrainObj.terrainData.size.z;j++)
			{
				if(maxHeight < TerrainObj.SampleHeight(TerrainObj.transform.position + new Vector3(i,0,j)))
				{
					maxHeight = TerrainObj.SampleHeight(TerrainObj.transform.position + new Vector3(i,0,j));
				}
			}
		}
		return maxHeight;
	}

	// pathfinding using Astar algorithm(arraylist<PathNode>)
	public PathNode[] AStarPath(Vector3 Source, Vector3 Destination)
	{
		// Determine the nodes
		PathNode start = getClosestNode (Source);
		PathNode goal = getClosestNode (Destination);

		if (start == null || goal == null)
						return null;

		// Reset Graph g\fScore
		ResetGraphScores(start.Position, goal.Position);

		ArrayList closedSet = new ArrayList ();
		ArrayList openSet = new ArrayList ();
		openSet.Add (start);
		start.GScore = 0;
		start.FScore = heuristic_cost_estimate (start.Position, goal.Position);

		while (openSet.Count > 0) 
		{
			PathNode current = getLowestFScoreInSet(openSet);
			if(current == goal)
			{
				return ConstructPathFromGoal(goal);
			}

			openSet.Remove(current);
			closedSet.Add(current);
			foreach(PathNode neighbour in current.Neightbours)
			{
				if(!closedSet.Contains(neighbour))
				{
					float tentativeGScore = current.GScore + (neighbour.Position - current.Position).magnitude;
					if(!openSet.Contains(neighbour) || tentativeGScore < neighbour.GScore)
					{
						neighbour.CameFrom = current;
						neighbour.GScore = tentativeGScore;
						neighbour.FScore = neighbour.GScore + heuristic_cost_estimate(neighbour.Position,goal.Position);
						if(!openSet.Contains(neighbour))
						{
							openSet.Add(neighbour);
						}
					}
				}
			}
		}
		return null;
	}

	private PathNode[] ConstructPathFromGoal(PathNode goal)
	{
		Stack<PathNode> path = new Stack<PathNode>();
		PathNode current = goal;
		while (current != null)
		{
			path.Push(current);
			current = current.CameFrom;
		}
		// TODO: make sure the order is correct
		return path.ToArray();
	}

	private PathNode getLowestFScoreInSet(ArrayList set)
	{
		float minimalFscore = float.MaxValue;
		PathNode lowestNode = null;
		foreach (PathNode node in set)
		{
			if(node != null)
			{
				if(node.FScore < minimalFscore)
				{
					minimalFscore = node.FScore;
					lowestNode = node;
				}
			}
		}

		return lowestNode;
	}

	/** Sets the fscore and gscore for each node for A* alogirthm **/
	private  void ResetGraphScores(Vector3 Start, Vector3 End)
	{
		foreach (PathNode node in GraphOfNodes)
		{
			if(node != null)
			{
				node.Reset();
				node.GScore = heuristic_cost_estimate(node.Position,Start);
				node.FScore = heuristic_cost_estimate(node.Position,End);
			}
		}
	}

	private float heuristic_cost_estimate(Vector3 Position, Vector3 Goal)
	{
		return (Goal - Position).magnitude;
	}

	private PathNode getClosestNode(Vector3 Position)
	{
		float MinDistance = float.MaxValue;
		PathNode chosenPoint = null;
		foreach (PathNode node in GraphOfNodes)
		{
			if(node != null)
			{
				if(Mathf.Abs((Position - node.Position).magnitude) < MinDistance)
				{
					MinDistance = Mathf.Abs((Position - node.Position).magnitude);
					chosenPoint = node;
				}
			}
		}

		return chosenPoint;
	}
}
