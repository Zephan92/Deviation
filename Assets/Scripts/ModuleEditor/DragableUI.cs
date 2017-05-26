using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.ModuleEditor
{
	public class DragableUI : MonoBehaviour
	{
		Vector2 Offset;
		public SnapPoint[] SnapPoints;
		private Vector2 _oldPos;
		public void Awake()
		{
			SnapPoints = FindObjectsOfType<SnapPoint>();
		}

		public void BeginDrag()
		{
			Debug.Log("Begin called.");
			_oldPos = transform.position;
			Offset = new Vector2(transform.position.x,transform.position.y) - new Vector2( Input.mousePosition.x, Input.mousePosition.y);
		}

		public void OnDrag()
		{
			transform.position = new Vector3(Offset.x + Input.mousePosition.x, Offset.y + Input.mousePosition.y, 0);
			Debug.Log("OnDrag called.");
		}

		public void EndDrag()
		{
			Debug.Log("EndDrag called.");
			foreach (SnapPoint rect in SnapPoints)
			{
				if (rect.Area.Contains(Input.mousePosition))
				{
					float x = rect.Area.x + rect.Area.width / 2;
					float y = rect.Area.y + rect.Area.height / 2;
					Vector2 newPos = new Vector2(x,y);
					transform.position = newPos;
					return;
				}
			}
			transform.position = _oldPos;
		}
	}
}
