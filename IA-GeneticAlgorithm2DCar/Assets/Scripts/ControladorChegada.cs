﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControladorChegada : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	void OnTriggerEnter2D(Collider2D col)
    {
        col.GetComponent<Rigidbody2D>().simulated = false;
    }
}
