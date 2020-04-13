using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Hunter {
	public class AI : MonoBehaviour {
		public Entity entity;
		public NavMeshAgent agent;
		[Range(0, 10)]
		public int agro;
		[Range(0, 10)]
		public int tactic;
		[Range(0, 12)]
		public int cowardice;

		public bool isCowardice { get; set; }

		public Idle idle { get; private set; }
		public Run run { get; private set; }
		public Patrul patrul { get; private set; }
		public Attack attack { get; private set; }
		public Death death { get; private set; }

		public State currentState;
		public string nameState;

		public Transform transformTarget;
		public List<Entity> enemies;
		public Vector3 vectorTarget;
		public List<Transform> pointPatruls;
		public Entity mainFriend;
		public Entity enemy;
		public List<Entity> friends = new List<Entity>();

		private string _name;
		public  bool isDeath;

		public float minDistanceOnGround = 0.3f;
		CapsuleCollider collider;
		public bool isOnGround {
			get {
				RaycastHit hitF;
				RaycastHit hitB;
				RaycastHit hitR;
				RaycastHit hitL;
				RaycastHit hitС;
				Vector3 pointB = transform.position + new Vector3(collider.center.x, collider.center.y - collider.height / 2, collider.center.z - collider.radius);
				Vector3 pointF = transform.position + new Vector3(collider.center.x, collider.center.y - collider.height / 2, collider.center.z + collider.radius);
				Vector3 pointR = transform.position + new Vector3(collider.center.x + collider.radius, collider.center.y - collider.height / 2, collider.center.z);
				Vector3 pointL = transform.position + new Vector3(collider.center.x - collider.radius, collider.center.y - collider.height / 2, collider.center.z);
				Vector3 pointС = transform.position + new Vector3(collider.center.x, collider.center.y - collider.height / 2, collider.center.z);
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


		public void Start() {
			collider = GetComponent<CapsuleCollider>();
			_name = name;
			entity = GetComponent<Entity>();
			agent = GetComponent<NavMeshAgent>();
			run = new Run(this, entity);
			patrul = new Patrul(this, entity);
			attack = new Attack(this, entity);
			death = new Death(this, entity);
			idle = new Idle(this, entity);
			run.Init();
			patrul.Init();
			attack.Init();
			death.Init();
			idle.Init();
			currentState = idle;
		}


		private void Update() {
			if (isDeath) return;
			nameState = currentState.nameState;
			List<State> nextState = new List<State>();
			foreach (State st in currentState.nextStates) {
				if (st.Condition()) {
					nextState.Add(st);
					//Debug.Log(name+": AddState: " + st.nameState);
				}
			}
			if (nextState.Count != 0) {
				State state = nextState[0];
				foreach (State st in nextState) {
					if (state.priority < st.priority)
						state = st;
				}
				if (!(currentState.Condition() && currentState.priority > state.priority)) {
						currentState.EndState();
						currentState = state;
						currentState.StartState();
						Debug.Log(name+": CurrentState = " + currentState.nameState);
				}
			}
			currentState.Behaviour();
		}

		private void OnTriggerEnter(Collider other) {
			Entity enemy = other.GetComponent<Entity>();
			if(enemy == null) {
				return;
			}
			if(enemy.fraction != entity.fraction) {
				AddEnemies(enemy);
			} else {
				if (!friends.Contains(enemy) && !enemy.CompareTag("Player"))
					friends.Add(enemy);
			}
		}


		private void OnTriggerExit(Collider other) {
			Entity _enemy = other.GetComponent<Entity>();
			if (_enemy == null) {
				return;
			}
			if (_enemy.fraction != entity.fraction) {
				RemoveEnemies(_enemy);
			}
		}

		public void AddEnemies(Entity _enemy) {
			if (!enemies.Contains(_enemy)) {
				enemies.Add(_enemy);
			}
		}

		public void RemoveEnemies(Entity _enemy) {
			if(enemy == _enemy) {
				//enemy = null;
				enemies.Remove(_enemy);
			}
		}


		Vector3 lastPos;
		private void LateUpdate() {
			Vector3 deltaPos = transform.position - lastPos;
			lastPos = transform.position;
			Vector2 dp = _Vector2(deltaPos);
			Vector2 f = _Vector2(transform.forward);
			Vector2 r = _Vector2(transform.right);
			dp = dp.x * f + dp.y * r;
			dp = dp.normalized;
			if (dp != Vector2.zero)
				dp = Max(dp);

			entity.animator.SetFloat("axisX", dp.y);
			entity.animator.SetFloat("axisY", dp.x);
			entity.animator.SetBool("onGround", isOnGround);
			
		}

		public Vector2 _Vector2(Vector3 v) {
			return new Vector2(v.x, v.z);
		}
		Vector2 Max(Vector2 v) {
			float max = v.x;
			if (v.y > max) {
				max = v.y;
			}
			float delta = 1 - max;
			v.x += delta;
			v.y += delta;
			return v;
		}
	}
}