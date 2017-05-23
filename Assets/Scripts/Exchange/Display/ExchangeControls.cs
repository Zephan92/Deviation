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

		private IExchangeController ec;
		private ITimerManager tm;

		public void Start()
		{
			tm = FindObjectOfType<TimerManager>();
			exchangeCanvas = GetComponent<Canvas>() ;
			RectTransform canvasRect = exchangeCanvas.transform as RectTransform;
			Vector3[] corners = new Vector3 [4];
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

		public void Awake()
		{
			outlineTex = Resources.Load("User Interface/Black") as Texture2D;
			emptyTex = Resources.Load("User Interface/White") as Texture2D;
			fullEnergyTex = Resources.Load("User Interface/Purple") as Texture2D;
			fullHealthTex = Resources.Load("User Interface/Green") as Texture2D;
			ec = FindObjectOfType<ExchangeController>();
			progressBar = new ProgressBar();
			actionBar = new ActionBar();
			exchangeTimer = new ExchangeTimer();
			player1energyBar = new ProgressBarDetails(outlineTex, emptyTex, fullEnergyTex, HealthPosition, BarSize);
			player1healthBar = new ProgressBarDetails(outlineTex, emptyTex, fullHealthTex, EnergyPosition, BarSize);
			player2energyBar = new ProgressBarDetails(outlineTex, emptyTex, fullEnergyTex, HealthPosition1, BarSize);
			player2healthBar = new ProgressBarDetails(outlineTex, emptyTex, fullHealthTex, EnergyPosition1, BarSize);
			actionTextures = new Texture2D[4] { ec.Players[1].Actions[0].ActionTexture, ec.Players[1].Actions[1].ActionTexture, ec.Players[1].Actions[2].ActionTexture, ec.Players[1].Actions[3].ActionTexture };
			actionNames = new string[] { ec.Players[1].Actions[0].Name, ec.Players[1].Actions[1].Name, ec.Players[1].Actions[2].Name, ec.Players[1].Actions[3].Name };
			player1actionBar = new ActionBarDetails(ec.Players[1].CurrentModule.ModuleTexture, actionTextures, outlineTex, ActionBarPosition, ActionBarSize, actionNames, tm);
			exchangeTimerDetails = new ExchangeTimerDetails(outlineTex, ExchangeTimerPosition, ExchangeTimerSize);
		}
		public void OnGUI()
		{
			if (ec.DisplayEnabled["ExchangeControls"])
			{
				player1energyBar = new ProgressBarDetails(outlineTex, emptyTex, fullEnergyTex, HealthPosition, BarSize);
				player1healthBar = new ProgressBarDetails(outlineTex, emptyTex, fullHealthTex, EnergyPosition, BarSize);
				player2energyBar = new ProgressBarDetails(outlineTex, emptyTex, fullEnergyTex, HealthPosition1, BarSize);
				player2healthBar = new ProgressBarDetails(outlineTex, emptyTex, fullHealthTex, EnergyPosition1, BarSize);
				actionTextures = new Texture2D[4] { ec.Players[1].Actions[0].ActionTexture, ec.Players[1].Actions[1].ActionTexture , ec.Players[1].Actions[2].ActionTexture , ec.Players[1].Actions[3].ActionTexture };
				actionNames = new string[] { ec.Players[1].Actions[0].Name, ec.Players[1].Actions[1].Name, ec.Players[1].Actions[2].Name, ec.Players[1].Actions[3].Name };

				player1actionBar = new ActionBarDetails(ec.Players[1].CurrentModule.ModuleTexture, actionTextures, outlineTex, ActionBarPosition, ActionBarSize, actionNames, tm);
				exchangeTimerDetails = new ExchangeTimerDetails(emptyTex, ExchangeTimerPosition, ExchangeTimerSize);

				progressBar.DrawProgressBar(ec.Players[0].Transform.position, player1healthBar, ec.Players[0].HealthPercentage, ec.Players[0].Health.ToString() + "/" + ec.Players[0].MaxHealth.ToString());
				progressBar.DrawProgressBar(ec.Players[0].Transform.position, player1energyBar, ec.Players[0].EnergyPercentage, ec.Players[0].Energy.ToString() + "/" + ec.Players[0].MaxEnergy.ToString());

				progressBar.DrawProgressBar(ec.Players[1].Transform.position, player2healthBar, ec.Players[1].HealthPercentage, ec.Players[1].Health.ToString() + "/" + ec.Players[1].MaxHealth.ToString());
				progressBar.DrawProgressBar(ec.Players[1].Transform.position, player2energyBar, ec.Players[1].EnergyPercentage, ec.Players[1].Energy.ToString() + "/" + ec.Players[1].MaxEnergy.ToString());

				actionBar.DrawActionBar(player1actionBar);
				exchangeTimer.DrawExchangeTimer(exchangeTimerDetails, ((int) tm.GetRemainingCooldown("ExchangeTimer")).ToString());
			}
			
		}
	}
}
