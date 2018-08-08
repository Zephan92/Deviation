using Assets.Scripts.DTO.Exchange;
using Assets.Scripts.Interface;
using Assets.Scripts.Interface.DTO;
using Assets.Scripts.Library;
using Barebones.Networking;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Deviation.Exchange.Scripts.DTO.Exchange
{
	public interface IBasicAction : IKitComponent
	{
		IExchangeAction Action { get; }
	}

	public class BasicAction : SerializablePacket, IBasicAction
	{
		public float TimeRemaining { get { return _timer.Remaining; } }
		public bool Ready { get { return _timer.TimerUp; } }
		public IExchangeAction Action { get; private set; }

		private IExchangePlayer _player;
		public IExchangePlayer Player
		{
			get { return _player; }
			set
			{
				_player = value;
				Action.Player = value;
			}
		}

		private IUnityTimer2 _timer = UnityTimer.Get;

		public BasicAction(){}
		public BasicAction(Guid guid) : this(ActionLibrary.GetActionInstance(guid)){}
		public BasicAction(string name) : this(ActionLibrary.GetActionInstance(name)){}
		public BasicAction(IExchangeAction action)
		{
			Action = action;
			_timer.Cooldown = action.Cooldown;
		}

		public BasicAction(BsonDocument document)
		{
			Action = ActionLibrary.GetActionInstance(document["Action"].AsString);
		}

		public BsonDocument ToBsonDocument()
		{//add try catch
			var retVal = new BsonDocument();

			retVal.Add("Action", Action.Name);

			return retVal;
		}

		public void StartCooldown()
		{
			if (Ready)
			{
				_timer.Restart();
				_timer.Cooldown = Action.Cooldown;
			}
			else
			{
				throw new ClipException($"Basic Action was used recently. Wait {_timer.Remaining} before trying again.");
			}
		}

		public void Reset()
		{
			Action.Player = Player;
			_timer.Restart(true);
		}

		public override void ToBinaryWriter(EndianBinaryWriter writer)
		{
			try
			{
				writer.Write(Action.Name);
			}
			catch (Exception ex)
			{
				throw new ClipException($"Failed to serialize BasicAction.", ex);
			}
		}

		public override void FromBinaryReader(EndianBinaryReader reader)
		{
			try
			{
				Action = ActionLibrary.GetActionInstance(reader.ReadString());
			}
			catch (Exception ex)
			{
				throw new ClipException($"Failed to deserialize BasicAction.", ex);
			}
		}

		public override string ToString()
		{
			string retVal = $"BasicAction" +
					$"\nAction: {Action}";
			return retVal;
		}
	}
}
