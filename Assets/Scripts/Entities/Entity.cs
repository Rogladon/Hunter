using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Hunter {
	[AddComponentMenu("Hunter/Entities/Entity")]
	[DisallowMultipleComponent]
	public class Entity : MonoBehaviour {
		public enum TypeSpeed {
			run,
			walk,
			sprint
		}

		public TypeSpeed typeSpeed;

		public int fraction;


		[Header("Stats")]
		public int healthPoint;
		public float strength;
		public float dexteritg;
		public float protection;

		[Header("Const")]

		public float speedRun = 5f;
		public float speedSprintRun = 10f;
		public float jumpForce = 100f;
		public float speedWalk = 2f;
		public float reloadSlash = 1;


		[Header("Components")]
		public Rigidbody rigid;
		public Transform footPoint;
		public Animator animator;
		public Transform weaponHolderRight;
		public Transform weaponHolderLeft;
		public Transform buffHolder;
		public Inventory inventory;
		public Transform head;
		public Weapon weapon;
		public Text hpSellf;
		public Text hpEnemy;
		public IntAnimatorDictionary animatorDictionary = IntAnimatorDictionary.New<IntAnimatorDictionary>();
		public Dictionary<int, RuntimeAnimatorController> animators { get { return animatorDictionary.dictionary; } }


		[Header("Baffs")]
		public Dictionary<string, float> bufDamage;


		private bool _isLockedMove;
		public bool isLockedMove { get { if (isDeath) return true; else return _isLockedMove; } set { _isLockedMove = value; } }
		public bool isDeath = false;


		public Vector3 position {
			get {
				return transform.position;
			}
		}

		private float _hp;
		public float hp {
			get {
				return (int)_hp;
			}
			set {
				if (value > healthPoint) {
					_hp = healthPoint;
				} else if (value <= 0) {
					_hp = 0;
					isDeath = true;
					animator.SetBool("death", true);
				} else {
					_hp = value;
				}
			}
		}

		public Vector3 direction {
			get {
				Vector3 dir = transform.forward;
				return dir.normalized;
			}
		}


		

		public float speed {
			get {
				switch (typeSpeed) {
					case TypeSpeed.run:
						return speedRun;
						break;
					case TypeSpeed.walk:
						return speedWalk;
						break;
					case TypeSpeed.sprint:
						return speedSprintRun;
						break;
				}
				return 0;
			}
		}

		public List<Buff> buffs = new List<Buff>();

		public int countSlash = 0;
		float _timerSlash = 0f;
		Collider colliderIinventary;
		public AI ai;
		void Start() {
			if (GetComponent<AI>()) {
				ai = GetComponent<AI>();
			}
			inventory = GetComponentInChildren<Inventory>();
			colliderIinventary = inventory.GetComponent<Collider>();
			colliderIinventary.enabled = false;
			rigid = GetComponent<Rigidbody>();
			if (rigid == null) {
				GetComponentInChildren<Rigidbody>();
			}
			animator = GetComponent<Animator>();
			if(animator == null) {
				animator = GetComponentInChildren<Animator>();
			}
			hp = healthPoint;
			animator.runtimeAnimatorController = animators[5];
			if (weapon != null) {
				WeponInHand(weapon);
			}
			
		}

		void Update() {
			if(countSlash != 0) {
				_timerSlash += Time.deltaTime;
				if(_timerSlash >= reloadSlash) {
					countSlash = 0;
					_timerSlash = 0f;
				}
			}
			animator.SetInteger("attack", countSlash);
			
			//animator.SetBool("hit", false);
		}
#region MOVE
		

		
		#endregion

#region ATTACK
		public void DoSlash() {
			if (isLockedMove) return;
			countSlash++;
			_timerSlash = 0;
		}

		public void DoAttack(float dmg, Entity _entity) {
			if (countSlash == 0) return;
			Debug.Log(name + ": I'm Attack " + _entity.name);
			float damage = dmg + strength;
			_entity.DoHit(damage);
			if (hpSellf != null)
				hpEnemy.text = (_entity.hp)+ "/" + _entity.healthPoint;
		}

		public void DoHit(float dmg) {
			animator.SetBool("hit", true);
			hp -= dmg;
			if(ai)
				ai.agro++;
			if(hpSellf != null)
				hpSellf.text = hp + "/" + healthPoint;
			//Debug.Log(name + ": Aaaa, I took damage " + dmg+". My hp="+hp);
		}
		#endregion


#region INTERECTION
		public void WeponInHand(Weapon _weapon) {
			//if (weaponHolderRight.childCount != 0) {
			//	foreach(Transform w in weaponHolderRight) {
			//		Destroy(w.gameObject);
			//	}
			//}
			if (_weapon.typeWepon == Weapon.TypeWepon.doubleHanded) {
				WeponDestroy();
				weapon = Instantiate(_weapon, weaponHolderLeft);
			}
			if (_weapon.typeWepon == Weapon.TypeWepon.oneHanded) {
				WeponDestroy();
				weapon = Instantiate(_weapon, weaponHolderRight);
			}
			weapon.entity = this;
			animator.runtimeAnimatorController = animators[(int)weapon.typeWepon];
			//animator.SetInteger("typeWeapon", (int)weapon.typeThing);
		}

		public void WeponDestroy() {
			WeponDestroy(weaponHolderLeft);
			WeponDestroy(weaponHolderRight);
		}
		public void WeponDestroy(Transform holder) {
			Weapon[] ws = holder.GetComponentsInChildren<Weapon>();
			foreach(var i in ws) {
				Destroy(i.gameObject);
			}
			animator.runtimeAnimatorController = animators[5];
		}
		public void TakeThing(Thing t) {
			inventory.AddThing(t.actionPrefab);
			Destroy(t.gameObject);
		}
#endregion

		public void AddBuff(Buff buff) {
			if(buff == null) {
				//	Debug.LogError(name + ": This buff does't exist");
				return;
			}
			//Debug.Log(name + ": I got buff " + buff.name);
			Buff b = Instantiate(buff, buffHolder);
			buffs.Add(b);
		}
		public void RemoveBuff(Buff buff) {
			foreach(var i in buffs) {
				if(i.nameBuff == buff.nameBuff) {
					buffs.Remove(i);
					Destroy(i.gameObject);
				//	Debug.Log(name + "I remove buff" + buff.name);
					return;
				}
			}
		}
	}
}
