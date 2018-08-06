using Assets.Scripts.Enum;
using Assets.Scripts.Interface;
using Assets.Scripts.Interface.DTO;
using Assets.Scripts.Interface.Exchange;
using Assets.Scripts.Library;
using System;
using UnityEngine;

namespace Assets.Scripts.DTO.Exchange
{
	public class ExchangeAction : IExchangeAction
	{
		public Guid Id { get; set; }

		//Action Name
		public string Name { get; set; }

		//Action Description
		public string Description { get; set; }

		//Action Attack: Used for calculating health/energy
		public IAttack Attack { get; set; }

		//This is used the ui texture for the action in the 
		public Texture2D ActionTexture { get; set; }

		//This is how long after an action is used before it can be used again
		public float Cooldown { get; set; }

		public IKit ParentKit { get; set; }

		public string PrimaryActionName { get; set; }

		//this is the primary action method run when this action is used
		public System.Action<IBattlefieldController, IAttack, IExchangePlayer, BattlefieldZone> PrimaryAction;

		public TraderType Type { get; set; }

		public ExchangeAction(Guid id, string name, IAttack attack, Texture2D actionTexture, string primaryActionName, float cooldown, TraderType type)
		{
			Id = id;
			Name = name;
			Attack = attack;
			Cooldown = cooldown;
			ActionTexture = actionTexture;
			PrimaryActionName = primaryActionName;
			Type = type;

			if(ActionMethodLibrary.ContainsActionMethod(primaryActionName))
			{
				//based on the action name, go find the relevant method in the Action Method Library
				PrimaryAction = ActionMethodLibrary.GetActionMethod(primaryActionName);
			}
			else
			{
				//otherwise return a generic method and throw an error
				PrimaryAction = ActionMethodLibrary.GetActionMethod("default");
				Debug.LogError(name + " - Action: The \"" + primaryActionName + "\" Action Method was not in the ActionMethod Dictionary");
			}
		}

		//when this method is called, it runs the primary action and passes in the Attack and Battlefield Controller for use in that method
		public void InitiateAttack(IBattlefieldController bc, BattlefieldZone zone)
		{
			PrimaryAction(bc, Attack, ParentKit.Player, zone);
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			IExchangeAction action = (IExchangeAction)obj;
			return	Id == action.Id &&
					Name == action.Name &&
					Attack == action.Attack &&
					Cooldown == action.Cooldown &&
					ActionTexture == action.ActionTexture &&
					PrimaryActionName == action.PrimaryActionName &&
					Type == action.Type;
		}

	}
}
