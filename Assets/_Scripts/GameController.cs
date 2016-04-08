using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class GameController : MonoBehaviour 
{
	public GameObject[] hazard;
	public Vector3 BSpawnValues;
	public Vector3 YSpawnValues;
	public Vector3 RSpawnValues;
	private Vector3 SpawnPosition;
	public float Wait1;
	public float Wait2;
	public float Wait3;

	public Slider TimeSlider;

	//background busic holder\

	private AudioSource backgroundMusic;

	//in the game counter
	private bool InGame = false;

	public GUIText ScoreText;
	private int Score;

	public GUIText TimeText;
	private float MusicTime;
	private float TotalTime;
	public  int CountNo;
	//Next Stage;
	public AudioClip[] NextSong;
	private  int Level;
	public GUIText LeveLText;

	public Button Continue;




	void Start()
	{
		Level = 0;
		TimeSlider.minValue = 0;
		CountNo = 0;
		MusicTime = 0;
		Score = 0;
		UpdateScore ();
		UpdateTime ();
		/*StartCoroutine (SpawnWaves ());
		backgroundMusic = GetComponent<AudioSource> ();
		InGame = true;
		TotalTime = backgroundMusic.clip.length;
		//Debug.Log ("total------------- "+ TotalTime);
		TimeSlider.maxValue = TotalTime;
*/
		stage ();

	}

	public void OnGUI(){
		float x = TotalTime - 3;
		if (MusicTime >= x) {
			InGame = false;
		}


		if (!backgroundMusic.isPlaying) {
			InGame= false;
			if (GUI.Button (new Rect (Screen.width / 2 - 50, Screen.height / 2 - 10, 100, 50), "Next Stage")) {
			
				stage ();

			}
		}
	}
	void Update (){	
		UpTime (backgroundMusic.time);
		if (Input.GetKeyDown (KeyCode.E)) {
		
			InGame = false;
			backgroundMusic.Stop ();
			PlayerPrefs.SetInt ("Score", Score);
			Debug.Log (Score);
		}
		//	Debug.Log (backgroundMusic.time);
	
			//change sence and display the result and animation 
			//InGame = false;
			//OnGUI();

			/*if (Input.GetKeyDown (KeyCode.R)) {
				Level += 1;
				CountNo = Random.Range (0, 13);

				backgroundMusic.clip = NextSong [CountNo];
				backgroundMusic.Play ();
				InGame = true; 
				StartCoroutine (SpawnWaves ());
				TotalTime = backgroundMusic.clip.length;
				TimeSlider.maxValue = TotalTime;
			}*/



		//}
		TimeSlider.value = backgroundMusic.time;
	}
	void stage()
	{
		backgroundMusic = GetComponent<AudioSource> ();
			CountNo = Random.Range (0, 13);
			Level +=1;
			backgroundMusic.clip = NextSong[CountNo];
			backgroundMusic.Play();
			InGame = true; 
			StartCoroutine (SpawnWaves ());
			TotalTime = backgroundMusic.clip.length;
			TimeSlider.maxValue = TotalTime;
		PlayerPrefs.SetInt ("Score", Score);
		}
	IEnumerator SpawnWaves()
	{	
		Quaternion SpawnRotation = Quaternion.identity;
		yield return new WaitForSeconds (Wait1);
		while (InGame) {
		//	for(int i = 0; i<10;i++){
				switch ((int)Mathf.Floor (Random.Range (0,3))) {
				case 0://Blue Ball
					SpawnPosition = new Vector3 (BSpawnValues.x, BSpawnValues.y, BSpawnValues.z);
					Instantiate (hazard [0], SpawnPosition, SpawnRotation);
					break;
				case 1: //Yellow Ball
					SpawnPosition = new Vector3 (YSpawnValues.x, YSpawnValues.y, YSpawnValues.z);
					Instantiate (hazard [1], SpawnPosition, SpawnRotation);
					break;
				case 2: //Red Ball
					SpawnPosition = new Vector3 (RSpawnValues.x, RSpawnValues.y, RSpawnValues.z);
					Instantiate (hazard [2], SpawnPosition, SpawnRotation);
					break;
				}
			//	yield return new WaitForSeconds(Wait2);
		//	}
			yield return new WaitForSeconds(Wait3);
		
		}
	}
	public void UpTime(float newTime)
	{
		MusicTime = backgroundMusic.time;
		UpdateTime ();
	}

	void UpdateTime()
	{

		TimeText.text = "Time: " + MusicTime;
		LeveLText.text = "Level: " + Level;

	}
	public void AddScore(int newScoreValue)
	{
		Score += newScoreValue;
		UpdateScore();
	}
	void UpdateScore()
	{
		ScoreText.text = "Score: " + Score;
	}


}
