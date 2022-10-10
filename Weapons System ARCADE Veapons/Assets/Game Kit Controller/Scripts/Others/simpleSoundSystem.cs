using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class simpleSoundSystem : MonoBehaviour
{
	public bool soundsEnabled = true;
	public AudioSource mainAudioSource;

	public List<soundCategoryInfo> soundCategoryInfoList = new List<soundCategoryInfo> ();

	public void playRandomSound ()
	{
		if (!soundsEnabled) {
			return;
		}

		int randomCategoryIndex = Random.Range (0, soundCategoryInfoList.Count);

		int randomSound = Random.Range (0, soundCategoryInfoList [randomCategoryIndex].soundInfoList.Count);

		mainAudioSource.PlayOneShot (soundCategoryInfoList [randomCategoryIndex].soundInfoList [randomSound].soundClip);
	}

	public void playRandomSoundFromCategoryByName (string categoryName)
	{
		int currentIndex = soundCategoryInfoList.FindIndex (s => s.categoryName == categoryName);

		if (currentIndex > -1) {
			int randomSound = Random.Range (0, soundCategoryInfoList [currentIndex].soundInfoList.Count);

			mainAudioSource.PlayOneShot (soundCategoryInfoList [currentIndex].soundInfoList [randomSound].soundClip);
		}
	}


	[System.Serializable]
	public class soundCategoryInfo
	{
		public string categoryName;

		public List<soundInfo> soundInfoList = new List<soundInfo> ();
	}

	[System.Serializable]
	public class soundInfo
	{
		public string Name;

		public AudioClip soundClip;
	}
}
