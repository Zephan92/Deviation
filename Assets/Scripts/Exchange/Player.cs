using UnityEngine;
using Assets.Scripts.Enum;
using Assets.Scripts.Controllers;
using Assets.Scripts.Library;
using Assets.Scripts.Utilities;
using Assets.Scripts.Interface;
using Assets.Scripts.Interface.DTO;
using Assets.Scripts.Interface.Exchange;

namespace Assets.Scripts.Exchange
{
	public class Player : MonoBehaviour, IExchangeObject, IPlayer
	{
		public Transform Transform { get { return transform; } }
		private Battlefield _battlefield;
		public IKit EquipedKit { get; set; }
		private int _health, _minHealth, _maxHealth;
		private int _energy, _minEnergy, _maxEnergy;
		private float _energyRate;
		private IPlayer[] _enemies;

		private int _currentColumn, _currentRow;
		private MovingDetails _movingDetails;
		private float _restoreEnergy;

		private IBattlefieldController bc;
		private IExchangeController ec;
		private ITimerManager tm;

		public void SetPlayer(Battlefield startField, IKit kit, float energyRate, int maxHealth, int maxEnergy, int minHealth, int minEnergy)
		{
			_battlefield = startField;
			EquipedKit = kit;
			_energyRate = energyRate;
			_minHealth = minHealth;
			_minEnergy = minEnergy;
			_maxHealth = maxHealth;
			_maxEnergy = maxEnergy;

			_restoreEnergy = 0;
			ResetHealth();
			ResetEnergy();
			UpdateLocation(0, 0);
			CreateTimersForKitActions();
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

			if (tm == null)
			{
				var tmObject = GameObject.FindGameObjectWithTag("ExchangeController");
				tm = tmObject.GetComponent<TimerManager>();
			}

			//bc.SetBattlefieldState(CurrentBattlefield, ConvertToArrayNumber(_currentRow), ConvertToArrayNumber(_currentColumn), true);
		}

		public void Update()
		{
			CheckMovingDetails();
			RestoreEnergy();
		}

		//Getters and Setters
		public void SetBattlefieldController(IBattlefieldController controller)
		{
			bc = controller;
		}

		public void SetExchangeController(IExchangeController controller)
		{
			ec = controller;
		}

		public int GetHealth()
		{
			return _health;
		}

		public int GetEnergy()
		{
			return _energy;
		}

		public float GetEnergyRate()
		{
			return _energyRate;
		}

		public int GetMaxHealth()
		{
			return _maxHealth;
		}

		public int GetMaxEnergy()
		{
			return _maxEnergy;
		}

		public int GetMinHealth()
		{
			return _minHealth;
		}

		public int GetMinEnergy()
		{
			return _minEnergy;
		}


		public IModule GetCurrentModule()
		{
			return EquipedKit.GetCurrentModule();
		}

		public IAction GetCurrentAction()
		{
			return EquipedKit.GetCurrentModule().GetCurrentAction();
		}

		public Battlefield GetBattlefield()
		{
			return _battlefield;
		}

		public void SetTimerManager(ITimerManager manager)
		{
			tm = manager;
		}

		public void SetEnemies(IPlayer[] enemies)
		{
			_enemies = enemies;
		}

		public int GetCurrentColumn()
		{
			return _currentColumn;
		}

		public int GetCurrentRow()
		{
			return _currentRow;
		}


		//restores energy
		public void RestoreEnergy()
		{
			_restoreEnergy += _energyRate;
			if (_restoreEnergy > 1)
			{
				AddEnergy(1);
				_restoreEnergy = 0.0f;
				ec.UpdateExchangeControlsDisplay();
			}
		}

		//reset health to max
		public void ResetHealth()
		{
			_health = _maxHealth;
		}

		//reset energy to max
		public void ResetEnergy()
		{
			_energy = _maxEnergy;
		}

		//sets health
		public void SetHealth(int set)
		{
			_health = AddStatWithinBoundary(set, 0, _minHealth, _maxHealth);
		}

		//sets energy
		public void SetEnergy(int set)
		{
			_energy = AddStatWithinBoundary(set, 0, _minEnergy, _maxEnergy);
		}

		//adds specified health
		public void AddHealth(int add)
		{
			_health = AddStatWithinBoundary(_health, add, _minHealth, _maxHealth);
		}

		//adds specified energy
		public void AddEnergy(int add)
		{
			_energy = AddStatWithinBoundary(_energy, add, _minEnergy, _maxEnergy);
		}

		private int AddStatWithinBoundary(int current, int add, int statMin, int statMax)
		{
			int retVal = current + add;

			if (retVal > statMax)
			{
				retVal = statMax;

			}
			else if (retVal < statMin)
			{
				retVal = statMin;
			}

			return retVal;
		}

