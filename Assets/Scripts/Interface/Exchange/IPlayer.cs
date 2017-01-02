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

		void SetBattlefieldController(IBattlefieldController controller);
		void SetExchangeController(IExchangeController controller);
		void SetTimerManager(ITimerManager manager);
		IKit EquipedKit { get; set; }
		IPlayer[] Enemies { get; set; }

		void SetPlayer(bool isMainPlayer, Battlefield startField, IKit kit, float energyRate, int maxHealth, int maxEnergy, int minHealth, int minEnergy);
		Transform Transform { get; }
		int GetHealth();
		int GetEnergy();
		float GetEnergyRate();
		int GetMaxHealth();
		int GetMaxEnergy();
		int GetMinHealth();
		int GetMinEnergy();
		IModule GetCurrentModule();
		IAction GetCurrentAction();
		Battlefield GetBattlefield();
		void SetEnemies(IPlayer[] enemies);
		int GetCurrentColumn();
		int GetCurrentRow();

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
