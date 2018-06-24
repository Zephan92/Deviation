using Assets.Deviation.Client.Scripts;
using Assets.Deviation.Client.Test;
using Assets.Deviation.Exchange.Scripts.Client;
using Assets.Scripts.Interface.DTO;
using Assets.Scripts.Library;
using Barebones.MasterServer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestClientMatchController : TestBase
{
	public ClientMatchController cmc;
	public Button UsePresetOptionsButton;
	public Transform TestUI;

	public override void Awake()
	{
		base.Awake();

		cmc = FindObjectOfType<ClientMatchController>();
		TestUI = TestingSuite.transform.Find("TestUI");
		UsePresetOptionsButton = TestUI.Find("UsePresetOptions").GetComponent<Button>();
		UsePresetOptionsButton.onClick.AddListener(InitiatePresetOptions);
	}

	public override void Start ()
	{
		base.Start();

		if (Debug.isDebugBuild && !Application.isEditor)
		{
			var testArgs = Msf.Args.ExtractValue("-test");
			if (testArgs != null && testArgs.Equals("GuestLogin"))
			{
				Debug.LogError("Test: ClientMatchController");

				StartCoroutine(PresetOptions(1f));
			}
		}
	}

	void Update () {
		
	}

	public void InitiatePresetOptions()
	{
		StartCoroutine(PresetOptions(0f));
	}

	private IEnumerator PresetOptions(float wait)
	{
		Debug.Log("Test - Using Preset Options");

		ITrader chosenTrader;
		List<IExchangeAction> actions;
		chosenTrader = new Trader("TestTrader", "The ultimate avenger", Assets.Scripts.Enum.TraderType.Test, "Testing Description", System.Guid.NewGuid());
		actions = new List<IExchangeAction>();
		actions.Add(ActionLibrary.GetActionInstance("Drain"));
		actions.Add(ActionLibrary.GetActionInstance("ShockWave"));
		actions.Add(ActionLibrary.GetActionInstance("Wall Push"));
		actions.Add(ActionLibrary.GetActionInstance("StunField"));

		cmc.ConfirmTrader(chosenTrader);
		cmc.ConfirmActions(actions);

		yield return new WaitForSeconds(wait);

		if (ClientDataRepository.Instance.HasExchange)
		{
			cmc.Ready();
		}
		else
		{
			Debug.LogError("No Exchange has been found");
		}
	}
}
