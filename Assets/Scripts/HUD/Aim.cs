using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Aim : MonoBehaviour
{
	public RectTransform rect;

	void Start() {
		rect = GetComponent<RectTransform>();
	}

	public void Action() {
		rect.sizeDelta = new Vector2(30, 30);
	}
	public void Default() {
		rect.sizeDelta = new Vector2(50, 50);
	}
}
