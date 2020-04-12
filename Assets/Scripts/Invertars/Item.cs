using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Hunter {
	public class Item : MonoBehaviour {
		public int count;

		public Text text;

		public GameObject onSelectObject;
		public Thing thing { get; set; }
		public bool isSelect { get; set; }

		void Start() {
			UpdateInfo();
			count = 1;
		}
		public void OnSelect() {
			onSelectObject.SetActive(true);
		}
		public void OffSelectObject() {
			onSelectObject.SetActive(false);
		}
		public void UpdateInfo() {
			if (!thing) return;
			if (count > 1) {
				text.text = thing.nameThing + " (" + count + ")";
			} else {
				text.text = thing.nameThing;
			}
		}
	}
}
