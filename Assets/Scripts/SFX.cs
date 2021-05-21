using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (AudioSource))]
public class SFX : MonoBehaviour {
    private static AudioSource source;

    void Start () {
        source = GetComponent<AudioSource> ();
    }

    public static void PlaySound (string path) {
        try {
            source.PlayOneShot ((AudioClip) Resources.Load ($"Audio/SFX/{path}"));
        } catch { }
    }
}