using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Hunter {
	[AddComponentMenu("Hunter/Entities/PlayerControl")]
	[DisallowMultipleComponent]
	public class PlayerController : MonoBehaviour {

		Entity entity;

		[Header("Components")]
		public Aim aim;
		[HideInInspector]
		public bool isLockedMove = false;
		[HideInInspector]
		public CharacterController charCrtl;
		public bool isDied => entity.isDeath;
		public Animator animator => entity.animator;
		public Rigidbody rigid => entity.rigid;

		public Transform cam;

		public Vector3 direction {
			get {
				Vector3 dir = cam.forward;
				dir.y = 0;
				return dir.normalized;
			}
		}
		public float minDistanceOnGround = 0.2f;
		public bool isOnGround {
			get {
				RaycastHit hitF;
				RaycastHit hitB;
				RaycastHit hitR;
				RaycastHit hitL;
				RaycastHit hitС;
				Vector3 pointB = transform.position + new Vector3(charCrtl.center.x, charCrtl.center.y - charCrtl.height / 2, charCrtl.center.z-charCrtl.radius);
				Vector3 pointF = transform.position + new Vector3(charCrtl.center.x, charCrtl.center.y - charCrtl.height / 2, charCrtl.center.z + charCrtl.radius);
				Vector3 pointR = transform.position + new Vector3(charCrtl.center.x + charCrtl.radius, charCrtl.center.y - charCrtl.height / 2, charCrtl.center.z);
				Vector3 pointL = transform.position + new Vector3(charCrtl.center.x - charCrtl.radius, charCrtl.center.y - charCrtl.height / 2, charCrtl.center.z);
				Vector3 pointС = transform.position + new Vector3(charCrtl.center.x, charCrtl.center.y - charCrtl.height / 2, charCrtl.center.z);
				Ray rayB = new Ray(pointB, Vector3.down);
				Ray rayF = new Ray(pointF, Vector3.down);
				Ray rayL = new Ray(pointL, Vector3.down);
				Ray rayR = new Ray(pointR, Vector3.down);
				Ray rayС = new Ray(pointС, Vector3.down);
				Debug.DrawRay(pointB, Vector3.down * minDistanceOnGround, Color.red);
				Debug.DrawRay(pointF, Vector3.down * minDistanceOnGround, Color.red);
				Debug.DrawRay(pointR, Vector3.down * minDistanceOnGround, Color.red);
				Debug.DrawRay(pointL, Vector3.down * minDistanceOnGround, Color.red);
				Debug.DrawRay(pointС, Vector3.down * minDistanceOnGround, Color.red);
				if (Physics.Raycast(rayB, out hitB, minDistanceOnGround) ||
					Physics.Raycast(rayF, out hitF, minDistanceOnGround) ||
					Physics.Raycast(rayR, out hitR, minDistanceOnGround) ||
					Physics.Raycast(rayL, out hitL, minDistanceOnGround) ||
					Physics.Raycast(rayС, out hitС, minDistanceOnGround)) {
					return true;
				}
				
				return false;
			}
		}

		public float gravity;

		private float _timer;
		public float timeJump;

		void Start() {
			entity = GetComponent<Entity>();
			charCrtl = GetComponent<CharacterController>();
			if (cam == null) {
				cam = Camera.main.transform;
			}
		}


		void FixedUpdate() {
			animator.SetBool("onGround", isOnGround);
			if (isDied) return;

			if (isLockedMove) return;
			Move();
			Jump();
			Slash();
			TakeThingAndInventary();
			
		}

		void Move() {
			if (InputHelper.Walk) {
				entity.typeSpeed = (entity.typeSpeed == Entity.TypeSpeed.run) ? Entity.TypeSpeed.walk : Entity.TypeSpeed.run;
			}
			if (entity.typeSpeed != Entity.TypeSpeed.walk) {
				if (InputHelper.Sprint) {
					entity.typeSpeed = Entity.TypeSpeed.sprint;
				} else {
					entity.typeSpeed = Entity.TypeSpeed.run;
				}
			}
			
			float ax = InputHelper.axisX;
			float ay = InputHelper.axisY;

			jSpeed -= gravity * Time.fixedDeltaTime;

			transform.forward = direction;

			Vector3 dir = (direction * ay + transform.right * ax) * entity.speed * 0.1f + (Vector3.up * jSpeed * Time.deltaTime);
			charCrtl.Move(dir);
			entity.animator.SetFloat("axisX", ax);
			entity.animator.SetFloat("axisY", ay);
		}

		void Jump() {
			if (charCrtl.isGrounded) {
				if(fallForce <= -15) {
					entity.DoHit(-fallForce*4);
					fallForce = 0;
				}
				if (InputHelper.Jump) {
					jSpeed = entity.jumpForce+charCrtl.velocity.y;
				}
			} else {
				if(!Physics.Raycast(transform.position, Vector3.down, 0.5f)) {
					if(fallForce == 0)
						jSpeed = 0;
					fallForce = jSpeed+1;
				}
				
			}
			Debug.Log(jSpeed);
		}
		float jSpeed;
		float fallForce;
		void Slash() {
			if (InputHelper.Slash) {
				entity.DoSlash();
			}
		}
		void TakeThingAndInventary() {
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if(Physics.Raycast(ray,out hit)) {
				Debug.DrawLine(entity.head.position, hit.point, Color.red);
				if (hit.transform.CompareTag("Thing")) {
					if (Vector3.Distance(entity.head.position, hit.transform.position) < 3f) {

						aim.Action();
						if (InputHelper.Action) {
							Thing t = hit.transform.GetComponent<Thing>();
							if (t == null) {
								t = hit.transform.GetComponentInParent<Thing>();
							}
							entity.TakeThing(t);
							return;
						}
					} else {
						aim.Default();
					}
				} else {
					aim.Default();
				}
			}
		}
	}
}