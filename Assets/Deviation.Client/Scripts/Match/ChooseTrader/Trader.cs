using Assets.Scripts.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Deviation.Client.Scripts
{
	public interface ITrader
	{
		string Name { get; set; }
		string Title { get; set; }
		TraderType Type { get; set; }
		string Description { get; set; }
		Guid Guid { get; set; }
	}

	public class Trader : ITrader
	{
		public string Name { get; set; }
		public string Title { get; set; }
		public TraderType Type { get; set; }
		public string Description { get; set; }
		public Guid Guid { get; set; }

		public Trader(string name, string title, TraderType type, string description, Guid guid)
		{
			Name = name;
			Title = title;
			Type = type;
			Description = description;
			Guid = guid;
		}
	}
}
