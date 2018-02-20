using Assets.Deviation.Exchange.Scripts.Client;
using Assets.Scripts.Controllers;
using Assets.Scripts.Interface;
using Assets.Scripts.Utilities;
using Barebones.MasterServer;
using UnityEngine;

namespace Assets.Scripts.Exchange.Display
{
	public class ExchangeControls : MonoBehaviour
	{
		public Vector2 HealthPosition;
		public Vector2 EnergyPosition;
		public Vector2 HealthPosition1;
		public Vector2 EnergyPosition1;
		public Vector2 BarSize;
		public Vector2 ActionBarSize;
		public Vector2 ExchangeTimerSize;

		public Vector2 ActionBarPosition;
		public Vector2 ExchangeTimerPosition;
		public Texture2D outlineTex;
		public Texture2D emptyTex;
		public Texture2D fullEnergyTex;
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
		private ProgressBarDetails player1energyBar;
		private ProgressBarDetails player1healthBar;

		//private ProgressBarDetails player2energyBar;
		//private ProgressBarDetails player2healthBar;

		private Canvas exchangeCanvas;

		private IExchangePlayer [] _players;
		private IExchangePlayer _currentPlayer;
		private ITimerManager tm;
		private IExchangeController1v1 _ec;
		private ClientDataController cdc;

		public void Awake()
		{
			_ec = FindObjectOfType<ExchangeController1v1>();
			cdc = FindObjectOfType<ClientDataController>();
			outlineTex = Resources.Load("User Interface/Black") as Texture2D;
			emptyTex = Resources.Load("User Interface/White") as Texture2D;
			fullEnergyTex = Resources.Load("User Interface/Purple") as Texture2D;
			fullHealthTex = Resources.Load("User Interface/Green") as Texture2D;
			outerActionBarTex = Resources.Load("User Interface/Green") as Texture2D;
			cooldownTex = Resources.Load("AbilityIcons/CooldownTexture")as Texture2D;
			progressBar = new ProgressBar();
			actionBar = new ActionBar();
			exchangeTimer = new ExchangeTimer();
			player1energyBar = new ProgressBarDetails(outlineTex, emptyTex, fullEnergyTex, HealthPosition, BarSize);
			player1healthBar = new ProgressBarDetails(outlineTex, emptyTex, fullHealthTex, EnergyPosition, BarSize);
			//player2energyBar = new ProgressBarDetails(outlineTex, emptyTex, fullEnergyTex, HealthPosition1, BarSize);
			//player2healthBar = new ProgressBarDetails(outlineTex, emptyTex, fullHealthTex, EnergyPosition1, BarSize);
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
			EnergyPosition = new Vector2(BarSize.x / 2, corners[1].y * 0.1f + BarSize.y);
			HealthPosition = new Vector2(BarSize.x / 2, corners[1].y * 0.1f);
			EnergyPosition1 = new Vector2(BarSize.x / 2, corners[1].y * 0.1f + BarSize.y);
			HealthPosition1 = new Vector2(BarSize.x / 2, corners[1].y * 0.1f);
			ActionBarPosition = new Vector2(0, corners[2].y - ActionBarSize.y);
			ExchangeTimerPosition = new Vector2(corners[2].x - ExchangeTimerSize.x, 0);
		}

		public void FixedUpdate()
		{
			if (_ec == null)
			{
				_ec = FindObjectOfType<ExchangeController1v1>();
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
			if (_ec == null | tm == null | _players == null | _currentPlayer == null)
			{
				return;
			}

			switch (_ec.ExchangeState)
			{
				case Enum.ExchangeState.Setup:
					break;
				case Enum.ExchangeState.PreBattle:
					break;
				case Enum.ExchangeState.Start:
					BattleGUI();
					break;
				case Enum.ExchangeState.Battle:
					BattleGUI();
					break;
				case Enum.ExchangeState.End:
					BattleGUI();
					break;
				case Enum.ExchangeState.PostBattle:
					break;
				case Enum.ExchangeState.Teardown:
					break;
			}
		}

		private void BattleGUI()
		{
			player1energyBar = new ProgressBarDetails(outlineTex, emptyTex, fullEnergyTex, HealthPosition, BarSize);
			player1healthBar = new ProgressBarDetails(outlineTex, emptyTex, fullHealthTex, EnergyPosition, BarSize);
			//player2energyBar = new ProgressBarDetails(outlineTex, emptyTex, fullEnergyTex, HealthPosition1, BarSize);
			//player2healthBar = new ProgressBarDetails(outlineTex, emptyTex, fullHealthTex, EnergyPosition1, BarSize);
			actionTextures = new Texture2D[4]
			{
				_currentPlayer.Kit.Actions[0].ActionTexture,
				_currentPlayer.Kit.Actions[1].ActionTexture,
				_currentPlayer.Kit.Actions[2].ActionTexture,
				_currentPlayer.Kit.Actions[3].ActionTexture
			};
			actionNames = new string[]
			{
				_currentPlayer.Kit.Actions[0].Name,
				_currentPlayer.Kit.Actions[1].Name,
				_currentPlayer.Kit.Actions[2].Name,
				_currentPlayer.Kit.Actions[3].Name
			};

			player1actionBar = new ActionBarDetails(outerActionBarTex, actionTextures, cooldownTex, ActionBarPosition, ActionBarSize, actionNames, tm);
			exchangeTimerDetails = new ExchangeTimerDetails(emptyTex, ExchangeTimerPosition, ExchangeTimerSize);

			foreach (IExchangePlayer player in _players)
			{
				progressBar.DrawProgressBar(player.Position, player1healthBar, player.Health.CurrentPercentage, player.Health.Current.ToString() + "/" + player.Health.Max.ToString());
				progressBar.DrawProgressBar(player.Position, player1energyBar, player.Energy.CurrentPercentage, player.Energy.Current.ToString() + "/" + player.Energy.Max.ToString());
			}

			actionBar.DrawActionBar(player1actionBar);
			
			exchangeTimer.DrawExchangeTimer(exchangeTimerDetails, ((int)tm.GetRemainingCooldown("ExchangeTimer")).ToString());
			GUI.Label(new Rect(Vector2.one, exchangeTimerDetails.Size - new Vector2(1, 1)), _ec.ExchangeState.ToString(), new GUIStyle());
		}
	}
}
