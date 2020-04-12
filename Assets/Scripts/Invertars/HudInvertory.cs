using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Hunter {
	public class HudInvertory : MonoBehaviour {
		public Inventory inventory;

		[Header("Components")]
		public GameObject content;
		public Entity entity;
		public Text infoName;
		public Text infoMainStat;
		public Text infoOtherStat;
		public Text infoDestription;
		public Text infoPrice;
		public Text infoWeight;
		public Transform prewDomain;
		public GameObject infoPanel;

		[Header("Prefabs")]
		public Item prefabNameThing;

		private List<Item> currentThings = new List<Item>();
		private GameObject prewThing;
		private Thing.TypeThing currentType;
		private Thing weapon;

		void Start() {
			entity = GameObject.FindGameObjectWithTag("Player").GetComponent<Entity>();
			inventory = entity.inventory;
		}

		private void Update() {
			if (inventory == null) {
				inventory = entity.inventory;
			}
		}

		private void OnEnable() {
			SetThigsInContent(currentType.ToString());
		}

		public void SetThigsInContent(string typeString) {
			Thing.TypeThing type;
			try {
				type = (Thing.TypeThing)Enum.Parse(typeof(Thing.TypeThing), typeString);
			} catch (ArgumentException e) {
				Debug.LogError($"Failed to select type {typeString}. There is no such element in enum:\n{e.ToString()}");
				return;
			}
			if (currentThings.Count != 0) {
				foreach (var i in currentThings) {
					Destroy(i.gameObject);
				}
			}
			currentType = type;
			currentThings = new List<Item>();
			foreach (var i in inventory.things) {
				if (i.typeThing == type) {
					bool _continue = false; ;
					foreach(var _item in currentThings) {
						if(_item.thing == i) {
							_item.count+=1;
							_continue = true;
						}
					}
					if (_continue) continue;
					Item go = Instantiate(prefabNameThing, content.transform);
					go.thing = i;
					go.text.text = i.nameThing;
					currentThings.Add(go);
					EventTrigger trigger = go.GetComponent<EventTrigger>();
					EventTrigger.Entry entry = new EventTrigger.Entry();
					entry.eventID = EventTriggerType.PointerEnter;
					entry.callback.AddListener((data) => EnbleInfoThing(go));
					trigger.triggers.Add(entry);
					EventTrigger.Entry entry2 = new EventTrigger.Entry();
					entry2.eventID = EventTriggerType.PointerExit;
					entry2.callback.AddListener((data) => DisbleInfoThng(go));
					trigger.triggers.Add(entry2);
					EventTrigger.Entry entry3 = new EventTrigger.Entry();
					entry3.eventID = EventTriggerType.PointerClick;
					entry3.callback.AddListener((data) => SelectThing(i));
					trigger.triggers.Add(entry3);
				}
			}
			if (currentThings.Count == 0) {
				Item go = Instantiate(prefabNameThing,content.transform);
				go.text.text = "Пусто";
				go.text.alignment = TextAnchor.MiddleCenter;
				currentThings.Add(go);
			}
		}
		public void DisbleInfoThng(Item t) {
			infoPanel.SetActive(false);
			Destroy(prewThing);
			t.OffSelectObject();
			infoName.text = "";
			infoMainStat.text = "";
			infoDestription.text = "";
			infoOtherStat.text = "";
		}

		public void EnbleInfoThing(Item item) {
			Thing t = item.thing;
			item.OnSelect();
			infoPanel.SetActive(true);
			if (prewThing != null) {
				Destroy(prewThing);
			}
			infoName.text = t.nameThing;
			switch (t.typeThing) {
				case Thing.TypeThing.weapon:
					infoMainStat.text = "Урон: " + ((Weapon)t).damage;
					break;
			}
			infoDestription.text = t.descroption;
			infoPrice.text = t.price.ToString();
			infoWeight.text = t.weight.ToString();
			prewThing = Instantiate(t, prewDomain).gameObject;
			prewThing.transform.SetParent(prewDomain);
			prewDomain.rotation = Quaternion.identity;
		}

		public void SelectThing(Thing t) {
			switch (t.typeThing) {
				case Thing.TypeThing.weapon:
					if (t.select) {
						t.select = false;
						entity.WeponDestroy();
						SetThigsInContent("weapon");
						return;
					} else {
						t.select = true;
					}
					entity.WeponInHand((Weapon)t);
					foreach (var i in inventory.things) {
						if (i.typeThing == Thing.TypeThing.weapon) {
							if (i != t)
								i.select = false;
						}
					}
					weapon = t;
					SetThigsInContent("weapon");
					break;
			}
		}
		private void OnDisable() {
			if (prewThing != null) {
				Destroy(prewThing);
			}
		}

	}
}
