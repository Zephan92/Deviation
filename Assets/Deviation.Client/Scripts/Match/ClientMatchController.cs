using Assets.Deviation.Client.Scripts.Match;
using Assets.Deviation.Client.Scripts.UserInterface;
using Assets.Scripts.Interface;
using Assets.Scripts.Utilities;
using Barebones.MasterServer;
using Barebones.Networking;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Deviation.Exchange.Scripts.Client
{
	public enum ClientMatchState
	{
		Start = 0,
		ChooseCharacter = 1,
		ChooseActions = 2,
		End = 3
	}

	public class ClientMatchController : MonoBehaviour
	{
		public ClientMatchState State;

		public Text TimerText;
		public Text TitleText;
		public Button ReadyButton;
		private TraderDisplayController tpc;
		private ClientDataController cdc;
		private ITimerManager tm;
		 
		public void Start()
		{
			tm = FindObjectOfType<TimerManager>();
			cdc = FindObjectOfType<ClientDataController>();
			tpc = GetComponent<TraderDisplayController>();

			State = ClientMatchState.Start;

			if (cdc != null)
			{
				cdc.State = ClientState.Match;

				if (cdc.PlayerAccount != null)
				{
					ReadyButton.interactable = true;
				}
				else
				{
					cdc.PlayerAccountRecieved += () =>
					{
						ReadyButton.interactable = true;
					};
				}	
			}

			State = ClientMatchState.ChooseCharacter;
			tm.AddTimer("ChooseCharacter", 30);
			tm.AddTimer("ChooseActions", 60);
			tm.RestartTimer("ChooseCharacter");
			TitleText.text = "Choose Your Character!";
			if (UnityEngine.Debug.isDebugBuild && !Application.isEditor)
			{
				var testArgs = Msf.Args.ExtractValue("-test");
				if (testArgs.Equals("GuestLogin"))
				{
					StartCoroutine(Test());
				}
			}
		}

		private IEnumerator Test()
		{
			yield return new WaitForSeconds(1f);
			if (UnityEngine.Debug.isDebugBuild)
				Ready();
		}

		public void FixedUpdate()
		{
			switch (State)
			{
				case ClientMatchState.Start:
					break;
				case ClientMatchState.ChooseCharacter:
					tm.UpdateCountdowns();
					TimerText.text = ((int) tm.GetRemainingCooldown("ChooseCharacter")).ToString();
					if (tm.TimerUp("ChooseCharacter"))
					{
						State = ClientMatchState.ChooseActions;
						tm.RestartTimer("ChooseActions");
						TitleText.text = "Choose Your Actions!";
					}
					break;
				case ClientMatchState.ChooseActions:
					tm.UpdateCountdowns();
					TimerText.text = ((int) tm.GetRemainingCooldown("ChooseActions")).ToString();
					if (tm.TimerUp("ChooseActions"))
					{
						State = ClientMatchState.End;
					}
					break;
				case ClientMatchState.End:
					break;

			}
		}

		public void ConfirmTrader()
		{
			State = ClientMatchState.ChooseActions;
			tm.RestartTimer("ChooseActions");
			TitleText.text = "Choose Your Actions!";
			tpc.OnConfirmTrader();
		}

		public void Ready()
		{
			if (cdc.State == ClientState.Match && cdc.Exchange != null)
			{
				Guid characterGuid = GetPlayerCharacter();
				ActionModulePacket module = GetPlayerActionModule();
				InitExchangePlayerPacket packet = new InitExchangePlayerPacket(cdc.Exchange.ExchangeId, cdc.PlayerAccount, characterGuid, module);

				Msf.Client.Connection.SendMessage((short)ExchangePlayerOpCodes.CreateExchangeData, packet, (response, error) => {
					if (response == ResponseStatus.Error)
					{
						UnityEngine.Debug.LogErrorFormat("CreateExchangeData Error: {0}", error);
					}
					else if(response == ResponseStatus.Success)
					{
						ReadyButton.interactable = false;
						StartCoroutine(StartExchange());
					}
				});
			}
		}

		private IEnumerator StartExchange()
		{
			yield return new WaitUntil(() => cdc.RoomId != -1);
			SceneManager.LoadScene("DeviationStandalone");
		}

		private Guid GetPlayerCharacter()
		{
			return Guid.NewGuid();
		}

		private ActionModulePacket GetPlayerActionModule()
		{
			var q = new Guid("688b267a-fde1-4250-91a0-300aa3343147");//ShockWave
			var w = new Guid("258175b1-89e0-4f16-91e2-b65cb1e11c58");//Ambush
			//var w = new Guid("dacb468b-658f-4daa-9400-cd3f005d06bd");//Stun Field
			//var e = new Guid("d504df35-dc93-4f84-829e-01e202878341");//Tremor

			var e = new Guid("1e14d696-7a90-4271-97e2-fbc8a8c740f8");//Wall Push
			var r = new Guid("36a1cf13-8b79-4800-8574-7cec0c405594");//Small Projectile
			//this would go get the actions the player chose
			return new ActionModulePacket(q, w, e, r);
		}
	}
}
