using Assets.Scripts.Interface;
using System;
using UnityEngine;
using Assets.Scripts.Enum;
using Assets.Scripts.Controllers;
using Assets.Scripts.Library;
using System.Collections;
using Assets.Scripts.Utilities;

namespace Assets.Scripts.Exchange
{
	public class Player : MonoBehaviour, IExchangeObject
	{
		public Kit EquipedKit;
		public Battlefield CurrentBattlefield = Battlefield.One;
		public int CurrentColumn = 0;
		public int CurrentRow = 0;

		private int _health = 100;
		private int _maxHealth = 100;
		private int _energy = 100;
		private int _maxEnergy = 100;
		private float _restoreEnergy = 0.0f;
		private float _energyRate = 0.01f;
		private MovingDetails _movingDetails;

		private BattlefieldController bc;
		private ExchangeController ec;
		private TimerManager tm;

		public Player(Battlefield startField, int startRow, int startColumn, Kit kit, float energyRate, int maxHealth, int maxEnergy)
		{
			CurrentBattlefield = startField;
			UpdateLocation(startRow, startColumn);
			EquipedKit = kit;
			_energyRate = energyRate;
			_maxHealth = maxHealth;
			_maxEnergy = maxEnergy;
			ResetHealth();
			ResetEnergy();
		}

		public void Awake()
		{
			if (bc == null)
			{
				var bcObject = GameObject.FindGameObjectWithTag("BattlefieldController");
				bc = bcObject.GetComponent<BattlefieldController>();
			}

			if (ec == null)
			{
				var ecObject = GameObject.FindGameObjectWithTag("ExchangeController");
				ec = ecObject.GetComponent<ExchangeController>();
			}

			if (EquipedKit == null)
			{
				EquipedKit = KitLibrary.KitLibraryTable["InitialKit"];
			}

			if (tm == null)
			{
				var tmObject = GameObject.FindGameObjectWithTag("ExchangeController");
				tm = tmObject.GetComponent<TimerManager>();
				CreateTimersForKitActions();
			}
			_energyRate = 0.01f;
			_maxHealth = 100;
			_maxEnergy = 100;
			ResetHealth();
			ResetEnergy();
			//bc.SetBattlefieldState(CurrentBattlefield, ConvertToArrayNumber(CurrentRow), ConvertToArrayNumber(CurrentColumn), true);
		}

		public void Update()
		{
			CheckMovingDetails();
			_restoreEnergy += _energyRate;
			if (_restoreEnergy > 1)
			{
				AddEnergy(1);
				if (_energy > _maxEnergy)
					_energy = _maxEnergy;
				_restoreEnergy = 0.0f;
				ec.UpdateExchangeControlsDisplay();
			}

		}

		public void ResetHealth()
		{
			_health = _maxHealth;
		}

		public void ResetEnergy()
		{
			_energy = _maxEnergy;
		}

		public int GetHealth()
		{
			return _health;
		}

		public int GetEnergy()
		{
			return _energy;
		}

		public void SetHealth(int health)
		{
			_health = health;
			if (_health > _maxHealth)
				_health = _maxHealth;
		}

		public void SetEnergy(int energy)
		{
			_energy = energy;
			if (_energy > _maxEnergy)
				_energy = _maxEnergy;
		}

		public void AddHealth(int health)
		{
			_health += health;
			if (_health > _maxHealth)
				_health = _maxHealth;
		}

		public void AddEnergy(int energy)
		{
			_energy += energy;
			if (_energy > _maxEnergy)
				_energy = _maxEnergy;
		}

		//Move Related Methods
		public void MoveObject(Direction direction, int distance, bool force = false)
		{
			if (_movingDetails != null)
			{
				return;
			}
			int destPoint;

			switch (direction)
			{
				case Direction.Right:
					if (force || !bc.GetBattlefieldState(CurrentBattlefield, ConvertToArrayNumber(CurrentRow), ConvertToArrayNumber(CurrentColumn + 1)))
					{
						destPoint = CurrentColumn + 1;
						if (destPoint <= 2)
							_movingDetails = new MovingDetails(new Vector3(destPoint, 0, CurrentRow), direction);
					}
					break;
				case Direction.Left:
					if (force || !bc.GetBattlefieldState(CurrentBattlefield, ConvertToArrayNumber(CurrentRow), ConvertToArrayNumber(CurrentColumn - 1)))
					{
						destPoint = CurrentColumn - 1;
						if (destPoint >= -2)
							_movingDetails = new MovingDetails(new Vector3(destPoint, 0, CurrentRow), direction);
					}
					break;
				case Direction.Up:
					if (force || !bc.GetBattlefieldState(CurrentBattlefield, ConvertToArrayNumber(CurrentRow + 1), ConvertToArrayNumber(CurrentColumn)))
					{
						destPoint = CurrentRow + 1;
						if (destPoint <= 2)
							_movingDetails = new MovingDetails(new Vector3(CurrentColumn, 0, destPoint), direction);
					}
					break;
				case Direction.Down:
					if (force || !bc.GetBattlefieldState(CurrentBattlefield, ConvertToArrayNumber(CurrentRow - 1), ConvertToArrayNumber(CurrentColumn)))
					{
						destPoint = CurrentRow - 1;
						if (destPoint >= -2)
							_movingDetails = new MovingDetails(new Vector3(CurrentColumn, 0, destPoint), direction);
					}
					break;
			}
			
			//UpdateTransform(CurrentRow,CurrentColumn);
		}

