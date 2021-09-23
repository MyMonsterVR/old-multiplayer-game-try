using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace Com.CookieDeveloper.MyMonsterVR
{
	public class Weapon : MonoBehaviourPunCallbacks
	{
		public Gun[] loadout;
		public Transform weaponParent;
		public GameObject bulletholePrefab;
		public LayerMask canBeShot;

		private Look cam;

		public int currentIndex;

		private float currentCooldown;

		private GameObject currentWeapon;

		float oldXSens;
		float oldYSens;

		bool ifModified = false;

		void Start()
		{
			cam = this.GetComponent<Look>();
		}

		void Update()
		{
			if (photonView.IsMine && Input.GetKeyDown(KeyCode.Alpha1)) { photonView.RPC("Equip", RpcTarget.AllViaServer, 0); }

			if (currentWeapon != null && photonView.IsMine)
			{
				Aim(Input.GetMouseButton(1));

				if (Input.GetMouseButtonDown(0) && currentCooldown <= 0f) {
					photonView.RPC("Shoot", RpcTarget.AllViaServer);
				}

				if (currentCooldown > 0) currentCooldown -= Time.deltaTime;

				currentWeapon.transform.localPosition = Vector3.Lerp(currentWeapon.transform.localPosition, Vector3.zero, Time.deltaTime);
			}
		}

		[PunRPC]
		void Equip(int ind)
		{
			currentIndex = ind;

			if (currentWeapon != null) Destroy(currentWeapon);

			currentIndex = ind;

			GameObject newWeapon = Instantiate(loadout[ind].prefab, weaponParent.position, weaponParent.rotation, weaponParent);
			newWeapon.transform.localPosition = Vector3.zero;
			newWeapon.transform.localEulerAngles = Vector3.zero;

			currentWeapon = newWeapon;
		}
		private bool once;
		void Aim(bool isAiming)
		{
			Transform anchor = currentWeapon.transform.Find("Anchor");
			Transform ads = currentWeapon.transform.Find("States/ADS");
			Transform hip = currentWeapon.transform.Find("States/HIP");

			if(isAiming)
			{
				anchor.position = Vector3.Lerp(anchor.position, ads.position, Time.deltaTime * loadout[currentIndex].aimSpeed);

				ifModified = true;

				if (!once)
				{
					once = true;
					cam.ySens /= 2;
					cam.xSens /= 2;
					oldXSens = cam.xSens;
					oldYSens = cam.ySens;
				}
				else
				{
					return;
				}
			}
			else
			{
				if (ifModified)
				{
					cam.xSens = oldXSens;
					cam.ySens = oldYSens;

					Debug.Log("new cam speed: " + cam.xSens + " " + cam.ySens);

					once = false;
				}
				else
				{
					cam.xSens = 10;
					cam.ySens = 10;
				}
				anchor.position = Vector3.Lerp(anchor.position, hip.position, Time.deltaTime * loadout[currentIndex].aimSpeed);
			}
		}

		#region Shoot
		[PunRPC]
		void Shoot()
		{
			Transform spawn = transform.Find("Cameras/Normal Camera");

			//bloom

			Vector3 bloom = spawn.position + spawn.forward * 1000f;
			bloom += Random.Range(-loadout[currentIndex].bloom, loadout[currentIndex].bloom) * spawn.up;
			bloom += Random.Range(-loadout[currentIndex].bloom, loadout[currentIndex].bloom) * spawn.right;
			bloom -= spawn.position;
			bloom.Normalize();

			//COOLDOWN
			currentCooldown = loadout[currentIndex].fireRate;


			//Shooting
			RaycastHit hit = new RaycastHit();
			if (Physics.Raycast(spawn.position, bloom, out hit, 1000f, canBeShot))
			{
				GameObject newHole = PhotonNetwork.Instantiate("Bullet Hole", hit.point + hit.normal * 0.001f, Quaternion.identity, 0);
				newHole.transform.LookAt(hit.point + hit.normal);
				PhotonNetwork.Destroy(newHole);

				if(photonView.IsMine)
				{
					//Shooting other player
					if(hit.collider.gameObject.layer == 11)
					{
						//RPC CALL TO DAMAGE
					}
				}
			}

			currentWeapon.transform.Rotate(-loadout[currentIndex].recoil, 0, 0);
			currentWeapon.transform.position -= currentWeapon.transform.forward * loadout[currentIndex].kickback;
		}
	}
	#endregion
}