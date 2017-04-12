using Assets.Scripts.Controllers;
using Assets.Scripts.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Exchange.Display
{
	public class ExchangeControls : MonoBehaviour
	{
		public Vector2 HealthPosition = new Vector2(20, 30);
		public Vector2 EnergyPosition = new Vector2(20, 40);
		public Vector2 HealthPosition1 = new Vector2(20, 30);
		public Vector2 EnergyPosition1 = new Vector2(20, 40);
		public Vector2 Size = new Vector2(60, 20);

		public Texture2D outlineTex;
		public Texture2D emptyTex;
		public Texture2D fullEnergyTex;
		public Texture2D fullHealthTex;

		public Material fontMaterial;

		public bool toggle;

		private IProgressBar progressBar;
		private ProgressBarDetails player1energyBar;
		private ProgressBarDetails player1healthBar;

		private ProgressBarDetails player2energyBar;
		private ProgressBarDetails player2healthBar;

		private IExchangeController ec;
		public void Awake()
		{
			ec = FindObjectOfType<ExchangeController>();
			progressBar = new ProgressBar();
			player1energyBar = new ProgressBarDetails(outlineTex, emptyTex, fullEnergyTex, HealthPosition, Size);
			player1healthBar = new ProgressBarDetails(outlineTex, emptyTex, fullHealthTex, EnergyPosition, Size);
			player2energyBar = new ProgressBarDetails(outlineTex, emptyTex, fullEnergyTex, HealthPosition1, Size);
			player2healthBar = new ProgressBarDetails(outlineTex, emptyTex, fullHealthTex, EnergyPosition1, Size);
		}
		public void OnGUI()
		{
			if (ec.DisplayEnabled["ExchangeControls"])
			{
				player1energyBar = new ProgressBarDetails(outlineTex, emptyTex, fullEnergyTex, HealthPosition, Size);
				player1healthBar = new ProgressBarDetails(outlineTex, emptyTex, fullHealthTex, EnergyPosition, Size);
				player2energyBar = new ProgressBarDetails(outlineTex, emptyTex, fullEnergyTex, HealthPosition1, Size);
				player2healthBar = new ProgressBarDetails(outlineTex, emptyTex, fullHealthTex, EnergyPosition1, Size);

				progressBar.DrawProgressBar(player1healthBar, ec.Players[0].HealthPercentage, ec.Players[0].Health.ToString() + "/" + ec.Players[0].MaxHealth.ToString());
				progressBar.DrawProgressBar(player1energyBar, ec.Players[0].EnergyPercentage, ec.Players[0].Energy.ToString() + "/" + ec.Players[0].MaxEnergy.ToString());

				progressBar.DrawProgressBar(player2healthBar, ec.Players[1].HealthPercentage, ec.Players[1].Health.ToString() + "/" + ec.Players[1].MaxHealth.ToString());
				progressBar.DrawProgressBar(player2energyBar, ec.Players[1].EnergyPercentage, ec.Players[1].Energy.ToString() + "/" + ec.Players[1].MaxEnergy.ToString());
			}
			
		}
	}
}
