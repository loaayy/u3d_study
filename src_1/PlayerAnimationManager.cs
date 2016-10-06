using UnityEngine;
using System.Collections;

public class PlayerAnimationManager : MonoBehaviour {

	private Animator animator;
	private InputState inputState;

	void Awake(){
		animator = GetComponent<Animator> ();
		inputState = GetComponent<InputState> ();
	}

	// Update is called once per frame
	void Update () {

		animator.SetBool ("Running", inputState.standing);
	}
}
