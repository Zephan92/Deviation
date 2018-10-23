using Assets.Deviation.MasterServer.Scripts.Market;
using Assets.Deviation.MasterServer.Scripts.MatchMaking;
using Assets.Deviation.MasterServer.Scripts.Notification;
using Barebones.MasterServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Deviation.MasterServer.Scripts
{
	public class DeviationServer : MonoBehaviour
	{
		public NotificationModule Notification;
		public MaterialBankModule MaterialBank;
		public Exchange1v1MatchMakingModule MatchMaking;
		public Exchange1v1Module Exchange;
		public MarketModule Market;

		public void Awake()
		{
			Notification = FindObjectOfType<NotificationModule>();
			MaterialBank = FindObjectOfType<MaterialBankModule>();
			MatchMaking = FindObjectOfType<Exchange1v1MatchMakingModule>();
			Exchange = FindObjectOfType<Exchange1v1Module>();
			Market = FindObjectOfType<MarketModule>();
		}
	}
}
