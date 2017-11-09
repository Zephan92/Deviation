using Assets.Scripts.Controllers;
using Assets.Scripts.Interface;
using Assets.Scripts.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Exchange.Display
{
	public class ChangePhaseControls : MonoBehaviour
	{
		public Vector2 ExchangeTimerPosition;
		public Vector2 ExchangeTimerSize;
		public Texture2D emptyTex;

		private IExchangeTimer exchangeTimer;
		private Canvas exchangeCanvas;

		private ITimerManager tm;
		//private IMultiplayerController mc;
		private IChangePhaseController cp;
		public void Awake()
		{
			cp = FindObjectOfType<ChangePhaseController>();
			tm = FindObjectOfType<TimerManager>();
			//mc = FindObjectOfType<MultiplayerController>();

			exchangeTimer = new ExchangeTimer();
			exchangeCanvas = GetComponent<Canvas>();
			RectTransform canvasRect = exchangeCanvas.transform as RectTransform;
			Vector3[] corners = new Vector3[4];
			canvasRect.GetWorldCorners(corners);

			emptyTex = Resources.Load("User Interface/White") as Texture2D;
			ExchangeTimerSize = new Vector2(corners[1].y * 0.1f, corners[1].y * 0.1f);
			ExchangeTimerPosition = new Vector2(corners[2].x - ExchangeTimerSize.x, 0);
		}

		public void OnGUI()
		{
			if (cp.PhaseStarted)
			{
				var exchangeTimerDetails = new ExchangeTimerDetails(emptyTex, ExchangeTimerPosition, ExchangeTimerSize);
				exchangeTimer.DrawExchangeTimer(exchangeTimerDetails, ((int)tm.GetRemainingCooldown("ChangePhase")).ToString());
			}
		}
	}
}
