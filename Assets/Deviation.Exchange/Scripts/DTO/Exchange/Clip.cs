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
	public interface IClip
	{
		Dictionary<IExchangeAction, int> Actions { get; }
		int Remaining { get; }
		float TimeRemaining { get; }
		bool Ready { get; }
		IExchangeAction Peek();
		IExchangeAction Pop();
		void Add(IExchangeAction action, int count);
		void Reset();
		BsonDocument ToBsonDocument();
	}

	public class Clip : SerializablePacket, IClip
	{
		public const int MAXACTIONCOUNT = 10;
		public const int MAXACTIONTYPECOUNT = 5;

		public Dictionary<IExchangeAction, int> Actions { get; private set; }
		public int Remaining => _remainingActions.Count;
		public float TimeRemaining { get; }
		public bool Ready { get { return _timer.TimerUp; } }

		private Stack<IExchangeAction> _remainingActions;
		private RNGCryptoServiceProvider _provider = new RNGCryptoServiceProvider();
		private IUnityTimer2 _timer = UnityTimer.Get;

		public Clip(){}

		public Clip(Dictionary<IExchangeAction, int> actions)
		{
			Actions = actions;
		}

		public Clip(Dictionary<Guid, int> actionGuids)
		{
			foreach (var action in actionGuids)
			{
				Add(action.Key, action.Value);
			}
		}

		public Clip(Dictionary<string, int> actionNames)
		{
			foreach (var action in actionNames)
			{
				Add(action.Key, action.Value);
			}
		}

		public Clip(List<IExchangeAction> actions)
		{
			foreach (var action in actions)
			{
				Add(action, 1);
			}
		}

		public Clip(List<Guid> actions)
		{
			foreach (var action in actions)
			{
				Add(action, 1);
			}
		}

		public Clip(List<string> actions)
		{
			foreach (var action in actions)
			{
				Add(action, 1);
			}
		}

		public IExchangeAction Peek()
		{
			return _remainingActions.Peek();
		}

		public IExchangeAction Pop()
		{
			if (Remaining > 0)
			{
				if (Ready)
				{
					_timer.Restart();
					_timer.Cooldown = Peek().Cooldown;
					return _remainingActions.Pop();
				}
				else
				{
					throw new ClipException($"An action was used recently. Wait {_timer.Remaining} before trying again.");
				}
			}
			else
			{
				throw new ClipException($"There are no more remaining actions in this clip.");
			}
		}

		public void Add(Guid action, int count)
		{
			Add(ActionLibrary.GetActionInstance(action), 1);
		}

		public void Add(string action, int count)
		{
			Add(ActionLibrary.GetActionInstance(action), 1);
		}

		public void Add(IExchangeAction action, int count)
		{
			if (Actions.Count() + count <= MAXACTIONCOUNT)
			{
				if (Actions.ContainsKey(action) && Actions[action] + count <= MAXACTIONTYPECOUNT)
				{
					if (Actions == null)
					{
						_remainingActions = new Stack<IExchangeAction>();
						Actions = new Dictionary<IExchangeAction, int>();
					}
					
					Actions.Add(action, count);
				}
				else
				{
					throw new ClipException($"Cannot add more than {MAXACTIONTYPECOUNT} actions of type {action.Name} to Clip.");
				}
			}
			else
			{
				throw new ClipException($"Cannot add more than {MAXACTIONCOUNT} actions to Clip.");
			}
		}

		public void Reset()
		{
			List<IExchangeAction> actions = new List<IExchangeAction>();

			foreach (var action in Actions)
			{
				for (int i = 0; i < action.Value; i++)
				{
					actions.Add(action.Key);
				}
			}

			_remainingActions = new Stack<IExchangeAction>(Shuffle(actions));
			_timer.Restart(true);
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

		public override void ToBinaryWriter(EndianBinaryWriter writer)
		{
			writer.Write(Actions.Keys.Count);
			foreach (var action in Actions)
			{
				writer.Write(action.Key.Name);
				writer.Write(action.Value);
			}
		}

		public override void FromBinaryReader(EndianBinaryReader reader)
		{
			int actionCount = reader.ReadInt32();
			for (int i = 0; i < actionCount; i++)
			{
				string name = reader.ReadString();
				int count = reader.ReadInt32();
				Add(name, count);
			}
		}

		public Clip(BsonDocument document) : this(JsonUtility.FromJson<Dictionary<string, int>>(document["Actions"]))
		{

		}

		public BsonDocument ToBsonDocument()
		{
			Dictionary<string, int> actions = new Dictionary<string, int>();

			foreach (var action in Actions)
			{
				actions.Add(action.Key.Name, action.Value);
			}

			var retVal = new BsonDocument();

			retVal.Add("Actions", JsonUtility.ToJson(actions));

			return retVal;
		}
	}

	public class ClipException : Exception
	{
		public ClipException()
		{
		}

		public ClipException(string message)
			: base(message)
		{
		}

		public ClipException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}
}
