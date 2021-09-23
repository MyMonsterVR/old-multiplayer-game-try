using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.CookieDeveloper.MyMonsterVR {
	[CreateAssetMenu(fileName = "Unnamed Gun", menuName = "Gun")]
	public class Gun : ScriptableObject
	{

		public string Name;
		public float fireRate;
		public float bloom;
		public float recoil;
		public float kickback;
		public float aimSpeed;
		public GameObject prefab;

	}
}