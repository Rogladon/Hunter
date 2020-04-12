using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Hunter {
	public class HUD : MonoBehaviour {
		public bool isVisibleCursor => isVisibleMenu;
		bool _isVisibleMenu = false;
		public bool isVisibleMenu {
			get { 
				if(gameMenu.activeSelf || invertoruMenu.activeSelf) {
					return true;
				}
				return _isVisibleMenu; }
			set {
				_isVisibleMenu = value;
			}
		}

		[Header("Components")]
		[SerializeField]
		GameObject aim;
		[SerializeField]
		GameObject gameMenu;
		[SerializeField]
		GameObject invertoruMenu;
		CameraController cameraController;
		PlayerController playerController;
		//public static Inventory allInventory;
		void Start() {
			//Thing[] ts = (Thing [])Resources.FindObjectsOfTypeAll(typeof(Thing));
			isVisibleMenu = false;
			cameraController = Camera.main.GetComponent<CameraController>();
			playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
		}

		// Update is called once per frame
		void Update() {
			if (Input.GetKeyDown(KeyCode.Escape)) {
				gameMenu.SetActive(!isVisibleMenu);
			}
			Cursor.visible = isVisibleCursor;
			aim.SetActive(!isVisibleCursor);
			if (isVisibleCursor) {
				Cursor.lockState = CursorLockMode.None;
			} else {
				Cursor.lockState = CursorLockMode.Locked;
			}
			if (Input.GetKeyDown(KeyCode.I)) {
				invertoruMenu.SetActive(!isVisibleMenu);
			}
			if (isVisibleMenu) {
				playerController.isLockedMove = true;
				cameraController.isLocked = true;
			} else {
				playerController.isLockedMove = false;
				cameraController.isLocked = false;
			}
		}

		public void Continue() {
			Time.timeScale = 0;
			isVisibleMenu = false;
			
		}
		public void Pause() {
			Time.timeScale = 0;
			
		}
		public void Exit() {

		}

	}
}
