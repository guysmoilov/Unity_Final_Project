using UnityEngine;
using System.Collections;

public class WifeController : MonoBehaviour 
{
	StackFSM brain;
	TextMesh textMesh;

	public float changeStateChance = 0.005f;

	// Use this for initialization
	void Start () 
	{
		brain = GetComponent<StackFSM>();
		textMesh = GetComponentInChildren<TextMesh>();

		brain.PushState(CleanState);
	}

	void CleanState()
	{
		textMesh.text = "Wife: Cleaning the house";

		if (Random.value < changeStateChance)
		{
			brain.PopState();

			if (Random.value >= 0.5f)
			{
				brain.PushState(CookState);
			}
			else
			{
				brain.PushState(LaundryState);
			}
		}
	}

	void CookState()
	{
		textMesh.text = "Wife: Cooking dinner";
		
		if (Random.value < changeStateChance)
		{
			brain.PopState();
			
			if (Random.value >= 0.5f)
			{
				brain.PushState(CleanState);
			}
			else
			{
				brain.PushState(LaundryState);
			}
		}
	}

	void LaundryState()
	{
		textMesh.text = "Wife: Doing the laundry";
		
		if (Random.value < changeStateChance)
		{
			brain.PopState();
			
			if (Random.value >= 0.5f)
			{
				brain.PushState(CleanState);
			}
			else
			{
				brain.PushState(CookState);
			}
		}
	}
}
