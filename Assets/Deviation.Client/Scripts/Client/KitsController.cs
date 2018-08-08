using Assets.Deviation.Client.Scripts.Match;
using Assets.Deviation.Client.Scripts.UserInterface;
using Assets.Deviation.Exchange.Scripts.DTO.Exchange;
using Assets.Scripts.DTO.Exchange;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Deviation.Client.Scripts.Client
{
	public class KitsController : MonoBehaviour
	{
		public Transform MenuBar;
		public Transform Kits;
		public Transform Collection;
		public Transform EditKitUI;

		public VerticalScrollPanel KitList;

		public void Awake()
		{
			var parent = GameObject.Find("KitsUI");
			MenuBar = parent.transform.Find("Menu Bar");
			Kits = parent.transform.Find("Kits");
			Collection = parent.transform.Find("Collection");
			EditKitUI = parent.transform.Find("EditKitUI");
			KitList = GetComponentInChildren<VerticalScrollPanel>();
			List<IKit> kits = new List<IKit>();
			IKit kit = CreateStarterKit();
			kits.Add(kit);
			InitializeKitPanels(kits);
		}

		private IKit CreateStarterKit()
		{
			IBasicAction basicAction = new BasicAction("Small Projectile");
			var clip1Actions = new Dictionary<string, int>
			{
				{ "Small Projectile", 4 },
				//{ "Small Projectile", 2 },
				{ "Tremor", 2 },
				{ "ShockWave", 4 }
			};
			var clip2Actions = new Dictionary<string, int>
			{
				{ "Small Projectile", 4 },
				//{ "Small Projectile", 2 },
				{ "Drain", 2 },
				{ "Ambush", 4 }
			};
			var clip3Actions = new Dictionary<string, int>
			{
				{ "Small Projectile", 2 },
				{ "Medium Projectile", 2 },
				{ "Large Projectile", 2 },
				{ "Wall Push", 4 }
			};
			IClip[] clips = new IClip[] { new Clip(clip1Actions), new Clip(clip2Actions), new Clip(clip3Actions) };
			return new Kit("Starter Kit", clips, basicAction);
		}

		public void InitializeKitPanels(List<IKit> kits)
		{
			GameObject kitRow = new GameObject("KitRow");
			LayoutGroupFactory.CreateHorizontalLayout(kitRow);
			kitRow.GetComponent<RectTransform>().sizeDelta = new Vector2(1000, 100);
			kitRow.transform.SetParent(KitList.List.transform);
			Create_CreateKitButtonPanel(kitRow);

			for (int i = 1; i < kits.Count + 1; i++)
			{
				IKit kit = kits[i];
				if (i % 6 == 0 && i > 0)
				{
					kitRow = new GameObject("KitRow");
					LayoutGroupFactory.CreateHorizontalLayout(kitRow);
					kitRow.GetComponent<RectTransform>().sizeDelta = new Vector2(1000, 100);
					kitRow.transform.SetParent(KitList.List.transform);
				}

				Create_KitPanel(kit, kitRow);
			}
		}

		public GameObject Create_CreateKitButtonPanel(GameObject parent)
		{
			var kitPanel = Instantiate(Resources.Load("CreateKitPanel"), parent.transform) as GameObject;
			var button = kitPanel.GetComponent<Button>();
			button.onClick.AddListener(() => { EditKitUI.gameObject.SetActive(true); });
			return kitPanel;
		}

		public GameObject Create_KitPanel(IKit kit, GameObject parent)
		{
			var kitPanel = Instantiate(Resources.Load("KitPanel"), parent.transform) as GameObject;
			var kitDetailsPanel = kitPanel.GetComponent<KitDetailsPanel>();
			kitDetailsPanel.UpdateKitDetails(kit);
			return kitPanel;
		}
	}
}
