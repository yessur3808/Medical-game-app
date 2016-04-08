using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class HighScoreTable : MonoBehaviour {
//	public string Name;
	public GUIText[] ScoreText = new GUIText[10];
	public int Score;

	// Use this for initialization
	/*void Start () {
		Score = PlayerPrefs.GetInt ("Score");
		ScoreText.text = "Score:" + Score;

	}*/


	/*
	void SaveScore(){

		if (InGame==false)
		{
			PlayerPrefs.SetString("Name", Score );
			print ("save");
		}


	}
*/
	// a = new old ; b = small one
	void Start(){
	

			SaveScore ();
	}

	void Swap(int a , int b)
	{
		int c;
		c = a;
		a = b;
		b = c; 

	}
	void Compare (int[] ScoreArray)
	{
		for (int i = 0; i<ScoreArray.Length; i++) {
			for(int j = 1; j < ScoreArray.Length;j++){
				if(j>i)
				{
					Swap (i,j);
				}
			}

		}

	}
	void SaveScore()
	{
		for (int i =0; i<10; i++) {
			if(ScoreText[i].text == null)
			{
				Score = PlayerPrefs.GetInt("Score");
				ScoreText[i].text ="Score" + Score;
			}
		}

	}
}
