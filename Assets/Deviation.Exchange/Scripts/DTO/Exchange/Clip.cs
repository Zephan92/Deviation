using Assets.Scripts.DTO.Exchange;
using Assets.Scripts.Interface;
using Assets.Scripts.Interface.DTO;
using Assets.Scripts.Library;
using Barebones.Networking;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Deviation.Exchange.Scripts.DTO.Exchange
{
	public interface IClip : IKitComponent
	{
		int Remaining { get; }
		Dictionary<IExchangeAction, int> Actions { get; }
		IExchangeAction Peek();
		IExchangeAction Pop();
		void Add(IExchangeAction action, int count);
		BsonDocument ToBsonDocument();
	}

	public class Clip : SerializablePacket, IClip
	{
		public const int MAXACTIONCOUNT = 10;
		public const int MAXACTIONTYPECOUNT = 5;

		public Dictionary<IExchangeAction, int> Actions { get; private set; }
		public int Remaining => _remainingActions.Count;
		public float TimeRemaining { get { return _timer.Remaining; } }
		public bool Ready { get { return _timer.TimerUp; } }

		private IExchangePlayer _player;
		public IExchangePlayer Player
		{
			get { return _player; }
			set
			{
				_player = value;
				foreach (IExchangeAction action in _remainingActions)
				{
					action.Player = value;
				}
			}
		}

		private Stack<IExchangeAction> _remainingActions;
		private RNGCryptoServiceProvider _provider = new RNGCryptoServiceProvider();
		private IUnityTimer2 _timer = UnityTimer.Get;

		public Clip()
		{
			Actions = new Dictionary<IExchangeAction, int>();
			_remainingActions = new Stack<IExchangeAction>();
		}

		public Clip(Dictionary<IExchangeAction, int> actions)
		{
			Actions = actions;
		}

		public Clip(Dictionary<Guid, int> actionGuids) 
		{
			Actions = new Dictionary<IExchangeAction, int>();
			_remainingActions = new Stack<IExchangeAction>();

			foreach (var action in actionGuids)
			{
				Add(action.Key, action.Value);
			}
		}

		public Clip(Dictionary<string, int> actionNames)
		{
			Actions = new Dictionary<IExchangeAction, int>();
			_remainingActions = new Stack<IExchangeAction>();

			foreach (var action in actionNames)
			{
				Add(action.Key, action.Value);
			}
		}

		public Clip(List<IExchangeAction> actions)
		{
			Actions = new Dictionary<IExchangeAction, int>();
			_remainingActions = new Stack<IExchangeAction>();

			foreach (var action in actions)
			{
				Add(action, 1);
			}
		}

		public Clip(List<Guid> actions)
		{
			Actions = new Dictionary<IExchangeAction, int>();
			_remainingActions = new Stack<IExchangeAction>();

			foreach (var action in actions)
			{
				Add(action, 1);
			}
		}

		public Clip(List<string> actions)
		{
			Actions = new Dictionary<IExchangeAction, int>();
			_remainingActions = new Stack<IExchangeAction>();

			foreach (var action in actions)
			{
				Add(action, 1);
			}
		}

		public Clip(BsonDocument document)
		{
			Actions = new Dictionary<IExchangeAction, int>();
			_remainingActions = new Stack<IExchangeAction>();

			string[] actions = document["Actions"].AsArray.Select(x => x.AsString).ToArray();
			foreach (string action in actions)
			{
				Add(action, 1);
			}
		}

		public IExchangeAction Peek()
		{
			if (Remaining > 0)
			{
				return _remainingActions.Peek();
			}
			else
			{
				throw new ClipException($"There are no more remaining actions in this clip.");
			}
		}

		public IExchangeAction Pop()
		{
			if (Remaining > 0)
			{
				if (Ready)
				{
					return _remainingActions.Pop();
				}
				else
				{
					throw new ClipException($"Clip was used recently. Wait {_timer.Remaining} before trying again.");
				}
			}
			else
			{
				throw new ClipException($"There are no more remaining actions in this clip.");
			}
		}

		public void StartCooldown()
		{
			if (Remaining > 0)
			{
				if (Ready)
				{
					_timer.Restart();
					_timer.Cooldown = Peek().Cooldown;
				}
				else
				{
					throw new ClipException($"Clip was used recently. Wait {_timer.Remaining} before trying again.");
				}
			}
			else
			{
				throw new ClipException($"There are no more remaining actions in this clip.");
			}
		}

		public void Add(Guid action, int count){ Add(ActionLibrary.GetActionInstance(action), count); }
		public void Add(string action, int count){ Add(ActionLibrary.GetActionInstance(action), count); }
		public void Add(IExchangeAction action, int count)
		{
			if (Actions.Count() + count > MAXACTIONCOUNT)
			{
				throw new ClipException($"Cannot add more than {MAXACTIONCOUNT} actions to Clip. Action Name: {action.Name}, Count: {count}.");
			}
			else if (count > MAXACTIONTYPECOUNT)
			{
				throw new ClipException($"Cannot add more than {MAXACTIONTYPECOUNT} actions of type {action.Name} to Clip. Action Count: {count}.");
			}
			else if (Actions.ContainsKey(action))
			{
				if (Actions[action] + count > MAXACTIONTYPECOUNT)
				{
					throw new ClipException($"Cannot add more than {MAXACTIONTYPECOUNT} actions of type {action.Name} to Clip. Action Count: {count}.");
				}
				else
				{
					Actions[action] += count;
				}
			}
			else
			{
				Actions.Add(action, count);
			}
		}

		public void Reset()
		{
			List<IExchangeAction> actions = new List<IExchangeAction>();

			try
			{
				foreach (var action in Actions)
				{
					for (int i = 0; i < action.Value; i++)
					{
						actions.Add(action.Key);
					}
				}

				_remainingActions = new Stack<IExchangeAction>(Shuffle(actions));
				foreach (IExchangeAction action in _remainingActions)
				{
					action.Player = Player;
				}
				_timer.Restart(true);
			}
			catch(Exception ex)
			{
				throw new ClipException($"Failed to Reset Clip.", ex);
			}
		}

		public override void ToBinaryWriter(EndianBinaryWriter writer)
		{
			try
			{
				writer.Write(Actions.Keys.Count);
				foreach (var action in Actions)
				{
					writer.Write(action.Key.Name);
					writer.Write(action.Value);
				}
			}
			catch(Exception ex)
			{
				throw new ClipException($"Failed to serialize Clip.", ex);
			}
		}

		public override void FromBinaryReader(EndianBinaryReader reader)
		{
			try
			{
				int actionCount = reader.ReadInt32();
				for (int i = 0; i < actionCount; i++)
				{
					string name = reader.ReadString();
					int count = reader.ReadInt32();
					Add(name, count);
				}
			}
			catch (Exception ex)
			{
				throw new ClipException($"Failed to deserialize Clip.", ex);
			}
		}

		public BsonDocument ToBsonDocument()
		{
			var retVal = new BsonDocument();
			var actions = new List<BsonValue>();

			try
			{
				foreach (var action in Actions)
				{
					for (int i = 0; i < action.Value; i++)
					{
						actions.Add(action.Key.Name);
					}
				}

				retVal.Add("Actions", new BsonArray(actions.ToArray()));
			}
			catch (Exception ex)
			{
				throw new ClipException($"Failed to serialize Clip.", ex);
			}

			return retVal;
		}

		public override string ToString()
		{
			string retVal = $"Clip";
			foreach (var action in Actions)
			{
				retVal += $"\n{action.Key.Name}: {action.Value}";
			}

			return retVal;
		}

		private List<IExchangeAction> Shuffle(List<IExchangeAction> list)
		{
			int n = list.Count;
			while (n > 1)
			{
				byte[] box = new byte[1];
				do _provider.GetBytes(box);
				while (!(box[0] < n * (Byte.MaxValue / n)));
				int k = (box[0] % n);
				n--;
				IExchangeAction value = list[k];
				list[k] = list[n];
				list[n] = value;
			}
			return list;
		}
	}

	public class ClipException : Exception
	{
		public ClipException(){}
		public ClipException(string message) : base(message){}
		public ClipException(string message, Exception inner) : base(message, inner){}
	}
}
