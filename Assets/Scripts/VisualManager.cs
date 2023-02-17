using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Beat;

/*this class count the bounce time of the ball and set visual effects based on the number*/
public class VisualManager : Singleton<VisualManager>
{
    public int bounceCount = 0;
    public Light2D globalLight;
    public Bloom bloom;
    public ChromaticAberration chromaticAberration;
    public Volume globalVolume;

    [SerializeField] private float maxComboTimer;
    private float comboTimer = 0.0f;
    [SerializeField] private float maxLerpTime;
    private float lerpTime = 0;
    private bool isBallInside = false;

    [Header("Clock")]
    [SerializeField] Clock clock;
    [SerializeField] Beat.TickValue tickValue;
    bool pleaseChange = false, isUpbeat = false;
    public bool changedOnThisBeat = false;

    //FSM
    private VisualStateBase currentState;
    public VisualState0 visualState0 = new VisualState0();
    public VisualState1 visualState1 = new VisualState1();
    public VisualState2 visualState2 = new VisualState2();
    public VisualState3 visualState3 = new VisualState3(); 

    public void ChangeState(VisualStateBase newState)
    {
        if (currentState != newState) {
            if (currentState != null)
            {
                currentState.LeaveState(this);
            }

            currentState = newState;

            if (currentState != null)
            {
                currentState.EnterState(this);
            }
        }
    }

    private void OnEnable() {
        clock.Beat += OnBeat;
        clock.Eighth += OnBeat;
    }
    private void OnDisable() {
        clock.Beat -= OnBeat;
        clock.Eighth -= OnBeat;
    }

    void Start()
    {
        globalVolume.profile.TryGet<Bloom>(out bloom);
        globalVolume.profile.TryGet<ChromaticAberration>(out chromaticAberration);
        currentState = visualState0;
    }

    
    void Update()
    {
        
        VolumeManager.instance.stack.GetComponent<Bloom>().intensity.value = 100.0f;

        comboTimer = Mathf.Max(comboTimer - Time.deltaTime, 0.0f);

        if (comboTimer <= 0.0f && !isBallInside) {
            bounceCount = 0;
            
            lerpTime = Mathf.Min(lerpTime + Time.deltaTime, maxLerpTime);
            //lerp the effect back
            bloom.intensity.value = Mathf.Lerp(bloom.intensity.value, 0.0f, lerpTime/maxLerpTime);
            chromaticAberration.intensity.value = Mathf.Lerp(chromaticAberration.intensity.value, 0.0f, lerpTime/maxLerpTime);
            globalLight.intensity = Mathf.Lerp(globalLight.intensity, 0.0f, lerpTime/maxLerpTime);

            if (bloom.intensity.value <= 0.01f)
                foreach (Platform platform in FindObjectsOfType<Platform>())
                    platform.gameObject.layer = 7;

        } else {
            if (pleaseChange) {
                bloom.intensity.value = Mathf.Min(bounceCount * 0.07f, 0.9f);
                chromaticAberration.intensity.value = bounceCount * 0.1f + 0.2f;
                globalLight.intensity = bounceCount * 0.07f;
            }
        }

        if (pleaseChange) {
            if (!changedOnThisBeat) {
                isUpbeat = !isUpbeat;
                
                if (isUpbeat) {
                    globalLight.intensity += 0.1f;
                    
                    
                }
                else {
                    globalLight.intensity = Mathf.Max(globalLight.intensity-0.1f, 0.0f);
                }

                globalLight.color = new Color (Random.Range(0, 255),
                                               Random.Range(0,255),
                                               Random.Range(0, 255));
                changedOnThisBeat = true;
            }
        }

    }

    public void addBounceCount() {
        comboTimer = maxComboTimer;
        bounceCount++;
        lerpTime = 0;
    }

    void OnTriggerStay2D(Collider2D collider) {
        if (collider.CompareTag("Ball"))
            isBallInside = true;
    }

    void OnTriggerExit2D(Collider2D collider) {
        if (collider.CompareTag("Ball")) {
            Destroy(collider.gameObject);
            isBallInside = false;
        }
    }

    void OnBeat(Beat.Args beatArgs) {
        if(tickValue == beatArgs.BeatVal) {
            ReactAction();
        }
    }

    void ReactAction() {
        changedOnThisBeat = false;
        pleaseChange = true;
    }
}
