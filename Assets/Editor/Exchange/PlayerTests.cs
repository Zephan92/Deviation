using NUnit.Framework;
using Assets.Scripts.Exchange;
using Assets.Scripts.Enum;
using Assets.Scripts.Library;
using Assets.Scripts.Interface;
using Assets.Scripts.Utilities;
using Assets.Scripts.Interface.DTO;
using Assets.Scripts.Interface.Exchange;
using NSubstitute;

namespace Assets.Editor.Exchange
{
	public class PlayerTests
	{
		//stubs
		private IBattlefieldController _battlefieldController;
		private IExchangeController _exchangeController;
		private ITimerManager _timerManager;
		private INPCController _npcController;
		private IKit _kit;
		private IModule _module;
		private IAction _action;
		private IAttack _attack;

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
			for(int i = 0; i < _players.Length; i++)
			{
				_players[i] = new Player();
			}

			_attack = Substitute.For<IAttack>();
			_attack.EnergyRecoilModifier.Returns(-1);
			_attack.BaseDamage.Returns(35);

			_action = Substitute.For<IAction>();
			_action.Attack.Returns(_attack);

			_module = Substitute.For<IModule>();
			_module.GetCurrentAction().Returns(_action);

			_kit = Substitute.For<IKit>();
			_kit.GetCurrentModule().Returns(_module);

			_battlefieldController = Substitute.For<IBattlefieldController>();
			_exchangeController = Substitute.For<IExchangeController>();
			_npcController = Substitute.For<INPCController>();

			_timerManager = Substitute.For<ITimerManager>();
			_timerManager.TimerUp(_action.Name).Returns(true);

			_sut = new Player();
			_sut.BattlefieldController = _battlefieldController;
			_sut.ExchangeController = _exchangeController;
			_sut.TimerManager = _timerManager;
			_sut.Enemies = _players;
			_sut.NPCController = _npcController;
			_sut.SetPlayer(true, _startField, _kit, _energyRate, _maxHealth, _maxEnergy, _minHealth, _minEnergy);
		}

		[Test]
		public void SetPlayerTests()
		{
			//Arrange
			Player player = new Player();
			player.BattlefieldController = _battlefieldController;
			player.ExchangeController = _exchangeController;
			player.TimerManager = _timerManager;
			player.Enemies = _players;

			//Act
			player.SetPlayer(true, _startField, _kit, _energyRate, _maxHealth, _maxEnergy, _minHealth, _minEnergy);

			//Assert
			Assert.AreEqual(_startField, player.Battlefield);
			Assert.AreEqual(_kit, player.EquipedKit);
			Assert.AreEqual(_energyRate, player.EnergyRate);
			Assert.AreEqual(_minHealth, player.MinHealth);
			Assert.AreEqual(_minEnergy, player.MinEnergy);
			Assert.AreEqual(_maxHealth, player.MaxHealth);
			Assert.AreEqual(_maxEnergy, player.MaxEnergy);
			Assert.AreEqual(_maxHealth, player.Health);
			Assert.AreEqual(_maxEnergy, player.Energy);
			Assert.AreEqual(_currentColumn, player.CurrentColumn);
			Assert.AreEqual(_currentRow, player.CurrentRow);
		}

		[Test]
		public void RestoreEnergyTests()
		{
			//Arrange
			int energyStart = _sut.MaxEnergy / 2;
			_sut.SetEnergy(energyStart);

			//Act
			//Test: Run 100+ Frames, gain 1 energy
			for (int i = 0; i < 120; i++)
				_sut.RestoreEnergy();

			//Assert
			Assert.AreEqual(energyStart + 1, _sut.Energy);
		}

		[Test]
		public void ResetHealthTests()
		{
			_sut.ResetHealth();

			Assert.AreEqual(_maxHealth, _sut.Health);
		}

		[Test]
		public void ResetEnergyTest()
		{
			_sut.ResetEnergy();

			Assert.AreEqual(_maxEnergy, _sut.Energy);
		}

		[Test]
		public void SetHealthTest()
		{
			int health = 35;

			_sut.SetHealth(health);

			Assert.AreEqual(health, _sut.Health);
		}

		[Test]
		public void SetEnergyTest()
		{
			int energy = 42;

			_sut.SetEnergy(energy);

			Assert.AreEqual(energy, _sut.Energy);
		}

		[Test]
		public void AddHealthTest()
		{
			int health = -63;

			_sut.AddHealth(health);

			Assert.AreEqual(_sut.MaxHealth + health, _sut.Health);
		}

