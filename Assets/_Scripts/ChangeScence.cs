using UnityEngine;
using System.Collections;

public class ChangeScence : MonoBehaviour {
	
	private AudioSource soundeffect;
	public AudioClip[] clips;
	public static int SoundEffectNo;

	
	void Start(){
		SoundEffectNo = Random.Range (0, 9);
		Debug.Log (SoundEffectNo);
	}

	public void AddSoundEffect(int soundno)
	{
		soundeffect = GetComponent<AudioSource>();
		soundeffect.clip = clips[soundno];
		soundeffect.Play ();
	}
	
	public void ChangeToScence (int sceneToChangeTo) {
		Application.LoadLevel (sceneToChangeTo);
	}
}
