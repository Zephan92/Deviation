using Assets.Scripts.Exchange.NPC;
using Assets.Scripts.Interface.Exchange;
using NUnit.Framework;

namespace Assets.Editor.Exchange
{
	[TestFixture]
	public class NPCControllerTests
	{
		INPCController _sut;

		[SetUp]
		public void SetUp()
		{
			_sut = new NPCController();
		}

		[Test]
		public void TestStartDecisionMaker()
		{
			_sut.StartDecisionMaker();
		}

		[Test]
		public void TestPauseDecisionMaker()
		{
			_sut.PauseDecisionMaker();

		}

		[Test]
		public void TestUnpauseDecisionMaker()
		{
			_sut.UnpauseDecisionMaker();

		}

		[Test]
		public void TestStopDecisionMaker()
		{
			_sut.StopDecisionMaker();

		}

		[Test]
		public void TestMakeDecision()
		{
			_sut.MakeDecision();
		}
	}
}
