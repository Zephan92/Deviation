using NUnit.Framework;
using Assets.Scripts.Exchange;
using Assets.Scripts.Enum;
using Assets.Scripts.Interface;
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
		private IExchangeAction _action;
		private IAttack _attack;

		//stub variables
		private BattlefieldZone _startField;

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
			_startField = BattlefieldZone.One;
			
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

			_action = Substitute.For<IExchangeAction>();
			_action.Attack.Returns(_attack);

			_module = Substitute.For<IModule>();
			_module.Actions[0].Returns(_action);

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
		public void TestSetPlayer()
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
			Assert.AreEqual(_startField, player.BattlefieldZone);
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
		public void TestRestoreEnergy()
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
		public void TestResetHealth()
		{
			_sut.ResetHealth();

			Assert.AreEqual(_maxHealth, _sut.Health);
		}

		[Test]
		public void TestResetEnergy()
		{
			_sut.ResetEnergy();

			Assert.AreEqual(_maxEnergy, _sut.Energy);
		}

		[Test]
		public void TestSetHealth()
		{
			int health = 35;

			_sut.SetHealth(health);

			Assert.AreEqual(health, _sut.Health);
		}

		[Test]
		public void TestSetEnergy()
		{
			int energy = 42;

			_sut.SetEnergy(energy);

			Assert.AreEqual(energy, _sut.Energy);
		}

		[Test]
		public void TestAddHealth()
		{
			int health = -63;

			_sut.AddHealth(health);

			Assert.AreEqual(_sut.MaxHealth + health, _sut.Health);
		}

		[Test]
		public void TestAddEnergy()
		{
			int energy = -47;

			_sut.AddEnergy(energy);

			Assert.AreEqual(_sut.MaxEnergy + energy, _sut.Energy);
		}

		[Test]
		public void TestAddStatWithinBoundary_ZeroHealth()
		{
			int health = 0;

			_sut.AddHealth(health);

			Assert.AreEqual(_sut.MaxHealth, _sut.Health);
		}

		[Test]
		public void TestAddStatWithinBoundary_FullHealth()
		{
			int health = -(_sut.MaxHealth - _sut.MinHealth);

			_sut.AddHealth(health);

			Assert.AreEqual(_sut.MinHealth, _sut.Health);
		}

		[Test]
		public void TestAddStatWithinBoundary_MoreThanMax()
		{
			int health = 50;

			_sut.AddHealth(health);

			Assert.AreEqual(_sut.MaxHealth, _sut.Health);
		}

		[Test]
		public void TestAddStatWithinBoundary_LessThanMin()
		{
			int health = -150;

			_sut.AddHealth(health);


			Assert.AreEqual(_sut.MinHealth, _sut.Health);
		}

		//[Test]
		//public void TestMoveObject()
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
		//public void TestMoveObject_Instant()
		//{
		//	int column = 2;
		//	int row = 1;

		//	_sut.MoveObject_Instant(row, column);
		//	Assert.AreEqual(column, _sut.GetCurrentColumn());
		//	Assert.AreEqual(row, _sut.GetCurrentRow());
		//}

		[Test]
		public void TestPrimaryAction()
		{
			//int energyRecoil = (int)(_sut.Actions[0].Attack.EnergyRecoilModifier * _sut.Actions[0].Attack.BaseDamage);

			_sut.DoAction(0);

			//_action.Received().InitiateAttack(_battlefieldController);
			Assert.LessOrEqual(_sut.MaxEnergy,_sut.Energy);
		}

		[Test]
		public void TestPrimaryAction_MinEnergy()
		{
			_sut.SetEnergy(_sut.MinEnergy);

			_sut.DoAction(0);

			//_action.DidNotReceive().InitiateAttack(_battlefieldController);
			Assert.AreEqual(_sut.MinEnergy, _sut.Energy);

		}

		[Test]
		public void TestPrimaryAction_TimerNotReady()
		{
			_timerManager.TimerUp(_action.Name).Returns(false);

			_sut.DoAction(0);

			//_action.DidNotReceive().InitiateAttack(_battlefieldController);
			Assert.AreEqual(_sut.MaxEnergy, _sut.Energy);
		}

		[Test]
		public void TestPrimaryAction_TimerReady()
		{
			_timerManager.StartTimer("default");
			_sut.DoAction(0);
			_timerManager.StopTimer("default");

			_sut.DoAction(0);

			//_action.Received().InitiateAttack(_battlefieldController);
			Assert.LessOrEqual(_sut.MaxEnergy, _sut.Energy);

		}

		[Test]
		public void TestPrimaryModule()
		{
			_sut.PrimaryModule();
			//does nothing right now
		}
	}
}