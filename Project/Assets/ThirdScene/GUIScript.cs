using UnityEngine;
using System.Collections;

public class GUIScript : MonoBehaviour 
{
	public int countdown = 5;
	public Rect countDownDisplay = new Rect(Screen.width / 2 - 30, Screen.height / 2 - 10, 60, 20);
	public GUIStyle countdownStyle;

	public GUIStyle messageStyle;

	bool countdownStarted = false;

	void Start()
	{
		StartCoroutine(ChangeFont());
	}

	void OnGUI()
	{
		GUI.Label(new Rect(0, 0, 100, 20), "Shoot the leader bird!", messageStyle);

		if (!countdownStarted)
		{
			if (GUI.Button(new Rect(Screen.width / 2 - 100, Screen.height - 30, 200, 30), "Click here to start countdown"))
			{
				countdownStarted = true;

				// TODO: Make birds wander
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
				GUI.Label(countDownDisplay, "SHOOT!");
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
			Debug.Log(countdown);
			for (;countdownStyle.fontSize > 0; countdownStyle.fontSize--)
			{
				yield return new WaitForFixedUpdate();
			}

			countdownStyle.fontSize = originalFontSize;
		}
	}
}
