using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace Com.CookieDeveloper.MyMonsterVR
{
	public class Look : MonoBehaviourPunCallbacks
	{
		#region Variables

		public Transform player;
		public Transform playerArm;
		public Transform cams;
		public Transform weapon;

		public static bool cursorLocked = true;

		[Range(0.01f, 20f)]
		public float xSens;
		[Range(0.01f, 20f)]
		public float ySens;
		public float maxAngle = 75;

		private Quaternion camCenter;
		#endregion

		#region basic
		void Start()
		{
			camCenter = cams.localRotation;
		}

		void Update()
		{
			if (!photonView.IsMine) return;
			LockCursor();
			if (!cursorLocked)
				return;
			else
			{
				setY();
				setX();
			}
		}
		#endregion

		#region Actual Camera movement
		void setY()
		{
			float input = Input.GetAxis("Mouse Y") * ySens * 100 * Time.deltaTime;
			Quaternion adj = Quaternion.AngleAxis(input, -Vector3.right);
			Quaternion delta = cams.localRotation * adj;

			if (Quaternion.Angle(camCenter, delta) < maxAngle)
			{
				cams.localRotation = delta;
			}
			weapon.rotation = cams.rotation;
			//playerArm.rotation = cams.rotation;
		}

		void setX()
		{
			float input = Input.GetAxis("Mouse X") * xSens * 100 * Time.deltaTime;
			Quaternion adj = Quaternion.AngleAxis(input, Vector3.up);
			Quaternion delta = player.localRotation * adj;
			player.localRotation = delta;
		}
		#endregion

		#region mouse locker

		void LockCursor()
		{
			if (cursorLocked)
			{
				Cursor.lockState = CursorLockMode.Locked;
				Cursor.visible = false;
				player.GetComponent<Motion>().enabled = true;
				if (Input.GetKeyDown(KeyCode.Escape))
				{
					cursorLocked = false;
				}
			}
			else
			{
				Cursor.lockState = CursorLockMode.None;
				Cursor.visible = true;
				player.GetComponent<Motion>().enabled = false;
				if (Input.GetKeyDown(KeyCode.Escape))
				{
					cursorLocked = true;
				}
			}
		}
		#endregion
	}
}