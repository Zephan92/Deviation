using System;
using Assets.Scripts.Enum;
using Assets.Scripts.Interface;

namespace Assets.Editor.Controllers
{
	public class ExchangeControllerStub : IExchangeController
	{
		public Battlefield GetMainPlayerFieldNumber { get; set; }
		public int GetNumberOfPlayers { get; set; }


		public Battlefield MainPlayerFieldNumber
		{
			get
			{
				return GetMainPlayerFieldNumber;
			}
		}

		public int NumberOfPlayers
		{
			get
			{
				return 2;
			}
		}

		public void UpdateExchangeControlsDisplay()
		{
		}
	}
}
