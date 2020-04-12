using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hunter {
	public class Inventory : MonoBehaviour {
		[Header("Components")]
		public List<Thing> things = new List<Thing>();


		public void AddThing(Thing t) {
			things.Add(t);
		}
	}
}
