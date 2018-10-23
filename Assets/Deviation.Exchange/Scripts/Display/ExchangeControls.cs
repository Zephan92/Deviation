﻿using Assets.Deviation.Exchange.Scripts.Client;
using Assets.Scripts.Controllers;
using Assets.Scripts.Interface;
using Assets.Scripts.Utilities;
using Barebones.MasterServer;
using System;
using UnityEngine;

namespace Assets.Scripts.Exchange.Display
{
	public class ExchangeControls : MonoBehaviour
	{
		public Vector2 HealthPosition;
		public Vector2 BarSize;
		public Vector2 ActionBarSize;
		public Vector2 ExchangeTimerSize;

		public Vector2 ActionBarPosition;
		public Vector2 ExchangeTimerPosition;
		public Texture2D outlineTex;
		public Texture2D emptyTex;
		public Texture2D fullHealthTex;
		public Texture2D outerActionBarTex;
		public Texture2D cooldownTex;
		public Material fontMaterial;

		public bool toggle;

		private IActionBar actionBar;
		private IProgressBar progressBar;
		private ActionBarDetails player1actionBar;
		private Texture2D [] actionTextures;
		private string [] actionNames;
		private IExchangeTimer exchangeTimer;
		private ExchangeTimerDetails exchangeTimerDetails;
		private ProgressBarDetails playerhealthBar;

		private Canvas exchangeCanvas;

		private IExchangePlayer [] _players;
		private IExchangePlayer _currentPlayer;
		private ITimerManager tm;
		private IExchangeController1v1 ec;
		private ClientDataRepository cdc;

		public void Awake()
		{
			ec = FindObjectOfType<ExchangeController1v1>();
			cdc = FindObjectOfType<ClientDataRepository>();
			outlineTex = Resources.Load("Color/Black") as Texture2D;
			emptyTex = Resources.Load("Color/White") as Texture2D;
			fullHealthTex = Resources.Load("Color/Green") as Texture2D;
			outerActionBarTex = Resources.Load("Color/Green") as Texture2D;
			cooldownTex = Resources.Load("AbilityIcons/CooldownTexture")as Texture2D;
			progressBar = new ProgressBar();
			actionBar = new ActionBar();
			exchangeTimer = new ExchangeTimer();
			playerhealthBar = new ProgressBarDetails(outlineTex, emptyTex, fullHealthTex, HealthPosition, BarSize);
			exchangeTimerDetails = new ExchangeTimerDetails(outlineTex, ExchangeTimerPosition, ExchangeTimerSize);
		}

		public void Start()
		{
			tm = FindObjectOfType<TimerManager>();
			exchangeCanvas = GetComponent<Canvas>();
			RectTransform canvasRect = exchangeCanvas.transform as RectTransform;
			Vector3[] corners = new Vector3[4];
			canvasRect.GetWorldCorners(corners);
			BarSize = new Vector2(corners[2].x * 0.08f, corners[1].y * 0.025f);
			ActionBarSize = new Vector2(corners[2].x * 0.4f, corners[1].y * 0.125f);
			ExchangeTimerSize = new Vector2(corners[1].y * 0.1f, corners[1].y * 0.1f);
			HealthPosition = new Vector2(BarSize.x / 2, corners[1].y * 0.1f);
			ActionBarPosition = new Vector2(0, corners[2].y - ActionBarSize.y);
			ExchangeTimerPosition = new Vector2(corners[2].x - ExchangeTimerSize.x, 0);
		}

		public void FixedUpdate()
		{
			if (ec == null)
			{
				ec = FindObjectOfType<ExchangeController1v1>();
				return;
			}

			if (tm == null)
			{
				tm = FindObjectOfType<TimerManager>();
				return;
			}

			if (_players == null && cdc != null)
			{
				var playerObjects = FindObjectsOfType<ExchangePlayer>();

				if (playerObjects.Length == ExchangeConstants.PLAYER_COUNT)
				{
					foreach (var player in playerObjects)
					{
						long id = cdc.PlayerAccount.Id;
						if (player.PlayerId == id)
						{
							_currentPlayer = player;
						}
					}

					if (_currentPlayer != null)
					{
						_players = playerObjects;
					}
				}
			}
		}

		public void OnGUI()
		{
			if (ec == null | tm == null | _players == null | _currentPlayer == null)
			{
				return;
			}

			switch (ec.ExchangeState)
			{
				case Enum.ExchangeState.Begin:
					BattleGUI();
					break;
				case Enum.ExchangeState.Battle:
					BattleGUI();
					break;
			}
		}

		private void BattleGUI()
		{
			playerhealthBar = new ProgressBarDetails(outlineTex, emptyTex, fullHealthTex, HealthPosition, BarSize);
			//actionTextures = new Texture2D[4]
			//{
			//	_currentPlayer.Kit.Actions[0].ActionTexture,
			//	_currentPlayer.Kit.Actions[1].ActionTexture,
			//	_currentPlayer.Kit.Actions[2].ActionTexture,
			//	_currentPlayer.Kit.Actions[3].ActionTexture
			//};
			//actionNames = new string[]
			//{
			//	_currentPlayer.Kit.Actions[0].Name,
			//	_currentPlayer.Kit.Actions[1].Name,
			//	_currentPlayer.Kit.Actions[2].Name,
			//	_currentPlayer.Kit.Actions[3].Name
			//};

			//player1actionBar = new ActionBarDetails(outerActionBarTex, actionTextures, cooldownTex, ActionBarPosition, ActionBarSize, actionNames, tm);
			exchangeTimerDetails = new ExchangeTimerDetails(emptyTex, ExchangeTimerPosition, ExchangeTimerSize);

			foreach (IExchangePlayer player in _players)
			{
				try
				{
					progressBar.DrawProgressBar(player.Position, playerhealthBar, player.Health.CurrentPercentage, player.Health.Current.ToString() + "/" + player.Health.Max.ToString());
				}
				catch (Exception)
				{
					//we don't care if this fails
				}
				
			}

			
			//actionBar.DrawActionBar(player1actionBar);
			
			
			exchangeTimer.DrawExchangeTimer(exchangeTimerDetails, ((int)tm.GetRemainingCooldown("ExchangeTimer")).ToString());
			GUI.Label(new Rect(Vector2.one, exchangeTimerDetails.Size - new Vector2(1, 1)), ec.ExchangeState.ToString(), new GUIStyle());
		}
	}
}
