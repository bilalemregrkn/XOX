using UnityEngine;

public class AudioManager : MonoBehaviour
{
	public static AudioManager Instance { get; private set; }
	
	[SerializeField] private AudioSource source;

	private void Awake()
	{
		Instance = this;
	}

	public void PlaySFX(AudioClip clip,float volume = 1)
	{
		if(clip)
			source.PlayOneShot(clip,volume);
	}
}