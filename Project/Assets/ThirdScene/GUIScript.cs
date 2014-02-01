using UnityEngine;
using System.Collections;

public class GUIScript : MonoBehaviour 
{
	public int countdown = 5;
	Rect countDownDisplay;
	public GUIStyle countdownStyle;

	public GUIStyle messageStyle;

	bool countdownStarted = false;
	bool startedShooting = false;

	void Start()
	{
		countDownDisplay = new Rect(Screen.width / 2 - 30, Screen.height / 2 - 10, 60, 20);
		StartCoroutine(ChangeFont());
		StartCoroutine(StartShooting());
	}

	void OnGUI()
	{
		GUI.Label(new Rect(0, 0, 100, 20), "Shoot the leader bird!", messageStyle);

		if (!countdownStarted)
		{
			if (GUI.Button(new Rect(Screen.width / 2 - 100, Screen.height - 30, 200, 30), "Click here to start countdown"))
			{
				countdownStarted = true;

				var birds = GameObject.FindGameObjectsWithTag("Bird");

				foreach (var bird in birds)
				{
					bird.GetComponent<StackFSM>().PushState(bird.GetComponent<BirdController>().WanderingState);
				}

				var leader = GameObject.FindGameObjectWithTag("LeaderBird");
				leader.GetComponent<StackFSM>().PushState(leader.GetComponent<BirdController>().WanderingState);
			}
		}
		else
		{
			if (countdown > 0)
			{
				GUI.Label(countDownDisplay, countdown.ToString(), countdownStyle);
			}
			else
			{
				GUI.Label(countDownDisplay, "SHOOT!", messageStyle);

				// Enable shooting
				startedShooting = true;
			}
		}
	}

	IEnumerator ChangeFont()
	{
		var originalFontSize = countdownStyle.fontSize;

		while (!countdownStarted)
			yield return new WaitForFixedUpdate();

		for (; countdown > 0; countdown--)
		{
			for (;countdownStyle.fontSize > 0; countdownStyle.fontSize--)
			{
				yield return new WaitForFixedUpdate();
			}

			countdownStyle.fontSize = originalFontSize;
		}
	}

	IEnumerator StartShooting()
	{
		while (!startedShooting)
			yield return new WaitForFixedUpdate();

		this.GetComponent<MouseShootScript>().enabled = true;
	}
}
