using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAudioSegment : MonoBehaviour
{
    public AudioSource audioSource;
    public float startTime; // in seconds
    public float endTime;   // in seconds

    private bool isPlayingSegment = false;

    void Update()
    {
        // Start playing the segment when you press a key (e.g., Space)
        if (Input.GetKeyDown(KeyCode.Space) && !isPlayingSegment)
        {
            audioSource.time = startTime;
            audioSource.Play();
            isPlayingSegment = true;
        }

        // Stop at the end time
        if (isPlayingSegment && audioSource.time >= endTime)
        {
            audioSource.Stop();
            isPlayingSegment = false;
        }
    }
}