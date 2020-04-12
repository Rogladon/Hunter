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

		public Idle idle { get; private set; }
		public Run run { get; private set; }
		public Patrul patrul { get; private set; }
		public Attack attack { get; private set; }
		public Death death { get; private set; }

		public State currentState;

		public Transform transformTarget;
		public List<Entity> enemies;
		public Vector3 vectorTarget;
		public List<Transform> pointPatruls;
		public Entity enemy;
		public List<Entity> friends = new List<Entity>();

		private string _name;
		public  bool isDeath;
		public void Start() {
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
			if (friends.Count > 0 && friends != null) {
				foreach (var i in friends) {
					if (i == null) {
						friends.Remove(i);
						continue;
					}
					if (i.isDeath) {
						friends.Remove(i);
						continue;
					}
					if (Vector3.Distance(entity.position, i.position) > 100f) {
						friends.Remove(i);
					}
				}
			}
			foreach (State st in currentState.nextStates) {
				if (st.Condition()) {
					currentState.EndState();
					currentState = st;
					currentState.StartState();
					name = _name + " : " + currentState.nameState;
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
			Entity enemy = other.GetComponent<Entity>();
			if (enemy == null) {
				return;
			}
			if (enemy.fraction != entity.fraction) {
				enemies.Remove(enemy);
			}
		}

		public void AddEnemies(Entity _enemy) {
			if (!enemies.Contains(_enemy)) {
				enemies.Add(_enemy);
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