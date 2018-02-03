using Assets.Deviation.Client.Scripts.Match;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets.Deviation.Client.Scripts
{
	public enum TraderDisplaySide
	{
		LeftSide,
		RightSide,
	}

	public enum TraderType
	{
		Alpha = 0,
		Beta = 1,
		Gamma = 2,
		Delta = 3,
		Epsilon = 4
	}

	public class TraderPanelFactory : MonoBehaviour
	{
		public static GameObject CreateTraderPanel(TraderDisplaySide side, ITrader trader, GameObject parent, UnityAction onClickAction)
		{
			string resource;
			if (side == TraderDisplaySide.LeftSide)
			{
				resource = "TraderPanelLeft";
			}
			else
			{
				resource = "TraderPanelRight";
			}
			var traderPanel = Instantiate(Resources.Load(resource), parent.transform) as GameObject;
			var traderDetailsObject = traderPanel.GetComponent<TraderDetailsPanel>();
			traderDetailsObject.UpdateTraderDetails(trader, onClickAction);
			return traderPanel;
		}

		public static GameObject CreatePanel(string name, float width, float height, Color color, GameObject parent)
		{
			GameObject panel = new GameObject(name);
			panel.transform.SetParent(parent.transform, false);
			panel.AddComponent<CanvasRenderer>();
			Image i = panel.AddComponent<Image>();
			i.color = color;
			panel.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
			return panel;
		}

		private static Text CreateText(string textName, string textString, float width, float height, int fontSize, GameObject parent)
		{
			GameObject textGO = new GameObject(textName);
			Text text = textGO.AddComponent<Text>();
			textGO.transform.SetParent(parent.transform, false);
			text.text = textString;
			text.font = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
			text.alignment = TextAnchor.MiddleLeft;
			text.fontSize = fontSize;
			text.color = Color.black;
			textGO.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
			return text;
		}

		public static HorizontalLayoutGroup CreateHorizontalLayout(GameObject go)
		{
			var layout = go.AddComponent<HorizontalLayoutGroup>();
			layout.childControlWidth = false;
			layout.childControlHeight = true;
			layout.childForceExpandWidth = false;
			layout.childForceExpandHeight = true;
			return layout;
		}

		public static VerticalLayoutGroup CreateVerticalLayout(GameObject go)
		{
			var layout = go.AddComponent<VerticalLayoutGroup>();
			layout.childControlWidth = true;
			layout.childControlHeight = false;
			layout.childForceExpandWidth = true;
			layout.childForceExpandHeight = false;
			return layout;
		}
	}
}
