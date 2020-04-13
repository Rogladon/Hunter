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
			if(ai.transformTarget == null && ai.vectorTarget == Vector3.zero && ai.enemies.Count == 0) {
				return true;
			}
			return false;
		}

		public override void Init() {
			PrevInit();
			priority = 0;
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
			
			if (ai.transformTarget != null) {
				ai.agent.destination = ai.transformTarget.position;
				return;
			} else if (ai.vectorTarget != Vector3.zero) {
				ai.agent.destination = ai.vectorTarget;
				return;
			}
			//if (cowardice && ai.cowardice <= 10 && ai.transformTarget == null) {
			//	ai.agro = 10;
			//	ai.cowardice = (int)(entity.hp / entity.healthPoint * 10) - 1;
			//}
		}

		public override bool Condition() {
			if(ai.transformTarget != null || (ai.vectorTarget != null && ai.vectorTarget != Vector3.zero)) {
				return true;
			}
			return false;
		}

		public override void Init() {
			PrevInit();
			priority = 2;
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
			if(entity.hp <= 0 || entity.isDeath) {
				return true;
			}
			return false;
		}

		public override void Init() {
			PrevInit();
			priority = 100;
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
			priority = 1;
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
		public RunHelp runHelp { get; private set; }
		int currentEnemy = 0;
		public Attack(AI _ai, Entity _entity) {
			ai = _ai;
			entity = _entity;

		}

		public override void StartState() {
			if(ai.enemy == null) {
				enemy = ai.enemies[0];
			} else {
				enemy = ai.enemy;
			}
			if (ai.friends.Count != 0) {
				foreach (var i in ai.friends) {
					if (Vector3.Distance(entity.position, i.position) < 15f) {
						i.ai.AddEnemies(enemy);
						i.ai.agro = 5;
					}
				}
			}
			ai.agent.stoppingDistance = 1.5f;
			ai.vectorTarget = Vector3.zero;
			ai.transformTarget = null;
		}

		public override void Behaviour() {
			if (currentEnemy >= ai.enemies.Count) currentEnemy = 0;
			if (ai.enemies.Count != 0) {
				enemy = ai.enemies[0];
				ai.enemy = enemy;
			}
			if (ai.friends.Count != 0) {
				foreach (var i in ai.friends) {
					if (Vector3.Distance(entity.position, i.position) < 15f) {
							i.ai.AddEnemies(enemy);
							if(!i.ai.isCowardice)
								i.ai.agro = 5;
					}
				}
			}
			if (enemy == null || enemy.isDeath) {
				ai.enemies.Remove(enemy);
				ai.enemy = null;
				return;
			}


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
					Debug.Log(ai.name + ": CurrentState = " + currentState.nameState);
				}
			}
			currentState.Behaviour();
		}

		public override bool Condition() {
			if(ai.enemies != null || ai.enemy != null) {
				if((ai.enemies.Count != 0 || ai.enemy != null)&& ai.agro > 1) {
					return true;
				}
			}
			return false;
		}

		public override void Init() {
			PrevInit();
			priority = 3;
			nameState = "Attack";
			nextStates.Add(ai.death);
			nextStates.Add(ai.idle);
			nextStates.Add(ai.run);
			runAttack = new RunAttack(ai, entity, this);
			slashAttack = new SlashAttack(ai, entity,this);
			tacticMove = new TacticMove(ai, entity, this);
			runHelp = new RunHelp(ai, entity, this);
			runHelp.Init();
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
			nextStates.Add(attack.runHelp);
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
			priority = 0;
			nameState = "SlashAttack";
			nextStates.Add(attack.runAttack);
			nextStates.Add(attack.tacticMove);
			nextStates.Add(attack.runHelp);
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
			priority = 1;
			nameState = "TactcAttack";
			nextStates.Add(attack.runAttack);
			nextStates.Add(attack.slashAttack);
			nextStates.Add(attack.runHelp);
		}
	}

	public class RunHelp : State {
		public Attack attack;
		Entity friend;
		public RunHelp(AI _ai, Entity _entity, Attack _attack) {
			ai = _ai;
			entity = _entity;
			attack = _attack;
		}

		public override void StartState() {
			Debug.Log("AAAAA");
			if (ai.mainFriend) {
				friend = ai.mainFriend;
				return;
			}
			Entity _entity = entity;
			float d = 1000f;
			if (ai.friends.Count > 0 && ai.friends != null) {
				foreach (var i in ai.friends) {
					if (i == null) {
						ai.friends.Remove(i);
						continue;
					}
					if (i.isDeath) {
						ai.friends.Remove(i);
						continue;
					}
					if (Vector3.Distance(entity.position, i.position) > 100f) {
						ai.friends.Remove(i);
						continue;
					}
					if(Vector3.Distance(entity.position, i.position) < d) {
						d = Vector3.Distance(entity.position, i.position);
						_entity = i;
					}
				}
				if (entity != _entity) {
					friend = _entity;
				} else {
					ai.cowardice = 0;
				}
			}
		}

		public override void Behaviour() {
			if (Vector3.Distance(entity.position, friend.position) > 3f) {
				ai.agent.destination = friend.position;
			}
			if (Vector3.Distance(entity.position, friend.position) < 3f) {
				
				if (ai.cowardice <= 10) {
					ai.agro = 10;
				} else {
					ai.agro = 1;
					ai.isCowardice = true;
					ai.transformTarget = friend.transform;
				}
				ai.cowardice = (int)(entity.hp / entity.healthPoint * 10) - 1;
			}
		}

		public override void EndState() {
			friend.ai.AddEnemies(attack.enemy);
			Debug.Log("Heeelp");
		}

		public override bool Condition() {
			if (ai.cowardice != 0) {
				if (entity.hp <= entity.healthPoint * (ai.cowardice / 10)) {
					return true;
				}
			}
			return false;
		}

		public override void Init() {
			PrevInit();
			nameState = "RunHelp";
			nextStates.Add(attack.slashAttack);
			priority = 4;
		}
	}

	public static class MainAI{
		
	}
}
/*
 * 
 * 
 * 
 * 
 * 
 * 
 */