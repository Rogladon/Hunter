using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hunter {
	public class Weapon :  Thing {

		public enum TypeWepon {
			oneHanded,//одноручное
			doubleHanded,//двуручное
			spear,//копья
			chain//цепи
		}

		public Dictionary<TypeWepon, string> typeNameWeapon = new Dictionary<TypeWepon, string>();

		[Header("Stats")]
		public TypeWepon typeWepon;
		public float damage;
		public BuffDamage superSlash;

		[Header("Components")]
		public Entity entity;
		public Collider collider;

		void Start() {
			if(collider == null)
				collider = GetComponent<Collider>();
			typeThing = TypeThing.weapon;
			InitialDictianory();
		}

		void InitialDictianory() {
			typeNameWeapon.Add(TypeWepon.chain, "Цепь");
			typeNameWeapon.Add(TypeWepon.oneHanded, "Одноручное");
			typeNameWeapon.Add(TypeWepon.doubleHanded, "Двуручное");
			typeNameWeapon.Add(TypeWepon.spear, "Колющее");
		}

		private void OnTriggerEnter(Collider collision) {
			if (entity == null) return;
			Entity _entity = collision.transform.GetComponent<Entity>();

			if(entity == _entity || _entity == null) {
				return;
			}
			if(entity.ai != null) {
				if(_entity != entity.ai.enemy) {
					return;
				}
			}
			entity.DoAttack(damage, _entity);
		}
	}
}
