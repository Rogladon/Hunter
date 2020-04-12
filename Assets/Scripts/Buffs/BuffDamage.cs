using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hunter {
	public class BuffDamage : Buff {
		public float dmgFactor;
		protected override void Action() {
			entity.weapon.damage = (entity.weapon.damage * dmgFactor);
			Debug.Log(entity.weapon.damage);
		}

		protected override void OnDestroy() {
			entity.weapon.damage = (entity.weapon.damage / dmgFactor);
		}
	}
}
