using UnityEngine;
using System.Collections;
using System.Diagnostics;
using System;

public class ClientController : MonoBehaviour
{
	public void OpenStandalone()
	{
		UnityEngine.Debug.Log("Opening Standalone");
		var commandLineArgs = "-show-screen-selector false -screen-height 900 -screen-width 1600";
		var exePath = Environment.GetEnvironmentVariable("DeviationStandalone", EnvironmentVariableTarget.User) + "/DeviationStandalone.exe";
		Process.Start(exePath, commandLineArgs);
	}
}
