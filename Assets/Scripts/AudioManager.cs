using System;
using System.Security.Cryptography;
using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public enum AudioTrack
    {
        Park,
        RabbitHole,
        MushroomForest,
        ParkBig
    }
    private enum FadeDirection
    {
        In,
        Out
    }

    public float FadeSpeed;

    public AudioSource[] ParkSources;
    public AudioSource[] RabbitHoleSources;
    public AudioSource[] MushroomForestSources;
    public AudioSource[] ParkBigSources;

    private AudioTrack _currentTrack;

    private int _currentSourceIndex;

	public void Start()
    {
        _currentTrack = AudioTrack.RabbitHole;
        _currentSourceIndex = 0;

        StartCoroutine("PlayPark");
	}
	
	public void Update()
    {
        /*
        if (Input.GetKeyDown(KeyCode.O))
            StartCoroutine("PlayPark");
        if (Input.GetKeyDown(KeyCode.P))
            StartCoroutine("PlayRabbitHoe");
        */

        if (_currentTrack == AudioTrack.MushroomForest) 
            UpdateMushroomForestTrack();
    }

    public void UpdateMushroomForestTrack()
    {
        if (_currentSourceIndex >= MushroomForestSources.Length - 1) return;

        var timeSamples = MushroomForestSources[_currentSourceIndex].timeSamples;
        var samples = MushroomForestSources[_currentSourceIndex].clip.samples;

        if (timeSamples < samples) return;

        MushroomForestSources[_currentSourceIndex].Stop();
        _currentSourceIndex++;
        MushroomForestSources[_currentSourceIndex].Play();
    }

    public IEnumerator PlayPark()
    {
        if (_currentTrack == AudioTrack.Park)
            yield break;

        _currentTrack = AudioTrack.Park;

        foreach (var audioClip in ParkSources)
        {
            audioClip.Play();
        }

        var crossFading = true;

        while (crossFading)
        {
            crossFading = false;

            foreach (var audioSource in ParkSources)
            {
                Fade(audioSource, FadeDirection.In, FadeSpeed);

                if (audioSource.volume < 1.0f)
                    crossFading = true;
            }

            foreach (var audioSource in RabbitHoleSources)
            {
                Fade(audioSource, FadeDirection.Out, FadeSpeed);

                if (audioSource.volume > 0.0f)
                    crossFading = true;
            }

            yield return null;
        }

        foreach (var audioClip in RabbitHoleSources)
        {
            audioClip.Stop();
        }
    }

    public IEnumerator PlayRabbitHole()
    {
        if (_currentTrack == AudioTrack.RabbitHole) 
            yield break;

        _currentTrack = AudioTrack.RabbitHole;

        foreach (var audioClip in RabbitHoleSources)
        {
            audioClip.Play();
        }

        var crossFading = true;

        while (crossFading)
        {
            crossFading = false;

            foreach (var audioSource in RabbitHoleSources)
            {
                Fade(audioSource, FadeDirection.In, FadeSpeed);

                if (audioSource.volume < 1.0f)
                    crossFading = true;
            }

            foreach (var audioSource in ParkSources)
            {
                Fade(audioSource, FadeDirection.Out, FadeSpeed);

                if (audioSource.volume > 0.0f)
                    crossFading = true;
            }

            yield return null;
        }

        foreach (var audioSource in ParkSources)
        {
            audioSource.Stop();
        }
    }

    public IEnumerator PlayMushroomForest()
    {
        if (_currentTrack == AudioTrack.MushroomForest)
            yield break;

        _currentTrack = AudioTrack.MushroomForest;
        _currentSourceIndex = 0;

        var mushroomForestIntro = MushroomForestSources[_currentSourceIndex];
        mushroomForestIntro.Play();

        var crossFading = true;

        while (crossFading)
        {
            crossFading = false;

            Fade(mushroomForestIntro, FadeDirection.In, FadeSpeed * 4.0f);
            if (mushroomForestIntro.volume < 1.0f)
                crossFading = true;

            foreach (var audioSource in RabbitHoleSources)
            {
                Fade(audioSource, FadeDirection.Out, FadeSpeed * 4.0f);

                if (audioSource.volume > 0.0f)
                    crossFading = true;
            }

            yield return null;
        }

        foreach (var audioSource in RabbitHoleSources)
        {
            audioSource.Stop();
        }
    }

    public IEnumerator PlayParkBig()
    {
        if (_currentTrack == AudioTrack.ParkBig)
            yield break;

        _currentTrack = AudioTrack.ParkBig;

        var mushroomForestSource = MushroomForestSources[_currentSourceIndex];


        foreach (var audioSource in ParkBigSources)
        {
            audioSource.Play();
        }

        var crossFading = true;

        while (crossFading)
        {
            crossFading = false;

            foreach (var audioSource in ParkBigSources)
            {
                Fade(audioSource, FadeDirection.In, FadeSpeed);

                if (audioSource.volume < 1.0f)
                    crossFading = true;
            }

            Fade(mushroomForestSource, FadeDirection.Out, FadeSpeed);
            if (mushroomForestSource.volume > 0f)
                crossFading = true;
            
            yield return null;
        }

        mushroomForestSource.Stop();
    }

    private void Fade(AudioSource audioSource, FadeDirection fadeDirection, float fadeSpeed)
    {
        var isFading = fadeDirection == FadeDirection.In ? (audioSource.volume < 1.0f) : (audioSource.volume > 0.0f);
        if (isFading)
        {
            var speed = fadeDirection == FadeDirection.In ? fadeSpeed : -FadeSpeed;
            audioSource.volume = audioSource.volume + speed * Time.deltaTime;
        }
    }

    public void StartPlaying(AudioTrack audioTrack)
    {
        switch (audioTrack)
        {
            case AudioTrack.Park:
                StartCoroutine("PlayPark");
                break;
                
            case AudioTrack.RabbitHole:
                StartCoroutine("PlayRabbitHole");
                break;

            case AudioTrack.MushroomForest:
                StartCoroutine("PlayMushroomForest");
                break;

            case AudioTrack.ParkBig:
                StartCoroutine("PlayParkBig");
                break;
        }
    }

}
