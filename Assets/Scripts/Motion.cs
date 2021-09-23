using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace Com.CookieDeveloper.MyMonsterVR
{
	public class Motion : MonoBehaviourPunCallbacks
	{
		#region Variables
		public float speed;
		public float sprintModifier;
		public float jumpForce;
		private Rigidbody rig;


		public Camera normalCam;
		public GameObject cameraParent;
		public Transform groundDetector;
		public LayerMask ground;

		private Vector3 inputVector;

		public KeyCode sprintKey = KeyCode.LeftShift;
		public KeyCode jumpKey = KeyCode.Space;
		#endregion

		#region Motion Code
		void Start()
		{
			cameraParent.SetActive(photonView.IsMine);

			if(!photonView.IsMine) gameObject.layer = 11;

			if(Camera.main) Camera.main.enabled = false;

			rig = GetComponent<Rigidbody>();
		}

		
		void Update()
		{
			if (!photonView.IsMine) return;

			float hmove = Input.GetAxisRaw("Horizontal");
			float vmove = Input.GetAxisRaw("Vertical");

			bool sprint = Input.GetKey(sprintKey);
			bool jump = Input.GetKeyDown(jumpKey);


			bool isGrounded = Physics.Raycast(groundDetector.position, Vector3.down, 0.1f, ground);
			bool isJumping = jump && isGrounded;
			bool isSprinting = sprint && vmove > 0 && !isJumping && isGrounded;

			if(isJumping)
			{
				rig.AddForce(Vector3.up * jumpForce * 100);
			}

			Vector3 dir = new Vector3(hmove, 0, vmove);
			dir.Normalize();

			float adjSpeed = speed;
			if (isSprinting) adjSpeed /= sprintModifier;

			Vector3 targetVelocity = transform.TransformDirection(dir) * adjSpeed * 100 * Time.fixedDeltaTime;
			targetVelocity.y = rig.velocity.y;
			inputVector = targetVelocity;


		}

		private void FixedUpdate()
		{
			rig.velocity = inputVector;
		}
		#endregion
	}
}
