using UnityEngine;
using System.Collections;

public class GatherPathFindingNodes : MonoBehaviour
{
	private const float NODE_SIZE = 2.5f; // How much the each node covers in pixel
	private bool[,] GraphOfTerrain;
	public Terrain TerrainObj;
	public float MaxSlope;

	// Use this for initialization
	void Start ()
	{
		// Initiate the terrian to be the size of the terrain / NODE_SIZE
		GraphOfTerrain = new bool[(int)(TerrainObj.terrainData.size.x / NODE_SIZE),(int)(TerrainObj.terrainData.size.z / NODE_SIZE)];

		// Get Terrain start position
		Vector3 terrPosition = TerrainObj.transform.position;

		// Refernece to the data of a hit
		RaycastHit data;

		// Save the RayCaster Source Position
		Vector3 SourcePosition = terrPosition;

		// Set the raycaster to be above all the mountains on the terain
		SourcePosition.y += maxHeightOfTerrain() + 5;

		// Run on all the terrain
		for (int nRow = 0; nRow < GraphOfTerrain.GetLength(0); nRow++)
		{
			for (int nCol = 0; nCol < GraphOfTerrain.GetLength(1); nCol++)
			{
				// Shoot a raycast down on the terrain
				Physics.Raycast(SourcePosition,Vector3.down,out data,SourcePosition.y + 10);

				float angle = Mathf.Abs(Vector3.Angle(Vector3.up,data.normal));

				// If the normal of the hit was smaller than the slope, it means AI can walk here, set to true
				GraphOfTerrain[nRow,nCol] = angle < MaxSlope;

				// Advance in the z axis (columns)
				SourcePosition.z += NODE_SIZE;

				print("At " + nRow * NODE_SIZE + ":" + nCol * NODE_SIZE +  " value : " + GraphOfTerrain[nRow,nCol]);
			}

			// Start A new row on the terrain
			SourcePosition.z = terrPosition.z;
			SourcePosition.x += NODE_SIZE;
		}

	}

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

}
