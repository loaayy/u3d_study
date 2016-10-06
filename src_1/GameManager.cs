using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using UnityEngine.Audio;

public class GameManager : MonoBehaviour {

	private GameObject floor;
	private Spawner spawner;
	public GameObject playerPrefab;
	private GameObject player;
	private bool gameStarted;
	private TimeManager timeManager;
	public Text continueText;
	private float blinkTime = 0f;
	private bool blink;

	public Text scoreText;
	private float timeElapsed = 0f;
	private float bestTime = 0f;

	private AudioSource backgroundSound;
	private AudioSource effectSound;
	private AudioSource gameOverSound;
	private AudioSource newRecordSound;

	private bool beatBestTime;

	void Awake(){
		floor = GameObject.Find ("floor");
		spawner = GameObject.Find ("Spawner").GetComponent<Spawner> ();
		timeManager = GetComponent<TimeManager> ();

		backgroundSound = GameObject.Find("backgroundSound").GetComponent<AudioSource>();
		effectSound = GameObject.Find("goSoundEffect").GetComponent<AudioSource>();
		gameOverSound = GameObject.Find("gameOver").GetComponent<AudioSource>();
		newRecordSound = GameObject.Find("newRecord").GetComponent<AudioSource>();

	}

	// Use this for initialization
	void Start () {
		var floorHeight = floor.transform.localScale.y;
		var pos = floor.transform.position;
		pos.x = 0;
		pos.y = -((Screen.height / PiexelPerfectCamera.pixelToUnit) / 2) + (floorHeight / 2);
		floor.transform.position = pos;
		spawner.active = false;
		Time.timeScale = 0;

		continueText.text = "PRESS ANY BUTTION TO START";	
		bestTime = PlayerPrefs.GetFloat ("BestTime");
	}

	void OnPlayerKilled()
	{
		spawner.active = false;
		var playerDestroyScript = player.GetComponent<DestroyOffscreen> ();
		playerDestroyScript.DestroyCallBack -= OnPlayerKilled;
		player.GetComponent<Rigidbody2D> ().velocity = Vector2.zero;

		timeManager.ManipulateTime (0, 8.5f);

		gameStarted = false;

		continueText.text = "PRESS ANY BUTTION TO RESTART";

		if (timeElapsed > bestTime) {
			bestTime = timeElapsed;
			PlayerPrefs.SetFloat ("BestTime",bestTime);
			beatBestTime = true;
			newRecordSound.Play ();
		}

		backgroundSound.Stop ();
		gameOverSound.Play ();
	}
	
	// Update is called once per frame
	void Update () {
		if (!gameStarted && Time.timeScale == 0) 
		{
			if (Input.anyKeyDown) 
			{
				timeManager.ManipulateTime (1, 1f);
				ResetGame ();
			}
		}

		if (!gameStarted) {
			blinkTime++;
			if (blinkTime % 48 == 0) {
				blink = !blink;
			}
			continueText.canvasRenderer.SetAlpha (blink ? 0 : 1);

			var textColor = beatBestTime?"#FF0":"#FFF";

			scoreText.text = "Time: " + FormatTime (timeElapsed) + "\n<color="+textColor+">Best: " + FormatTime (bestTime)+"</color>";


		} else {
			timeElapsed += Time.deltaTime;
			scoreText.text = "Time: " + FormatTime (timeElapsed);
		}
	}

	void ResetGame(){
		spawner.active = true;
		player = GameObjectUtil.Instantiate (playerPrefab, new Vector3 (0,(Screen.height/PiexelPerfectCamera.pixelToUnit)/2,0));
		var playerDestroyScript = player.GetComponent<DestroyOffscreen> ();
		playerDestroyScript.DestroyCallBack += OnPlayerKilled;
		gameStarted = true;
		continueText.canvasRenderer.SetAlpha (0);
		timeElapsed = 0;
		beatBestTime = false;

		effectSound.Play ();
		StartCoroutine (EnemyGenerator ());
	}

    IEnumerator EnemyGenerator(){
	    yield return new WaitForSeconds (1);
		backgroundSound.Play ();
	}


	string FormatTime(float value)
	{
		TimeSpan t = TimeSpan.FromSeconds (value);
		return string.Format("{0:D2}:{1:D2}",t.Minutes,t.Seconds);
	}
}
