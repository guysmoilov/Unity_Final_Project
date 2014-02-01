using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathNode
{
		private Vector3 _position;
		private float f_score;
		private float g_score;
		private List<PathNode> neighbours;
	 	private PathNode cameFrom;

		public PathNode (Vector3 Position)
		{
			this._position = Position;
			this.neighbours = new List<PathNode> ();
			Reset ();
		}
		public void Reset()
		{
			f_score = 0;
			g_score = 0;
			cameFrom = null;
		}	
		public float FScore
		{
			get{ return f_score;}
			set{f_score = value;}
		}
	
		public Vector3 Position
		{
		get{return this._position;}
		}
		public float GScore
		{
			get{ return g_score;}
			set{g_score = value;}
		}

		public PathNode CameFrom
		{
			get{ return cameFrom;}
			set{cameFrom = value;}
		}

	public List<PathNode> Neightbours
		{
			get{ return neighbours;}
			set{neighbours = value;}
		}
}

