using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioSource efectosSource; 

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void ReproduirSoUncop(AudioClip clip)
    {
        if (clip != null && efectosSource != null)
        {
            efectosSource.PlayOneShot(clip);
        }
    }
    public void ReproduirSoEnBucle(AudioClip clip)
    {
        if (clip != null && efectosSource != null)
        {
            efectosSource.clip = clip; 
            efectosSource.loop = true;  
            efectosSource.Play();
        }
    }
    public void AturarSo()
    {
        if (efectosSource != null && efectosSource.isPlaying)
        {
            efectosSource.Stop();
        }
    }
}
