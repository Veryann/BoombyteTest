using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioClip ballCollisionClip;
    [SerializeField] AudioClip ballHitClip;
    [SerializeField] Slider volumeSlider;
    public AudioSource audioSource;

    public void ChangeVolume(float sliderValue)
    {
        audioSource.volume = sliderValue;
    }
    
    public void PlayBallShot()
    {
        audioSource.clip = ballHitClip;
        audioSource.Play();
    }

    public IEnumerator PlayBallCollision(float volume)
    {
        float prevVolume = audioSource.volume;
        audioSource.volume = audioSource.volume * volume;
        audioSource.clip = ballCollisionClip;
        audioSource.Play();
        yield return new WaitForSeconds(ballCollisionClip.length);
        audioSource.volume = prevVolume;
    }

}
