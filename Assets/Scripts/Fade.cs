using UnityEngine;
using System.Collections;

public class Fade : MonoBehaviour
{
    public float FadeSpeed;

    [HideInInspector]
    public bool IsFading;

    private AudioSource _audioClip;

	public void Start()
	{
	    IsFading = false;

	    _audioClip = GetComponent<AudioSource>();
	}
	
	public void FixedUpdate()
	{
	    if (IsFading && _audioClip.volume < 1.0f)
	    {
	        _audioClip.volume += FadeSpeed * Time.fixedDeltaTime;
	    }
	}
}
