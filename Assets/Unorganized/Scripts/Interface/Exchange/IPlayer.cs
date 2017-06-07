using Assets.Scripts.Enum;
using Assets.Scripts.Interface.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Interface.Exchange
{
	public interface IPlayer
	{
		bool IsMainPlayer { get; set; }
		int Health { get; set; }
		int MinHealth { get; set; }
		int MaxHealth { get; set; }
		float HealthPercentage { get; }
		int Energy { get; set; }
		int MinEnergy { get; set; }
		int MaxEnergy { get; set; }
		float EnergyPercentage { get; }
		float EnergyRate { get; set; }
		int CurrentColumn { get; set; }
		int CurrentRow { get; set; }

		Battlefield Battlefield { get; set; }
		Transform Transform { get;  }
		IPlayer[] Enemies { get; set; }
		IKit EquipedKit { get; set; }
		IModule CurrentModule { get; set; }
		IExchangeAction [] Actions { get; set; }

		IBattlefieldController BattlefieldController { get; set; }
		IExchangeController ExchangeController { get; set; }
		ITimerManager TimerManager { get; set; }
		INPCController NPCController { get; set; }

		void SetPlayer(bool isMainPlayer, Battlefield startField, IKit kit, float energyRate, int maxHealth, int maxEnergy, int minHealth, int minEnergy);

		void RestoreEnergy();
		void ResetEnergy();
		void SetEnergy(int set);
		void AddEnergy(int add);

		void SetHealth(int set);
		void ResetHealth();
		void AddHealth(int add);

		bool MoveObject(Direction direction, int distance, bool force = false);
		void MoveObject_Instant(int row, int column);

		bool DoAction(int actionNumber);
		bool PrimaryModule();
	}
}
