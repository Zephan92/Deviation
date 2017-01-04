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
		public void CheckInput_NoInput()
		{
			_sut.CheckInput();

			_player.DidNotReceiveWithAnyArgs().MoveObject(Direction.None,1);
		}

		[Test]
		public void CheckInput_Up()
		{
			_input.IsUpPressed().Returns(true);

			_sut.CheckInput();

			_player.Received().MoveObject(Direction.Up, 1);
		}

		[Test]
		public void CheckInput_Down()
		{
			_input.IsDownPressed().Returns(true);

			_sut.CheckInput();

			_player.Received().MoveObject(Direction.Down, 1);
		}

		[Test]
		public void CheckInput_Left()
		{
			_input.IsLeftPressed().Returns(true);

			_sut.CheckInput();

			_player.Received().MoveObject(Direction.Left, 1);
		}

		[Test]
		public void CheckInput_Right()
		{
			_input.IsRightPressed().Returns(true);

			_sut.CheckInput();

			_player.Received().MoveObject(Direction.Right, 1);
		}

		[Test]
		public void CheckInput_CycleActionLeft()
		{
			_input.IsCycleActionLeftPressed().Returns(true);
			_player.CurrentModule.Returns(_module);

			_sut.CheckInput();

			_module.Received().CycleActionLeft();
			_exchangeController.Received().UpdateExchangeControlsDisplay();
		}

		[Test]
		public void CheckInput_Action()
		{
			_input.IsActionPressed().Returns(true);

			_sut.CheckInput();

			_player.Received().PrimaryAction();
			_exchangeController.Received().UpdateExchangeControlsDisplay();
			_exchangeController.Received().ClickOnButton("ExchangeControls", "CurrentAction");
		}

		[Test]
		public void CheckInput_CycleActionRight()
		{
			_input.IsCycleActionRightPressed().Returns(true);
			_player.CurrentModule.Returns(_module);

			_sut.CheckInput();

			_module.Received().CycleActionRight();
			_exchangeController.Received().UpdateExchangeControlsDisplay();
		}

		[Test]
		public void CheckInput_CycleModuleLeft()
		{
			_input.IsCycleModuleLeftPressed().Returns(true);

			_sut.CheckInput();

			_player.Received().CycleModuleLeft();
			_exchangeController.Received().UpdateExchangeControlsDisplay();
		}

		[Test]
		public void CheckInput_Module()
		{
			_input.IsModulePressed().Returns(true);

			_sut.CheckInput();

			_player.Received().PrimaryModule();
			_exchangeController.Received().UpdateExchangeControlsDisplay();
			_exchangeController.Received().ClickOnButton("ExchangeControls", "CurrentModule");
		}

		[Test]
		public void CheckInput_CycleModuleRight()
		{
			_input.IsCycleModuleRightPressed().Returns(true);

			_sut.CheckInput();

			_player.Received().CycleModuleRight();
			_exchangeController.Received().UpdateExchangeControlsDisplay();
		}

		[Test]
		public void CheckInput_Pause()
		{
			_input.IsPausePressed().Returns(true);
			_exchangeController.ExchangeState.Returns(ExchangeState.Battle);

			_sut.CheckInput();

			_exchangeController.Received().ChangeStateToPause();
		}
	}
}
