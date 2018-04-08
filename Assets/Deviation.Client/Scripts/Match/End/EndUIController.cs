using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Deviation.Client.Scripts.Match
{
	public class EndUIController : UIController
	{
		public Button ReadyButton;

		public override void Awake()
		{
			base.Awake();

			ReadyButton = GetComponentInChildren<Button>();
			ReadyButton.onClick.AddListener(cmc.Ready);
		}
	}
}
