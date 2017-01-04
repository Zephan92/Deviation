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
		Transform Transform { get;  }
		Battlefield Battlefield { get; set; }
		IKit EquipedKit { get; set; }
		int Health { get; set; }
		int MinHealth { get; set; }
		int MaxHealth { get; set; }
		int Energy { get; set; }
		int MinEnergy { get; set; }
		int MaxEnergy { get; set; }
		float EnergyRate { get; set; }
		IPlayer[] Enemies { get; set; }
		IBattlefieldController BattlefieldController { get; set; }
		IExchangeController ExchangeController { get; set; }
		ITimerManager TimerManager { get; set; }
		INPCController NPCController { get; set; }
		int CurrentColumn { get; set; }
		int CurrentRow { get; set; }
		IModule CurrentModule { get; set; }
		IAction CurrentAction { get; set; }

		void SetPlayer(bool isMainPlayer, Battlefield startField, IKit kit, float energyRate, int maxHealth, int maxEnergy, int minHealth, int minEnergy);
		void RestoreEnergy();
		void ResetHealth();
		void ResetEnergy();
		void SetHealth(int set);
		void SetEnergy(int set);
		void AddHealth(int add);
		void AddEnergy(int add);
		bool MoveObject(Direction direction, int distance, bool force = false);
		void MoveObject_Instant(int row, int column);
		bool PrimaryAction();
		bool PrimaryModule();
		void CycleActionLeft();
		void CycleActionRight();
		void CycleModuleLeft();
		void CycleModuleRight();
		void CycleBattlefieldCC();
		void CycleBattlefieldCW();
	}
}
