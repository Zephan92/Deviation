using NUnit.Framework;
using Assets.Scripts.Exchange;
using Assets.Scripts.Enum;
using Assets.Scripts.Library;
using Assets.Scripts.Interface;
using Assets.Editor.Controllers;
using Assets.Scripts.Utilities;
using Assets.Scripts.Interface.DTO;
using Assets.Scripts.Interface.Exchange;
using Assets.Editor.DTO;
using UnityEngine;
using System.Threading;

namespace Assets.Editor.Exchange
{
	public class PlayerTests
	{
		//stubs
		private IBattlefieldController _battlefieldController;
		private IExchangeController _exchangeController;
		private ITimerManager _timerManager;
		private AttackStub _attack;
		private ActionStub _action;
		private ModuleStub _module;
		private KitStub _kit;

		//stub variables
		private Battlefield _startField;

		//player variables
		private float _energyRate;
		private int _maxHealth;
		private int _maxEnergy;
		private int _minHealth;
		private int _minEnergy;
		private int _currentColumn;
		private int _currentRow;
		private IPlayer[] _players;

		//sut
		private IPlayer _sut;


		[SetUp]
		public void SetUp()
		{
			_startField = Battlefield.One;
			
			_energyRate = 0.01f;
			_maxHealth = 78;
			_maxEnergy = 102;
			_minHealth = 11;
			_minEnergy = 2;
			_currentColumn = 0;
			_currentRow = 0;

			_players = new IPlayer[4];
			_players.Initialize();
			_attack = new AttackStub(30, -1, 0, 0, -1);
			_action = new ActionStub("default", _attack, Color.gray, "", 0.5f);
			_module = new ModuleStub(_action,"default",new string[] {},ModuleType.Default,Color.black, 1);
			_kit = new KitStub(_module,"dummmmm",new string[] { },1);
			_battlefieldController = new BattlefieldControllerStub(true, _players);
			_exchangeController = new ExchangeControllerStub();
			_timerManager = new TimerManager();
			_sut = new Player();
			_sut.SetBattlefieldController(_battlefieldController);
			_sut.SetExchangeController(_exchangeController);
			_sut.SetTimerManager(_timerManager);
			_sut.SetPlayer(_startField, _kit, _energyRate, _maxHealth, _maxEnergy, _minHealth, _minEnergy);
		}

		[Test]
		public void SetPlayerTests()
		{
			//set
			Player player = new Player();
			player.SetBattlefieldController(_battlefieldController);
			player.SetExchangeController(_exchangeController);
			player.SetTimerManager(_timerManager);

			//test
			player.SetPlayer(_startField, _kit, _energyRate, _maxHealth, _maxEnergy, _minHealth, _minEnergy);

			//assert
			Assert.AreEqual(_startField, player.GetBattlefield());
			Assert.AreEqual(_kit, player.EquipedKit);
			Assert.AreEqual(_energyRate, player.GetEnergyRate());
			Assert.AreEqual(_minHealth, player.GetMinHealth());
			Assert.AreEqual(_minEnergy, player.GetMinEnergy());
			Assert.AreEqual(_maxHealth, player.GetMaxHealth());
			Assert.AreEqual(_maxEnergy, player.GetMaxEnergy());
			Assert.AreEqual(_maxHealth, player.GetHealth());
			Assert.AreEqual(_maxEnergy, player.GetEnergy());
			Assert.AreEqual(_currentColumn, player.GetCurrentColumn());
			Assert.AreEqual(_currentRow, player.GetCurrentRow());

		}

		[Test]
		public void RestoreEnergyTests()
		{
			//setup
			int energyStart = _sut.GetMaxEnergy() / 2;
			_sut.SetEnergy(energyStart);

			//Test: Run 100+ Frames, gain 1 energy
			for (int i = 0; i < 120; i++)
				_sut.RestoreEnergy();

			//Assert
			Assert.AreEqual(energyStart + 1, _sut.GetEnergy());
		}

		[Test]
		public void ResetHealthTests()
		{
			_sut.ResetHealth();
			Assert.AreEqual(_maxHealth, _sut.GetHealth());
		}

		[Test]
		public void ResetEnergyTest()
		{
			_sut.ResetEnergy();
			Assert.AreEqual(_maxEnergy, _sut.GetEnergy());
		}

		[Test]
		public void SetHealthTest()
		{
			int health = 35;
			_sut.SetHealth(health);
			Assert.AreEqual(health, _sut.GetHealth());
		}

