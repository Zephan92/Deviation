using Assets.Scripts.Interface.Exchange;
using Assets.Scripts.Exchange;
using NUnit.Framework;
using NSubstitute;
using Assets.Scripts.Interface;
using Assets.Scripts.Enum;
using Assets.Scripts.Interface.DTO;

namespace Assets.Editor.Exchange
{
	public class MainPlayerControllerTests
	{
		IMainPlayerController _sut;
		IInput _input;
		IPlayer _player;
		IExchangeController _exchangeController;
		IModule _module;

		[SetUp]
		public void SetUp()
		{
			_input = Substitute.For<IInput>();
			_player = Substitute.For<IPlayer>();
			_exchangeController = Substitute.For<IExchangeController>();
			_module = Substitute.For<IModule>();

			_sut = new MainPlayerController();
			_sut.Input = _input;
			_sut.MainPlayer = _player;
			_sut.ExchangeController = _exchangeController;
		}

		[Test]
		public void TestCheckInput_NoInput()
		{
			_sut.CheckInput();

			_player.DidNotReceiveWithAnyArgs().MoveObject(Direction.None,1);
		}

		[Test]
		public void TestCheckInput_Up()
		{
			_input.IsUpPressed().Returns(true);

			_sut.CheckInput();

			_player.Received().MoveObject(Direction.Up, 1);
		}

		[Test]
		public void TestCheckInput_Down()
		{
			_input.IsDownPressed().Returns(true);

			_sut.CheckInput();

			_player.Received().MoveObject(Direction.Down, 1);
		}

		[Test]
		public void TestCheckInput_Left()
		{
			_input.IsLeftPressed().Returns(true);

			_sut.CheckInput();

			_player.Received().MoveObject(Direction.Left, 1);
		}

		[Test]
		public void TestCheckInput_Right()
		{
			_input.IsRightPressed().Returns(true);

			_sut.CheckInput();

			_player.Received().MoveObject(Direction.Right, 1);
		}

		[Test]
		public void TestCheckInput_CycleActionLeft()
		{
			_input.IsCycleActionLeftPressed().Returns(true);
			_player.CurrentModule.Returns(_module);
			_player.CycleActionLeft().Returns(true);

			_sut.CheckInput();

			_player.Received().CycleActionLeft();
			_exchangeController.Received().UpdateExchangeControlsDisplay();
		}

		[Test]
		public void TestCheckInput_Action()
		{
			_input.IsActionPressed().Returns(true);
			_player.PrimaryAction().Returns(true);

			_sut.CheckInput();

			_player.Received().PrimaryAction();
			_exchangeController.Received().UpdateExchangeControlsDisplay();
			_exchangeController.Received().ClickOnButton("ExchangeControls", "CurrentAction");
		}

		[Test]
		public void TestCheckInput_CycleActionRight()
		{
			_input.IsCycleActionRightPressed().Returns(true);
			_player.CurrentModule.Returns(_module);
			_player.CycleActionRight().Returns(true);

			_sut.CheckInput();

			_player.Received().CycleActionRight();
			_exchangeController.Received().UpdateExchangeControlsDisplay();
		}

		[Test]
		public void TestCheckInput_CycleModuleLeft()
		{
			_input.IsCycleModuleLeftPressed().Returns(true);
			_player.CycleModuleLeft().Returns(true);

			_sut.CheckInput();

			_player.Received().CycleModuleLeft();
			_exchangeController.Received().UpdateExchangeControlsDisplay();
		}

		[Test]
		public void TestCheckInput_Module()
		{
			_input.IsModulePressed().Returns(true);
			_player.PrimaryModule().Returns(true);

			_sut.CheckInput();

			_player.Received().PrimaryModule();
			_exchangeController.Received().UpdateExchangeControlsDisplay();
			_exchangeController.Received().ClickOnButton("ExchangeControls", "CurrentModule");
		}

		[Test]
		public void TestCheckInput_CycleModuleRight()
		{
			_input.IsCycleModuleRightPressed().Returns(true);
			_player.CycleModuleRight().Returns(true);

			_sut.CheckInput();

			_player.Received().CycleModuleRight();
			_exchangeController.Received().UpdateExchangeControlsDisplay();
		}

		[Test]
		public void TestCheckInput_Pause()
		{
			_input.IsPausePressed().Returns(true);
			_exchangeController.ExchangeState.Returns(ExchangeState.Battle);

			_sut.CheckInput();

			_exchangeController.Received().ChangeStateToPause();
		}
	}
}
