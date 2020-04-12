using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hunter {


	public class Idle : State {

		public Idle(AI _ai, Entity _entity) {
			ai = _ai;
			entity = _entity;
			
		}
		public override void Behaviour() {
			
		}

		public override bool Condition() {
			if(ai.transformTarget == null && ai.vectorTarget == null && ai.enemies.Count == 0) {
				return true;
			}
			return false;
		}

		public override void Init() {
			PrevInit();
			nameState = "Idle";
			nextStates.Add(ai.death);
			nextStates.Add(ai.run);
			nextStates.Add(ai.patrul);
			nextStates.Add(ai.attack);
		}
	}

	public class Run : State {
		bool cowardice;
		int countFriend = 0;
		Entity friend;
		public Run(AI _ai, Entity _entity) {
			ai = _ai;
			entity = _entity;
			
		}
		public override void StartState() {
			if(entity.hp <= entity.healthPoint * (ai.cowardice / 10)) {
				cowardice = true;
			} else {
				cowardice = false;
			}
		}

		public override void Behaviour() {
			if (cowardice) {
				if (countFriend != ai.friends.Count) {
					if (countFriend == 0) {
						ai.transformTarget = ai.friends[0].transform;
						friend = ai.friends[0];
					}
					Debug.Log("Heeeelp");
					ai.vectorTarget = Vector3.zero;
					countFriend = ai.friends.Count;
				}
				if(Vector3.Distance(entity.position, friend.position) < 15f) {
					friend.ai.agro = 10;
					friend.ai.AddEnemies(ai.enemy);
				}
			}
			if (ai.transformTarget != null) {
				ai.agent.destination = ai.transformTarget.position;
				//if (ai.agent.isStopped) {
				if (Vector3.Distance(entity.position, ai.transformTarget.position) < 2f) {
					if (cowardice && ai.cowardice > 10 && ai.friends.Count != 0) {
						ai.transformTarget = ai.friends[0].transform;
					} else {
						ai.transformTarget = null;
					}
				}
				//}
				return;
			} else if (ai.vectorTarget != Vector3.zero) {
				ai.agent.destination = ai.vectorTarget;
				//if (ai.agent.isStopped) {
					if(Vector3.Distance(entity.position, ai.vectorTarget) < 2f){
						ai.vectorTarget = Vector3.zero;
					}
				//}
				return;
			}
			if (cowardice && ai.cowardice <= 10 && ai.transformTarget == null) {
				ai.agro = 10;
				ai.cowardice = (int)(entity.hp / entity.healthPoint * 10) - 1;
			}
		}

		public override bool Condition() {
			if(ai.transformTarget != null || (ai.vectorTarget != null && ai.vectorTarget != Vector3.zero)) {
				return true;
			}
			return false;
		}

		public override void Init() {
			PrevInit();
			nameState = "Run";
			nextStates.Add(ai.idle);
			nextStates.Add(ai.attack);
			nextStates.Add(ai.death);
		}
	}

	public class Death : State {

		public Death(AI _ai, Entity _entity) {
			ai = _ai;
			entity = _entity;
			
		}

		public override void StartState() {
			entity.GetComponent<CapsuleCollider>().enabled = false;
			entity.rigid.isKinematic = true;
			ai.isDeath = true;
		}
		public override void Behaviour() {
			
		}

		public override bool Condition() {
			if(entity.hp <= 0) {
				return true;
			}
			return false;
		}

		public override void Init() {
			PrevInit();
			nameState = "Death";
		}
	}
	public class Patrul : State {
		int countPoint = 0;
		public Patrul(AI _ai, Entity _entity) {
			ai = _ai;
			entity = _entity;
			
		}
		public override void Behaviour() {
			ai.agent.stoppingDistance = 0.5f;
			if(countPoint < ai.pointPatruls.Count) {
				ai.agent.destination = ai.pointPatruls[countPoint].position;
			} else {
				countPoint = 0;
			}
			if (Vector3.Distance(entity.position, ai.pointPatruls[countPoint].position)<1f) countPoint++;
		}

		public override bool Condition() {
			if(ai.pointPatruls != null) {
				if(ai.pointPatruls.Count != 0) {
					return true;
				}
			}
			return false;
		}

		public override void Init() {
			PrevInit();
			nameState = "Patrul";
			nextStates.Add(ai.death);
			nextStates.Add(ai.idle);
			nextStates.Add(ai.run);
			nextStates.Add(ai.attack);
		}
	}
	public class Attack : State {
		public Entity enemy;
		public RunAttack runAttack { get; private set; }
		public SlashAttack slashAttack { get; private set; }
		public TacticMove tacticMove { get; private set; }
		int currentEnemy = 0;
		public Attack(AI _ai, Entity _entity) {
			ai = _ai;
			entity = _entity;
			
		}

		public override void StartState() {
			enemy = ai.enemies[0];
			if (ai.friends.Count != 0) {
				foreach(var i in ai.friends) {
					if(Vector3.Distance(entity.position, i.position) < 15f) {
						i.ai.AddEnemies(enemy);
						i.ai.agro = 5;
					}
				}
			}
		}

		public override void Behaviour() {
			ai.agent.stoppingDistance = 1.5f;
			ai.vectorTarget = Vector3.zero;
			ai.transformTarget = null;
			if (currentEnemy >= ai.enemies.Count) currentEnemy = 0;
			if (ai.enemies.Count != 0) {
				enemy = ai.enemies[0];
				ai.enemy = enemy;
			}
			if (ai.friends.Count != 0) {
				foreach (var i in ai.friends) {
					if (Vector3.Distance(entity.position, i.position) < 15f) {
						if (i.ai.enemy == null) {
							i.ai.AddEnemies(enemy);
							i.ai.agro = 5;
						}
					}
				}
			}
			if (enemy == null || enemy.isDeath) {
				ai.enemies.Remove(enemy);
				return;
			}

			if (ai.cowardice != 0) {
				if (entity.hp <= entity.healthPoint * (ai.cowardice/ 10)) {
					if (ai.friends.Count != 0) {
						float min = 1000f;
						Entity friend = entity;
						foreach (var i in ai.friends) {
							if(i == null) {
								ai.friends.Remove(i);
							}
							if (Vector3.Distance(entity.position, i.position) < min) {
								min = Vector3.Distance(entity.position, i.position);
								friend = i;
							}
						}
						ai.transformTarget = friend.transform;
					} else {
						float x = Random.Range(-0.5f, 0.5f);
						float z = 1;
						Vector2 p = new Vector2(x, z);
						Vector2 f = ai._Vector2(enemy.transform.forward);
						Vector2 r = ai._Vector2(enemy.transform.right);
						p = p.x * f + p.y * r;
						p = p * 100;
						ai.vectorTarget = new Vector3(p.x,0,p.y);
					}
					Debug.Log(entity.name + "Aaaaaaaa!!!!");
					ai.agro = 1;
				}
			}

			foreach(State st in currentState.nextStates) {
				if (st.Condition()) {
					currentState.EndState();
					currentState = st;
					currentState.StartState();
					Debug.Log("Attack: " + currentState.nameState);
				}
			}
			currentState.Behaviour();
		}

		public override bool Condition() {
			if(ai.enemies != null) {
				if(ai.enemies.Count != 0 && ai.agro > 1) {
					return true;
				}
			}
			return false;
		}

		public override void Init() {
			PrevInit();
			nameState = "Attack";
			nextStates.Add(ai.death);
			nextStates.Add(ai.idle);
			nextStates.Add(ai.run);
			runAttack = new RunAttack(ai, entity, this);
			slashAttack = new SlashAttack(ai, entity,this);
			tacticMove = new TacticMove(ai, entity, this);
			slashAttack.Init();
			runAttack.Init();
			tacticMove.Init();
			currentState = runAttack;
		}
	}

	public class RunAttack : State {
		public Attack attack;
		public RunAttack(AI _ai, Entity _entity, Attack _attack) {
			ai = _ai;
			entity = _entity;
			attack = _attack;
		}
		public override void Behaviour() {
			if(attack.enemy != null) {
				ai.agent.destination = attack.enemy.position;
			}
		}

		public override bool Condition() {
			if (attack.enemy != null) {
				if(Vector3.Distance(entity.position, attack.enemy.position) >= 2) {
					return true;
				}
			}
			return false;
		}

		public override void Init() {
			PrevInit();
			nameState = "RunAttack";
			nextStates.Add(attack.slashAttack);
			nextStates.Add(attack.tacticMove);
		}

	}
	public class SlashAttack : State {
		public Attack attack;
		public SlashAttack(AI _ai, Entity _entity, Attack _attack) {
			ai = _ai;
			entity = _entity;
			attack = _attack;
		}
		public override void Behaviour() {
			if (attack.enemy != null) {
				if(entity.countSlash <= 2) {
					entity.DoSlash();
				} else {
					entity.countSlash = 0;
				}
				entity.transform.LookAt(attack.enemy.transform,Vector3.up);
			}
		}

		public override bool Condition() {
			if (attack.enemy != null) {
				if (Vector3.Distance(entity.position, attack.enemy.position) < 2 && ai.tactic <=1) {
					return true;
				}
			}
			return false;
		}

		public override void Init() {
			PrevInit();
			nameState = "SlashAttack";
			nextStates.Add(attack.runAttack);
			nextStates.Add(attack.tacticMove);
		}
	}

	public class TacticMove : State {
		public Attack attack;
		Vector3 lastPos;
		float minTime = 3;
		float maxTime = 8;
		float _timer;
		public TacticMove(AI _ai, Entity _entity, Attack _attack) {
			ai = _ai;
			entity = _entity;
			attack = _attack;
		}
		public override void StartState() {
			ai.agent.speed = entity.speedWalk;
			ai.agent.angularSpeed = 0;
		}
		public override void EndState() {
			ai.agent.speed = entity.speed;
			ai.agent.angularSpeed = 120;
		}
		public override void Behaviour() {
			if (lastPos == entity.position && _timer <= 0) {
				_timer = Random.Range(minTime / ai.tactic, maxTime / ai.tactic);
				float x = Random.Range(-1, 1);
				float z = Random.Range(-1, 1);
				Vector3 p = new Vector3(x, 0, z);
				p = attack.enemy.position - p * 2.5f;
				ai.agent.destination = p;
			} else {
				_timer -= Time.deltaTime;
			}
			lastPos = entity.position;
			entity.transform.LookAt(attack.enemy.transform, Vector3.up);
			if (entity.countSlash <= 2) {
				entity.DoSlash();
			} else {
				entity.countSlash = 0;
			}
		}

		public override bool Condition() {
			if (attack.enemy != null) {
				if (Vector3.Distance(entity.position, attack.enemy.position) < 2 && ai.tactic>1) {
					return true;
				}
			}
			return false;
		}

		public override void Init() {
			PrevInit();
			nameState = "TactcAttack";
			nextStates.Add(attack.runAttack);
			nextStates.Add(attack.slashAttack);
		}
	}

	public static class MainAI{
		
	}
}
