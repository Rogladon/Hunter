using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AraundThing : MonoBehaviour
{
	public float speedAround = 1f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		transform.Rotate(Vector3.up * speedAround * Time.deltaTime);
    }
}
