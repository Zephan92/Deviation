using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Deviation.Client.Scripts.UserInterface
{
	public class LayoutGroupFactory
	{
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
