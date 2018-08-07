using Assets.Deviation.Client.Scripts;
using Assets.Deviation.Client.Scripts.Client;
using Assets.Deviation.Client.Test;
using Assets.Deviation.Exchange.Scripts.Client;
using Assets.Deviation.Materials;
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
	public bool hasSearchListener;

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
				UnityEngine.Debug.Log("Test: ClientController");

				StartCoroutine(SearchForExchange());
			}
		}

		if (Application.isEditor)
		{
			cc.PlayButton.onClick.AddListener(AddOnSearchListener);
			cc.CraftButton.onClick.AddListener(AddOnCraftInit);
		}
	}

	private IEnumerator SearchForExchange()
	{
		yield return new WaitForSeconds(1f);
		cc.SwitchTab(ClientTab.Play);
		yield return new WaitForSeconds(1f);
		var mc = FindObjectOfType<MatchmakingController>();
		mc.SearchForExchangeMatch();
	}

	private void AddOnSearchListener()
	{
		if (hasSearchListener)
		{
			return;
		}

		var mc = FindObjectOfType<MatchmakingController>();
		mc.JoinQueueButton.onClick.AddListener(StartClient);
		hasSearchListener = true;
	}

	private void AddOnCraftInit()
	{
		var cc = FindObjectOfType<CraftingController>();
		var bag = new MaterialBag();
		foreach (var material in MaterialLibrary.GetMaterials())
		{
			bag.AddMaterial(material, 9999);
		}
		cc.Bag = bag;
	}

	private void StartClient()
	{
		if (hasOpponent)
		{
			return;
		}

		var commandLineArgs = " -test GuestLogin";
		var exePath = "C:/Users/zepha/Desktop/Programming/Projects/Deviation/Builds/DeviationServers/DeviationClient.exe";
		UnityEngine.Debug.Log(exePath + commandLineArgs);

		Process.Start(exePath, commandLineArgs);
		hasOpponent = true;
	}
}
