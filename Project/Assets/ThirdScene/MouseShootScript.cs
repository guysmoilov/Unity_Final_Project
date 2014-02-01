using UnityEngine;
using System.Collections;

public class MouseShootScript : MonoBehaviour 
{
	public Material redMaterial;
	public Material blueMaterial;
	public Material greenMaterial;

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Input.GetMouseButtonDown(0))
		{
			RaycastHit hit;

			Debug.DrawRay(Camera.main.ScreenPointToRay(Input.mousePosition).origin,
			              Camera.main.ScreenPointToRay(Input.mousePosition).direction * 100,
			              Color.red);

			if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
			{
				if (hit.transform.parent.tag == "LeaderBird")
				{
					Debug.Log("WIN " + hit.transform.parent.name);

					var renderers = hit.transform.parent.GetComponentsInChildren<MeshRenderer>();

					foreach (var renderer in renderers)
					{
						renderer.material = greenMaterial;
					}
				}
				else
				{
					Debug.Log(hit.transform.parent.name);

					var renderers = hit.transform.parent.GetComponentsInChildren<MeshRenderer>();
					
					foreach (var renderer in renderers)
					{
						renderer.material = redMaterial;
					}

					renderers = GameObject.FindGameObjectWithTag("LeaderBird").GetComponentsInChildren<MeshRenderer>();

					foreach (var renderer in renderers)
					{
						renderer.material = blueMaterial;
					}
				}
			}
		}
	}
}
