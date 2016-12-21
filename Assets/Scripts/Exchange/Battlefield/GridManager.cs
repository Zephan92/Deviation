using Assets.Scripts.Controllers;
using Assets.Scripts.Enum;
using UnityEngine;

namespace Assets.Scripts.Exchange
{
	class GridManager : MonoBehaviour
	{
		public Battlefield ThisBattlefield = Battlefield.One;
		private Transform[] _gridSpheres;
		private BattlefieldController bc;

		void Awake()
		{
			_gridSpheres = GetComponentsInChildren<Transform>();

			if (bc == null)
			{
				var bcObject = GameObject.FindGameObjectWithTag("BattlefieldController");
				bc = bcObject.GetComponent<BattlefieldController>();
			}
		}

		void SetBattlefield(Battlefield battlefield)
		{
			ThisBattlefield = battlefield;
		}

		void Update()
		{
			RefreshBattlefield();
		}

		//on update, refreshes the grid color
		private void RefreshBattlefield()
		{
			foreach (Transform sphere in _gridSpheres)
			{
				if (sphere == _gridSpheres[0])
				{
					continue;
				}

				if (bc.GetBattlefieldState(ThisBattlefield, ConvertToArrayNumber((int)sphere.transform.localPosition.z), ConvertToArrayNumber((int)sphere.transform.localPosition.x)))
				{
					sphere.GetComponent<Renderer>().material.color = Color.red;
				}
				else
				{
					sphere.GetComponent<Renderer>().material.color = Color.white;
				}
			}
		}

		//move these to a utilities class
		private int ConvertToArrayNumber(int input)
		{
			return input + 2;
		}

		private int ConvertFromArrayNumber(int input)
		{
			return input - 2;
		}
	}
}
