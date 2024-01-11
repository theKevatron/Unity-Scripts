using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;
using UnityEngine;

/// <summary>
///  This class is used to create an audio container which groups similar sounds together, which when called
///  on will choose one clip at random to play in an attempt to reduce repetativeness of sound effects
/// </summary>
[DisallowMultipleComponent]
[ExecuteInEditMode]
public class AudioContainer : MonoBehaviour
{
    public AudioSource[] audioSource;

    private void OnEnable()
    {
        audioSource = null;
        audioSource = GetComponentsInChildren<AudioSource>();
    }

    public void GetRandomAudioClip(Vector3 position, float volume)
    {
        AudioClip clip;

        if (audioSource?.Length >= 1)
        {
            int randomIndex = UnityEngine.Random.Range(0, (audioSource.Length - 1));
            clip = audioSource[randomIndex].clip;
            AudioSource.PlayClipAtPoint(clip, position, volume);
        }
    }
}
