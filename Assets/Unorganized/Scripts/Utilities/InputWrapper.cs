using System;
using Assets.Scripts.Interface;
using UnityEngine;

namespace Assets.Scripts.Utilities
{
	public class InputWrapper : IInput
	{
		public bool IsKeyPressed(KeyCode key)
		{
			switch (key)
			{
				case KeyCode.Escape:
					return Input.GetKeyDown(KeyCode.Escape);
				case KeyCode.UpArrow:
					return Input.GetKeyDown(KeyCode.UpArrow);
				case KeyCode.DownArrow:
					return Input.GetKeyDown(KeyCode.DownArrow);
				case KeyCode.LeftArrow:
					return Input.GetKeyDown(KeyCode.LeftArrow);
				case KeyCode.RightArrow:
					return Input.GetKeyDown(KeyCode.RightArrow);
				case KeyCode.Q:
					return Input.GetKeyDown(KeyCode.Q);
				case KeyCode.W:
					return Input.GetKeyDown(KeyCode.W);
				case KeyCode.E:
					return Input.GetKeyDown(KeyCode.E);
				case KeyCode.R:
					return Input.GetKeyDown(KeyCode.R);
				default:
					return false;
			}
		}

		public bool IsPausePressed()
		{
			return Input.GetKeyDown(KeyCode.Escape);
		}

		public bool IsUpPressed()
		{
			return Input.GetKeyDown(KeyCode.UpArrow);
		}

		public bool IsDownPressed()
		{
			return Input.GetKeyDown(KeyCode.DownArrow);
		}

		public bool IsLeftPressed()
		{
			return Input.GetKeyDown(KeyCode.LeftArrow);
		}

		public bool IsRightPressed()
		{
			return Input.GetKeyDown(KeyCode.RightArrow);
		}

		public bool IsAction_Q_Pressed()
		{
			return Input.GetKeyDown(KeyCode.Q);
		}

		public bool IsAction_W_Pressed()
		{
			return Input.GetKeyDown(KeyCode.W);
		}

		public bool IsAction_E_Pressed()
		{
			return Input.GetKeyDown(KeyCode.E);
		}

		public bool IsAction_R_Pressed()
		{
			return Input.GetKeyDown(KeyCode.R);
		}
	}
}
