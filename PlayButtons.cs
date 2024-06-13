using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayButtons : MonoBehaviour
{

    public AudioSource soundButton;
    public void playThisSound()
    {
        soundButton.Play();
    }
}