		//moves the player over time
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
					if (force || !bc.GetBattlefieldState(_battlefield, ConvertToArrayNumber(_currentRow), ConvertToArrayNumber(_currentColumn + 1)))
					{
						destPoint = _currentColumn + 1;
						if (destPoint <= 2)
							_movingDetails = new MovingDetails(new Vector3(destPoint, 0, _currentRow), direction);
					}
					break;
				case Direction.Left:
					if (force || !bc.GetBattlefieldState(_battlefield, ConvertToArrayNumber(_currentRow), ConvertToArrayNumber(_currentColumn - 1)))
					{
						destPoint = _currentColumn - 1;
						if (destPoint >= -2)
							_movingDetails = new MovingDetails(new Vector3(destPoint, 0, _currentRow), direction);
					}
					break;
				case Direction.Up:
					if (force || !bc.GetBattlefieldState(_battlefield, ConvertToArrayNumber(_currentRow + 1), ConvertToArrayNumber(_currentColumn)))
					{
						destPoint = _currentRow + 1;
						if (destPoint <= 2)
							_movingDetails = new MovingDetails(new Vector3(_currentColumn, 0, destPoint), direction);
					}
					break;
				case Direction.Down:
					if (force || !bc.GetBattlefieldState(_battlefield, ConvertToArrayNumber(_currentRow - 1), ConvertToArrayNumber(_currentColumn)))
					{
						destPoint = _currentRow - 1;
						if (destPoint >= -2)
							_movingDetails = new MovingDetails(new Vector3(_currentColumn, 0, destPoint), direction);
					}
					break;
			}
		}

		//moves the player instantly
		public void MoveObject_Instant(int row, int column)
		{
			UpdateLocation(row, column);
			UpdateTransform(row, column);
		}

		//uses the current action
		public void PrimaryAction()
		{
			IAction currentAction = EquipedKit.GetCurrentModule().GetCurrentAction();

			int attackCost = (int) (currentAction.Attack.EnergyRecoilModifier * currentAction.Attack.BaseDamage);
			int potentialEnergy = GetEnergy() + attackCost;
			if (tm.TimerUp(currentAction.Name) && potentialEnergy >= _minEnergy)
			{
				currentAction.InitiateAttack(bc);
				
				tm.StartTimer(currentAction.Name);
			}
			else
			{
				//Debug.Log("Cooldown Timer: " + tm.GetRemainingCooldown(currentAction.Name));
			}
		}

		//uses the current module ultimate
		public void PrimaryModule()
		{
			Debug.Log("CurrentModule");
		}

		//Cycle Action Left
		public void CycleActionLeft()
		{
			EquipedKit.GetCurrentModule().CycleActionLeft();
		}

		//Cycle Action Right
		public void CycleActionRight()
		{
			EquipedKit.GetCurrentModule().CycleActionRight();
		}

		//Cycle Module Left
		public void CycleModuleLeft()
		{
			EquipedKit.CycleModuleLeft();
		}

		//Cycle Module Right
		public void CycleModuleRight()
		{
			EquipedKit.CycleModuleRight();
		}


		//Cycles the Battlefield counter clockwise
		public void CycleBattlefieldCC()
		{
			
		}

		//cycles the battlefiled clockwise
		public void CycleBattlefieldCW()
		{
			
		}

		//create a timer for each action in each module
		private void CreateTimersForKitActions()
		{
			IKit kit = EquipedKit;
			IModule currentModule = EquipedKit.GetCurrentModule();

			for (int i = 0; i < kit.ModuleCount; i++)
			{
				IAction currentAction = currentModule.GetCurrentAction();
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

		//Utilities move this!!!!!!
		private int ConvertToArrayNumber(int input)
		{
			return input + 2;
		}

		private int ConvertFromArrayNumber(int input)
		{
			return input - 2;
		}

		//update current location of the player
		private void UpdateLocation(int row, int column)
		{
			if (!bc.GetBattlefieldState(_battlefield, ConvertToArrayNumber(column), ConvertToArrayNumber(row)))
			{
				_currentColumn = column;
				_currentRow = row;
			}
		}

		//moves the player over time
		private void UpdateTransform(float row, float column)
		{
			transform.localPosition = new Vector3(column, 0, row);
			transform.Translate(0, 0, 0);
		}

		//update moving details
		private void CheckMovingDetails()
		{
			if (_movingDetails != null)
			{
				switch (_movingDetails.MovingDirection)
				{
					case Direction.Up:
						if (transform.localPosition.z >= _movingDetails.Destination.z)
						{
							bc.SetBattlefieldState(_battlefield, ConvertToArrayNumber(_currentRow), ConvertToArrayNumber(_currentColumn), false);
							_currentRow += 1;
							bc.SetBattlefieldState(_battlefield, ConvertToArrayNumber(_currentRow), ConvertToArrayNumber(_currentColumn), true);
							UpdateTransform(_currentRow, _currentColumn);
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
							bc.SetBattlefieldState(_battlefield, ConvertToArrayNumber(_currentRow), ConvertToArrayNumber(_currentColumn), false);
							_currentColumn += 1;
							bc.SetBattlefieldState(_battlefield, ConvertToArrayNumber(_currentRow), ConvertToArrayNumber(_currentColumn), true);
							UpdateTransform(_currentRow, _currentColumn);
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
							bc.SetBattlefieldState(_battlefield, ConvertToArrayNumber(_currentRow), ConvertToArrayNumber(_currentColumn), false);
							_currentColumn -= 1;
							bc.SetBattlefieldState(_battlefield, ConvertToArrayNumber(_currentRow), ConvertToArrayNumber(_currentColumn), true);
							UpdateTransform(_currentRow, _currentColumn);
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
							bc.SetBattlefieldState(_battlefield, ConvertToArrayNumber(_currentRow), ConvertToArrayNumber(_currentColumn), false);
							_currentRow -= 1;
							bc.SetBattlefieldState(_battlefield, ConvertToArrayNumber(_currentRow), ConvertToArrayNumber(_currentColumn), true);
							UpdateTransform(_currentRow, _currentColumn);
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
