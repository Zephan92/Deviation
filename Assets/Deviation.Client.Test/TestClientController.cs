using Assets.Deviation.Client.Scripts;
using Assets.Deviation.Client.Test;
using Assets.Deviation.Exchange.Scripts.Client;
using Assets.Scripts.Interface.DTO;
using Assets.Scripts.Library;
using Barebones.MasterServer;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class TestClientController : TestBase
{
	public ClientController cc;
	public bool hasOpponent;

	public  override void Awake()
	{
		base.Awake();

		cc = FindObjectOfType<ClientController>();
	}

	public override void Start ()
	{
		base.Start();

		if (UnityEngine.Debug.isDebugBuild)
		{
			var testArgs = Msf.Args.ExtractValue("-test");
			if (testArgs != null && testArgs.Equals("GuestLogin"))
			{
				UnityEngine.Debug.LogError("Test: ClientController");

				StartCoroutine(SearchForExchange());
			}
		}

		if (Application.isEditor)
		{
			cc.JoinQueueButton.onClick.AddListener(StartClient);
		}
	}

	private IEnumerator SearchForExchange()
	{
		yield return new WaitForSeconds(1f);
		cc.SearchForExchangeMatch();
	}

	private void StartClient()
	{
		var commandLineArgs = " -test GuestLogin";
		var exePath = "C:/Users/zepha/Desktop/Programming/Projects/Deviation/Builds/DeviationServers/DeviationClient.exe";
		UnityEngine.Debug.Log(exePath + commandLineArgs);

		Process.Start(exePath, commandLineArgs);
		hasOpponent = true;
	}
}