		[Test]
		public void SetEnergyTest()
		{
			int energy = 42;
			_sut.SetEnergy(energy);
			Assert.AreEqual(energy, _sut.GetEnergy());
		}

		[Test]
		public void AddHealthTest()
		{
			int health = -63;
			_sut.AddHealth(health);
			Assert.AreEqual(_sut.GetMaxHealth() + health, _sut.GetHealth());
		}

		[Test]
		public void AddEnergyTest()
		{
			int energy = -47;
			_sut.AddEnergy(energy);
			Assert.AreEqual(_sut.GetMaxEnergy() + energy, _sut.GetEnergy());
		}

		[Test]
		public void AddStatWithinBoundaryTestNothing()
		{
			int health = 0;
			_sut.AddHealth(health);
			Assert.AreEqual(_sut.GetMaxHealth(), _sut.GetHealth());
		}

		[Test]
		public void AddStatWithinBoundaryTestFull()
		{
			int health = -(_sut.GetMaxHealth() - _sut.GetMinHealth());
			_sut.AddHealth(health);
			Assert.AreEqual(_sut.GetMinHealth(), _sut.GetHealth());
		}

		[Test]
		public void AddStatWithinBoundaryTestMoreThanMax()
		{
			int health = 50;
			_sut.AddHealth(health);
			Assert.AreEqual(_sut.GetMaxHealth(), _sut.GetHealth());
		}

		[Test]
		public void AddStatWithinBoundaryTestLessThanMin()
		{
			int health = -150;
			_sut.AddHealth(health);
			Assert.AreEqual(_sut.GetMinHealth(), _sut.GetHealth());
		}

		[Test]
		public void MoveObjectTest()
		{
			//need to test
			Direction direction = Direction.Right;
			int distance = 2;
			_sut.MoveObject(direction, distance);
		}

		[Test]
		public void MoveObject_InstantTest()
		{
			//need to test
			int column = 2;
			int row = 1;

			_sut.MoveObject_Instant(row, column);
		}

		[Test]
		public void PrimaryActionTest()
		{
			int energyRecoil = (int)(_sut.GetCurrentAction().Attack.EnergyRecoilModifier * _sut.GetCurrentAction().Attack.BaseDamage);
			_sut.PrimaryAction();
			Assert.IsTrue(_action.InitiateAttackCalled);
		}

		[Test]
		public void PrimaryActionTestNoEnergy()
		{
			_sut.SetEnergy(0);
			_sut.PrimaryAction();
			Assert.IsFalse(_action.InitiateAttackCalled);
		}

		[Test]
		public void PrimaryActionTestTimerNotReady()
		{
			_timerManager.RestartTimer("default");
			_sut.PrimaryAction();
			Assert.IsFalse(_action.InitiateAttackCalled);
		}

		[Test]
		public void PrimaryActionTestTimerReady()
		{
			_timerManager.StartTimer("default");
			_sut.PrimaryAction();
			_timerManager.StopTimer("default");
			_sut.PrimaryAction();
			Assert.IsTrue(_action.InitiateAttackCalled);
		}

		[Test]
		public void PrimaryModuleTest()
		{
			_sut.PrimaryModule();
			//does nothing right now
		}

		[Test]
		public void CycleActionLeftTest()
		{
			IAction action = _sut.GetCurrentAction().GetLeftAction();
			_sut.CycleActionLeft();
			Assert.AreEqual(action.Name, _sut.GetCurrentAction().Name);
		}

		[Test]
		public void CycleActionRightTest()
		{
			IAction action = _sut.GetCurrentAction().GetRightAction();
			_sut.CycleActionRight();
			Assert.AreEqual(action.Name, _sut.GetCurrentAction().Name);
		}

		[Test]
		public void CycleModuleLeftTest()
		{
			IModule action = _sut.GetCurrentModule().GetLeftModule();
			_sut.CycleModuleLeft();
			Assert.AreEqual(action.Name, _sut.GetCurrentModule().Name);
		}

		[Test]
		public void CycleModuleRightTest()
		{
			IModule module = _sut.GetCurrentModule().GetRightModule();
			_sut.CycleModuleRight();
			Assert.AreEqual(module.Name, _sut.GetCurrentModule().Name);
		}

		[Test]
		public void CycleBattlefieldCCTest()
		{
			_sut.CycleBattlefieldCW();
			//does nothing right now
		}

		[Test]
		public void CycleBattlefieldCWTest()
		{
			_sut.CycleBattlefieldCW();
			//does nothing right now
		}

	}
}