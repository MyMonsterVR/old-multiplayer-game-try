using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace Com.CookieDeveloper.MyMonsterVR
{
	public class Manager : MonoBehaviour
	{
		public string player_prefab;

		public Transform spawnPos;

		private void Start()
		{
			Spawn();
		}

		public void Spawn()
		{
			PhotonNetwork.Instantiate(player_prefab, spawnPos.position, spawnPos.rotation);
		}
	}
}