		[Test]
		public void AddEnergyTest()
		{
			int energy = -47;

			_sut.AddEnergy(energy);

			Assert.AreEqual(_sut.MaxEnergy + energy, _sut.Energy);
		}

		[Test]
		public void AddStatWithinBoundaryTestNothing()
		{
			int health = 0;

			_sut.AddHealth(health);

			Assert.AreEqual(_sut.MaxHealth, _sut.Health);
		}

		[Test]
		public void AddStatWithinBoundaryTestFull()
		{
			int health = -(_sut.MaxHealth - _sut.MinHealth);

			_sut.AddHealth(health);

			Assert.AreEqual(_sut.MinHealth, _sut.Health);
		}

		[Test]
		public void AddStatWithinBoundaryTestMoreThanMax()
		{
			int health = 50;

			_sut.AddHealth(health);

			Assert.AreEqual(_sut.MaxHealth, _sut.Health);
		}

		[Test]
		public void AddStatWithinBoundaryTestLessThanMin()
		{
			int health = -150;

			_sut.AddHealth(health);


			Assert.AreEqual(_sut.MinHealth, _sut.Health);
		}

		//[Test]
		//public void MoveObjectTest()
		//{
		//	//need to test
		//	Direction direction = Direction.Right;
		//	int distance = 2;
		//	int column = _sut.GetCurrentColumn();
		//	int row = _sut.GetCurrentRow();
		//	_sut.MoveObject(direction, distance);
		//	Assert.AreEqual(column + 2, _sut.GetCurrentColumn());
		//	Assert.AreEqual(row, _sut.GetCurrentRow());
		//}

		//[Test]
		//public void MoveObject_InstantTest()
		//{
		//	int column = 2;
		//	int row = 1;

		//	_sut.MoveObject_Instant(row, column);
		//	Assert.AreEqual(column, _sut.GetCurrentColumn());
		//	Assert.AreEqual(row, _sut.GetCurrentRow());
		//}

		[Test]
		public void PrimaryActionTest()
		{
			int energyRecoil = (int)(_sut.CurrentAction.Attack.EnergyRecoilModifier * _sut.CurrentAction.Attack.BaseDamage);

			_sut.PrimaryAction();

			_action.Received().InitiateAttack(_battlefieldController);
			Assert.LessOrEqual(_sut.MaxEnergy,_sut.Energy);
		}

		[Test]
		public void PrimaryActionTestNoEnergy()
		{
			_sut.SetEnergy(_sut.MinEnergy);

			_sut.PrimaryAction();

			_action.DidNotReceive().InitiateAttack(_battlefieldController);
			Assert.AreEqual(_sut.MinEnergy, _sut.Energy);

		}

		[Test]
		public void PrimaryActionTestTimerNotReady()
		{
			_timerManager.TimerUp(_action.Name).Returns(false);

			_sut.PrimaryAction();

			_action.DidNotReceive().InitiateAttack(_battlefieldController);
			Assert.AreEqual(_sut.MaxEnergy, _sut.Energy);
		}

		[Test]
		public void PrimaryActionTestTimerReady()
		{
			_timerManager.StartTimer("default");
			_sut.PrimaryAction();
			_timerManager.StopTimer("default");

			_sut.PrimaryAction();

			_action.Received().InitiateAttack(_battlefieldController);
			Assert.LessOrEqual(_sut.MaxEnergy, _sut.Energy);

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
			IAction action = _sut.CurrentAction.GetLeftAction();

			_sut.CycleActionLeft();

			_module.Received().CycleActionLeft();
			Assert.AreEqual(action.Name, _sut.CurrentAction.Name);
		}

		[Test]
		public void CycleActionRightTest()
		{
			IAction action = _sut.CurrentAction.GetRightAction();

			_sut.CycleActionRight();

			_module.Received().CycleActionRight();
			Assert.AreEqual(action.Name, _sut.CurrentAction.Name);
		}

		[Test]
		public void CycleModuleLeftTest()
		{
			IModule action = _sut.CurrentModule.GetLeftModule();

			_sut.CycleModuleLeft();

			_kit.Received().CycleModuleLeft();
			Assert.AreEqual(action.Name, _sut.CurrentModule.Name);
		}

		[Test]
		public void CycleModuleRightTest()
		{
			IModule module = _sut.CurrentModule.GetRightModule();

			_sut.CycleModuleRight();

			_kit.Received().CycleModuleRight();
			Assert.AreEqual(module.Name, _sut.CurrentModule.Name);
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