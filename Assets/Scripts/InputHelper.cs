using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hunter {
	public class InputHelper : MonoBehaviour {
		static public float axisX {
			get {
				return Input.GetAxis("Horizontal");
			}
		}

		static public float axisY {
			get {
				return Input.GetAxis("Vertical");
			}
		}

		static public bool Jump {
			get {
				if (Input.GetKeyDown(KeyCode.Space)) {
					return true;
				} else {
					return false;
				}
			}
		}

		static public bool Sprint {
			get {
				if (Input.GetKey(KeyCode.LeftShift)) {
					return true;
				} else {
					return false;
				}
			}
		}

		static public bool Walk {
			get {
				if (Input.GetKeyDown(KeyCode.CapsLock)) {
					return true;
				} else {
					return false;
				}
			}
		}

		static public bool Slash {
			get {
				if (Input.GetMouseButtonDown(0)) {
					return true;
				} else {
					return false;
				}
			}
		}

		static public bool Action {
			get {
				if (Input.GetKeyDown(KeyCode.E)) {
					return true;
				} else {
					return false;
				}
			}
		}

		public static Vector2 deltaScroll {
			get {
				return Input.mouseScrollDelta;
			}
		}

		static public bool isScroll {
			get {
				if(Input.mouseScrollDelta != Vector2.zero) {
					return true;
				}
				return false;
			}
		}
	}
}
