using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hunter {
	public class Thing : MonoBehaviour {
		public enum TypeThing {
			other,
			weapon,
			armor,
			sign
			
		}
		[Header("BaseInfo")]
		public Thing actionPrefab;
		public Thing viewPrefab;
		public int price;
		public int weight;
		public string nameThing;
		public string descroption;
		public TypeThing typeThing;
		public bool select { get; set; }

	}
}
