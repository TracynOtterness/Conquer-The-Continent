using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMap : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Camera camera = this.GetComponent<Camera>();
        camera.aspect = 1.3f;
	}
}
