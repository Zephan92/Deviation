using Assets.Deviation.Exchange.Scripts.Client;
using Barebones.MasterServer;
using UnityEngine;
using System.Collections;

namespace Assets.Deviation.Client.Test
{
	public class TestClientLoginController : TestBase
	{
		public ClientLoginController clc;

		public override void Awake()
		{
			base.Awake();

			clc = FindObjectOfType<ClientLoginController>();
		}

		public override void Start()
		{
			base.Start();

			if (UnityEngine.Debug.isDebugBuild)
			{
				var testArgs = Msf.Args.ExtractValue("-test");
				if (testArgs != null && testArgs.Equals("GuestLogin"))
				{
					UnityEngine.Debug.LogError("Test: ClientController");

					StartCoroutine(LoginAsGuest());
				}
			}
		}

		private IEnumerator LoginAsGuest()
		{
			yield return new WaitForSeconds(1f);
			ClientDataRepository.Instance.LoginAsGuest();
		}
	}
}
