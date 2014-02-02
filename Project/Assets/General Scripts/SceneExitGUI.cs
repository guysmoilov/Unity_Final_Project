using UnityEngine;
using System.Collections;

public class SceneExitGUI : MonoBehaviour 
{
	bool paused = false;

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (paused)
			{
				paused = false;
				Time.timeScale = 1;
			}
			else
			{
				paused = true;
				Time.timeScale = 0;
			}
		}
	}

	void OnGUI() 
	{
		if (paused)
		{
			GUI.Label(new Rect(Screen.width / 2 - 60, Screen.height / 2 - 20, 120, 20), "Skip to next scene?");
			
			if (GUI.Button(new Rect(Screen.width / 2 - 60, Screen.height / 2 + 30, 40, 30), "Yes"))
			{
				Time.timeScale = 1;
				paused = false;
				
				// Loop through levels
				Application.LoadLevel((Application.loadedLevel + 1) % Application.levelCount);
			}
			
			if (GUI.Button(new Rect(Screen.width / 2 - 20, Screen.height / 2 + 30, 40, 30), "No"))
			{
				paused = false;
				Time.timeScale = 1;
			}
			
			if (GUI.Button(new Rect(Screen.width / 2 + 20, Screen.height / 2 + 30, 40, 30), "Quit"))
			{
				Application.Quit();
			}
		}
	}
}
