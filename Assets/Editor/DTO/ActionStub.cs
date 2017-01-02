using Assets.Scripts.Interface.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Interface;
using UnityEngine;
using Assets.Scripts.Interface.Exchange;

namespace Assets.Editor.DTO
{
	public class ActionStub : IAction
	{
		public bool InitiateAttackCalled = false;

		public ActionStub(string name, IAttack attack, Color attackTexture, string primaryActionName, float cooldown)
		{
			Name = name;
			Attack = attack;
			Cooldown = cooldown;
			ActionTexture = attackTexture;
			PrimaryActionName = primaryActionName;
		}

		public Color ActionTexture
		{ get; set; }

		public IAttack Attack
		{ get; set; }

		public float Cooldown
		{ get; set; }

		public string Name
		{ get; set; }

		public IModule ParentModule
		{ get; set; }

		public IPlayer Player
		{ get; set; }

		public string PrimaryActionName
		{ get; set; }


		public IAction GetLeftAction()
		{
			return this;
		}

		public IAction GetRightAction()
		{
			return this;
		}

		public void InitiateAttack(IBattlefieldController bc)
		{
			InitiateAttackCalled = true;
		}
	}
}
