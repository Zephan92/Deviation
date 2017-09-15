using Assets.Scripts.Controllers;
using Assets.Scripts.Interface;
using Assets.Scripts.Utilities;
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

		private ProgressBarDetails player2energyBar;
		private ProgressBarDetails player2healthBar;

		private Canvas exchangeCanvas;

		private ExchangePlayer [] players;
		private ITimerManager tm;
		private IExchangeController1v1 _ec;
		public void Awake()
		{
			_ec = FindObjectOfType<ExchangeController1v1>();
			outlineTex = Resources.Load("User Interface/Black") as Texture2D;
			emptyTex = Resources.Load("User Interface/White") as Texture2D;
			fullEnergyTex = Resources.Load("User Interface/Purple") as Texture2D;
			fullHealthTex = Resources.Load("User Interface/Green") as Texture2D;
			progressBar = new ProgressBar();
			//actionBar = new ActionBar();
			exchangeTimer = new ExchangeTimer();
			player1energyBar = new ProgressBarDetails(outlineTex, emptyTex, fullEnergyTex, HealthPosition, BarSize);
			player1healthBar = new ProgressBarDetails(outlineTex, emptyTex, fullHealthTex, EnergyPosition, BarSize);
			player2energyBar = new ProgressBarDetails(outlineTex, emptyTex, fullEnergyTex, HealthPosition1, BarSize);
			player2healthBar = new ProgressBarDetails(outlineTex, emptyTex, fullHealthTex, EnergyPosition1, BarSize);
			//actionTextures = new Texture2D[4] { Players[1].Actions[0].ActionTexture, ec.Players[1].Actions[1].ActionTexture, ec.Players[1].Actions[2].ActionTexture, ec.Players[1].Actions[3].ActionTexture };
			//actionNames = new string[] { ec.Players[1].Actions[0].Name, ec.Players[1].Actions[1].Name, ec.Players[1].Actions[2].Name, ec.Players[1].Actions[3].Name };
			//player1actionBar = new ActionBarDetails(ec.Players[1].CurrentModule.ModuleTexture, actionTextures, outlineTex, ActionBarPosition, ActionBarSize, actionNames, tm);
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

			if (_ec.ExchangePlayers == null)
			{
				return;
			}

			if (players == null)
			{
				var playerObjects = FindObjectsOfType<ExchangePlayer>();

				if (playerObjects.Length == 2)
				{
					players = playerObjects;
				}
			}

		}

		public void OnGUI()
		{
			if (_ec == null || tm == null || players == null)
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
			player2energyBar = new ProgressBarDetails(outlineTex, emptyTex, fullEnergyTex, HealthPosition1, BarSize);
			player2healthBar = new ProgressBarDetails(outlineTex, emptyTex, fullHealthTex, EnergyPosition1, BarSize);
			//actionTextures = new Texture2D[4] { ec.Players[1].Actions[0].ActionTexture, ec.Players[1].Actions[1].ActionTexture , ec.Players[1].Actions[2].ActionTexture , ec.Players[1].Actions[3].ActionTexture };
			//actionNames = new string[] { ec.Players[1].Actions[0].Name, ec.Players[1].Actions[1].Name, ec.Players[1].Actions[2].Name, ec.Players[1].Actions[3].Name };

			//player1actionBar = new ActionBarDetails(ec.Players[1].CurrentModule.ModuleTexture, actionTextures, outlineTex, ActionBarPosition, ActionBarSize, actionNames, tm);
			exchangeTimerDetails = new ExchangeTimerDetails(emptyTex, ExchangeTimerPosition, ExchangeTimerSize);

			progressBar.DrawProgressBar(players[0].transform.position, player1healthBar, players[0].Health.CurrentPercentage, players[0].Health.Current.ToString() + "/" + players[0].Health.Max.ToString());
			progressBar.DrawProgressBar(players[0].transform.position, player1energyBar, players[0].Energy.CurrentPercentage, players[0].Energy.Current.ToString() + "/" + players[0].Energy.Max.ToString());
			progressBar.DrawProgressBar(players[1].transform.position, player2healthBar, players[1].Health.CurrentPercentage, players[1].Health.Current.ToString() + "/" + players[1].Health.Max.ToString());
			progressBar.DrawProgressBar(players[1].transform.position, player2energyBar, players[1].Energy.CurrentPercentage, players[1].Energy.Current.ToString() + "/" + players[1].Energy.Max.ToString());

			//actionBar.DrawActionBar(player1actionBar);
			
			exchangeTimer.DrawExchangeTimer(exchangeTimerDetails, ((int)tm.GetRemainingCooldown("ExchangeTimer")).ToString());
			GUI.Label(new Rect(Vector2.one, exchangeTimerDetails.Size - new Vector2(1, 1)), _ec.ExchangeState.ToString(), new GUIStyle());
		}
	}
}
