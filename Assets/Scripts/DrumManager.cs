using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Beat;

public class DrumManager : MonoBehaviour
{
    [SerializeField]
    Clock clock;

    SpriteRenderer spriteRenderer;

    [SerializeField]
    Beat.TickValue tickValue;

    [SerializeField] AudioSource audioSource;

    bool pleaseChange = false;

    #region Delegate

    private void OnEnable() {
        clock.Beat += OnBeat;
        clock.Eighth += OnBeat;
    }
    private void OnDisable() {
        clock.Beat -= OnBeat;
        clock.Eighth -= OnBeat;
    }

    void OnBeat(Beat.Args beatArgs) {
        if(tickValue == beatArgs.BeatVal) {
            ReactAction();
        }
    }

    #endregion

    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }


    void ReactAction() {
        pleaseChange = true;
    }

    void ChangeColor() {
        pleaseChange = false;
        spriteRenderer.color = new Color(Random.Range(0.0f, 1.0f),
            Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
    }

    private void Update() {
        if (pleaseChange) {
            if (!VisualManager.Instance.changedOnThisBeat) {
                if (!audioSource.isPlaying)
                    audioSource.Play();
            }
            // ChangeColor();
            
        }
    }
}
