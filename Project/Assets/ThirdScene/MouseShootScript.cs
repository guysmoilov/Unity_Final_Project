using UnityEngine;
using System.Collections;

public class MouseShootScript : MonoBehaviour 
{
	public Material redMaterial;
	public Material blueMaterial;
	public Material greenMaterial;
	
	// Update is called once per frame
	void Update () 
	{
		if (Input.GetMouseButtonDown(0))
		{
			RaycastHit hit;

			if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
			{
				if (hit.transform.parent.tag == "LeaderBird")
				{
					var renderers = hit.transform.parent.GetComponentsInChildren<MeshRenderer>();

					foreach (var renderer in renderers)
					{
						renderer.material = greenMaterial;
					}
				}
				else
				{
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

				// Return to leadership behaviour
				var birds = GameObject.FindGameObjectsWithTag("Bird");

				foreach (var bird in birds)
				{
					bird.GetComponent<StackFSM>().PopState();
				}

				GameObject.FindGameObjectWithTag("LeaderBird").GetComponent<StackFSM>().PopState();

				this.enabled = false;
			}
		}
	}
}