		public void MoveObject_Instant(int row, int column)
		{
			UpdateLocation(row, column);
			UpdateTransform(row, column);
		}

		//Kit/Module/Action methods
		public void PrimaryAction()
		{
			Library.Action currentAction = EquipedKit.GetCurrentModule().GetCurrentAction();

			if (tm.IsReady(currentAction.Name) && _energy >= (int) (-1 * currentAction.Attack.EnergyRecoilModifier * currentAction.Attack.BaseDamage))
			{
				currentAction.InitiateAttack(bc);
				
				tm.StartTimer(currentAction.Name);
			}
			else
			{
				//Debug.Log("Cooldown Timer: " + tm.GetRemainingCooldown(currentAction.Name));
			}
		}

		public void PrimaryModule()
		{
			Debug.Log("CurrentModule");
		}

		public void CycleActionLeft()
		{
			EquipedKit.GetCurrentModule().CycleActionLeft();
		}

		public void CycleActionRight()
		{
			EquipedKit.GetCurrentModule().CycleActionRight();
		}

		public void CycleModuleLeft()
		{
			EquipedKit.CycleModuleLeft();
		}

		public void CycleModuleRight()
		{
			EquipedKit.CycleModuleRight();
		}


		//Battlefield Methods
		public void CycleBattlefieldCC()
		{
			
		}

		public void CycleBattlefieldCW()
		{
			
		}

		private void CreateTimersForKitActions()
		{
			Kit kit = EquipedKit;
			Module currentModule = EquipedKit.GetCurrentModule();

			for (int i = 0; i < kit.ModuleCount; i++)
			{
				Library.Action currentAction = currentModule.GetCurrentAction();
				for (int j = 0; j < currentModule.ActionCount; j++)
				{
					tm.AddAttackTimer(currentAction.Name, currentAction.Cooldown);
					currentAction = currentModule.GetRightAction();
					currentModule.CycleActionRight();
				}

				currentModule = kit.GetRightModule();
				kit.CycleModuleRight();
			}
		}

		//Utilities
		private int ConvertToArrayNumber(int input)
		{
			return input + 2;
		}

		private int ConvertFromArrayNumber(int input)
		{
			return input - 2;
		}

		private void UpdateLocation(int row, int column)
		{
			if (!bc.GetBattlefieldState(CurrentBattlefield, ConvertToArrayNumber(column), ConvertToArrayNumber(row)))
			{
				CurrentColumn = column;
				CurrentRow = row;
			}
		}

		private void UpdateTransform(float row, float column)
		{
			transform.localPosition = new Vector3(column, 0, row);
			transform.Translate(0, 0, 0);
		}

		private void CheckMovingDetails()
		{
			if (_movingDetails != null)
			{
				switch (_movingDetails.MovingDirection)
				{
					case Direction.Up:
						if (transform.localPosition.z >= _movingDetails.Destination.z)
						{
							bc.SetBattlefieldState(CurrentBattlefield, ConvertToArrayNumber(CurrentRow), ConvertToArrayNumber(CurrentColumn), false);
							CurrentRow += 1;
							bc.SetBattlefieldState(CurrentBattlefield, ConvertToArrayNumber(CurrentRow), ConvertToArrayNumber(CurrentColumn), true);
							UpdateTransform(CurrentRow, CurrentColumn);
							_movingDetails = null;
						}
						else
						{
							UpdateTransform(transform.localPosition.z + 0.25f, transform.localPosition.x);
						}
						break;
					case Direction.Right:
						if (transform.localPosition.x >= _movingDetails.Destination.x)
						{
							bc.SetBattlefieldState(CurrentBattlefield, ConvertToArrayNumber(CurrentRow), ConvertToArrayNumber(CurrentColumn), false);
							CurrentColumn += 1;
							bc.SetBattlefieldState(CurrentBattlefield, ConvertToArrayNumber(CurrentRow), ConvertToArrayNumber(CurrentColumn), true);
							UpdateTransform(CurrentRow, CurrentColumn);
							_movingDetails = null;
						}
						else
						{
							UpdateTransform(transform.localPosition.z, transform.localPosition.x + 0.25f);
						}
						break;
					case Direction.Left:
						if (transform.localPosition.x <= _movingDetails.Destination.x)
						{
							bc.SetBattlefieldState(CurrentBattlefield, ConvertToArrayNumber(CurrentRow), ConvertToArrayNumber(CurrentColumn), false);
							CurrentColumn -= 1;
							bc.SetBattlefieldState(CurrentBattlefield, ConvertToArrayNumber(CurrentRow), ConvertToArrayNumber(CurrentColumn), true);
							UpdateTransform(CurrentRow, CurrentColumn);
							_movingDetails = null;
						}
						else
						{
							UpdateTransform(transform.localPosition.z, transform.localPosition.x - 0.25f);
						}
						break;
					case Direction.Down:
						if (transform.localPosition.z <= _movingDetails.Destination.z)
						{
							bc.SetBattlefieldState(CurrentBattlefield, ConvertToArrayNumber(CurrentRow), ConvertToArrayNumber(CurrentColumn), false);
							CurrentRow -= 1;
							bc.SetBattlefieldState(CurrentBattlefield, ConvertToArrayNumber(CurrentRow), ConvertToArrayNumber(CurrentColumn), true);
							UpdateTransform(CurrentRow, CurrentColumn);
							_movingDetails = null;
						}
						else
						{
							UpdateTransform(transform.localPosition.z - 0.25f, transform.localPosition.x);
						}
						break;
				}
			}
		}
	}
}
