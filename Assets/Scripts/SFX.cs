using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (AudioSource))]
public class SFX : MonoBehaviour {
    static AudioSource source;

    public static void PlaySound (string path) {
        try {
            source.PlayOneShot ((AudioClip) Resources.Load ($"Audio/SFX/{path}"));
        } catch { }
    }

    void Start () {
        source = GetComponent<AudioSource> ();
    }
}