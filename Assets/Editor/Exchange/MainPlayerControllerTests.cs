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
		public void TestCheckInput_Action_Q()
		{
			_input.IsAction_Q_Pressed().Returns(true);
			_player.DoAction(0).Returns(true);

			_sut.CheckInput();

			_player.Received().DoAction(0);
		}
		[Test]
		public void TestCheckInput_Action_W()
		{
			_input.IsAction_W_Pressed().Returns(true);
			_player.DoAction(1).Returns(true);

			_sut.CheckInput();

			_player.Received().DoAction(1);
		}
		[Test]
		public void TestCheckInput_Action_E()
		{
			_input.IsAction_E_Pressed().Returns(true);
			_player.DoAction(2).Returns(true);

			_sut.CheckInput();

			_player.Received().DoAction(2);
		}
		[Test]
		public void TestCheckInput_Action_R()
		{
			_input.IsAction_R_Pressed().Returns(true);
			_player.DoAction(3).Returns(true);

			_sut.CheckInput();

			_player.Received().DoAction(3);
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
