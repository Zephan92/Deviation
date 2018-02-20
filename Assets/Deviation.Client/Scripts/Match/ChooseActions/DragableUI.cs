using Assets.Scripts.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.ModuleEditor
{
	public class DragableUI : MonoBehaviour
	{
		Vector2 Offset;
		public SnapPoint[] SnapPoints;
		private Vector2 _oldPos;
		private SnapPoint _currentSnapPoint;

		private Transform _origParent;
		private Transform _parentWhileDrag;

		public void Awake()
		{
			_origParent = gameObject.transform.parent;
		}

		public void Start()
		{
			SnapPoints = FindObjectsOfType<SnapPoint>();
			_currentSnapPoint = gameObject.GetComponent<SnapPoint>();			
		}

		public void BeginDrag()
		{
			_oldPos = transform.position;
			transform.SetParent(transform.root);

			Offset = new Vector2(transform.position.x,transform.position.y) - new Vector2( Input.mousePosition.x, Input.mousePosition.y);
		}

		public void OnDrag()
		{
			transform.position = new Vector3(Offset.x + Input.mousePosition.x, Offset.y + Input.mousePosition.y, 0);
		}

		public void EndDrag()
		{
			foreach (SnapPoint snap in SnapPoints)
			{
				if (snap.Area.Contains(Input.mousePosition) && !snap.IsOccupied)
				{
					float x = snap.Area.x + snap.Area.width / 2;
					float y = snap.Area.y + snap.Area.height / 2;
					Vector2 newPos = new Vector2(x,y);
					transform.position = newPos;
					transform.SetParent(snap.transform, true);
					//_currentSnapPoint.IsOccupied = false;//todo
					//_currentSnapPoint = snap;
					//_currentSnapPoint.IsOccupied = true;
					return;
				}
			}

			transform.SetParent(_origParent);
			transform.position = _oldPos;
		}
	}
}
