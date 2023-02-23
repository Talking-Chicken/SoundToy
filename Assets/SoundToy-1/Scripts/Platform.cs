using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour {
    AudioSource audioSource;
    private bool isHited = false;

    //Part III Adding synth
    PXStrax pxStrax;

    [SerializeField]
    float midiNote = 100.0f;

    float[] d = new float[100];

    //getters & setters
    public bool IsHited {get=>isHited;set=>isHited=value;}

    private void Awake() {
        pxStrax = GetComponent<PXStrax>();//Part III
        audioSource = GetComponent<AudioSource>();
        for(int i = 0; i < d.Length; i++) {
            d[i] = Random.Range(100, 900);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Ball")) {
             //Part III
            audioSource.pitch = 0.5f + VisualManager.Instance.bounceCount * 0.01f;
            // audioSource.pitch = 1 + VisualManager.Instance.bounceCount * 0.01f;
            audioSource.Play();

            IsHited = true;
            // gameObject.layer = 6;
            pxStrax.KeyOn(midiNote);

            if (VisualManager.Instance.CurrentMode == VisualManager.Mode.Free) {
                gameObject.layer = 6;
            }
        }
    }

    
}
