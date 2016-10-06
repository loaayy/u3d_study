﻿using UnityEngine;
using System.Collections;

public class Jump : MonoBehaviour {

	public float jumpSpeed = 20f;
	public float forwardSpeed = 6;

	private Rigidbody2D body2d;
	private InputState inputState;
	private AudioSource jump;

	void Awake(){
		body2d = GetComponent<Rigidbody2D> ();
		inputState = GetComponent<InputState> ();
		jump= GameObject.Find("jump").GetComponent<AudioSource>();
	}

	// Update is called once per frame
	void Update () {
		if (inputState.standing) {
			if(inputState.actionButton){
					body2d.velocity = new Vector2(transform.position.x < 0 ? forwardSpeed : 0, jumpSpeed);
					jump.Play ();
			}
		}

	}
}
