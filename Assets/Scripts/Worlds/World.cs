using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
	public float height;
	public float width;
	public Transform thisTerrain;
    void Start()
    {
        
    }

	void Update() {

	}

	private void OnTriggerEnter(Collider other) {
		Vector3 pos = other.transform.position - thisTerrain.position;
		other.transform.position = new Vector3(pos.x, other.transform.position.y, pos.z);
	}
}
