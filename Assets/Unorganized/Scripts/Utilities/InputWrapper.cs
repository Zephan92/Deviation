using System;
using Assets.Scripts.Interface;
using UnityEngine;

namespace Assets.Scripts.Utilities
{
	public class InputWrapper : IInput
	{
		public bool GetKeyDown(KeyCode key)
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

		public bool GetKey(KeyCode key)
		{
			switch (key)
			{
				case KeyCode.Escape:
					return Input.GetKey(KeyCode.Escape);
				case KeyCode.UpArrow:
					return Input.GetKey(KeyCode.UpArrow);
				case KeyCode.DownArrow:
					return Input.GetKey(KeyCode.DownArrow);
				case KeyCode.LeftArrow:
					return Input.GetKey(KeyCode.LeftArrow);
				case KeyCode.RightArrow:
					return Input.GetKey(KeyCode.RightArrow);
				case KeyCode.Q:
					return Input.GetKey(KeyCode.Q);
				case KeyCode.W:
					return Input.GetKey(KeyCode.W);
				case KeyCode.E:
					return Input.GetKey(KeyCode.E);
				case KeyCode.R:
					return Input.GetKey(KeyCode.R);
				default:
					return false;
			}
		}
	}
}
