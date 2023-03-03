using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Beat;
using TMPro;

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
    [SerializeField] Beat.TickValue quarterValue, eighthValue, quarterTripletValue;
    bool pleaseChangeQuarter, pleaseChangeEighth, pleaseChangeQuarterTriplet, isUpbeat = false;
    public bool quarterChangedOnThisBeat = false, eighthChangedOnThisBeat = false, quarterTripletChangedOnThisBeat = false;
    public enum RhythmType {Quarter, Eighth, QuarterTriplet}
    public RhythmType CurrentRhythm = RhythmType.Quarter;

    [Header("Platform")]
    public List<Platform> Platforms = new List<Platform>();
    public int CurrentPlatformIndex = 0;

    [Header("Mode")]
    public Mode CurrentMode = Mode.Free;
    public enum Mode {Beat, Free}
    public TextMeshProUGUI ModeText;
    

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
        clock.QuarterTriplet += OnBeat;
    }
    private void OnDisable() {
        clock.Beat -= OnBeat;
        clock.Eighth -= OnBeat;
        clock.QuarterTriplet -= OnBeat;
    }

    void Start()
    {
        globalVolume.profile.TryGet<Bloom>(out bloom);
        globalVolume.profile.TryGet<ChromaticAberration>(out chromaticAberration);
        currentState = visualState0;
    }

    
    void Update()
    {
        //detect Mode
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow)) {
            if (CurrentMode == Mode.Free) {
                CurrentMode = Mode.Beat;
            } else {
                CurrentMode = Mode.Free;
                CurrentRhythm = RhythmType.Quarter;
            }
            
            ModeText.text = "Mode: "+CurrentMode.ToString();
        }
            


        comboTimer = Mathf.Max(comboTimer - Time.deltaTime, 0.0f);

        if (comboTimer <= 0.0f && !isBallInside) {
            bounceCount = 0;
            
            lerpTime = Mathf.Min(lerpTime + Time.deltaTime, maxLerpTime);
            //lerp the effect back
            bloom.intensity.value = Mathf.Lerp(bloom.intensity.value, 0.0f, lerpTime/maxLerpTime);
            chromaticAberration.intensity.value = Mathf.Lerp(chromaticAberration.intensity.value, 0.0f, lerpTime/maxLerpTime);
            globalLight.intensity = Mathf.Lerp(globalLight.intensity, 0.0f, lerpTime/maxLerpTime);

            if (bloom.intensity.value <= 0.1f)
                foreach (Platform platform in FindObjectsOfType<Platform>())
                    platform.gameObject.layer = 7;

        } else {
            bloom.intensity.value = Mathf.Min(bounceCount * 0.07f, 0.39f);
            chromaticAberration.intensity.value = Mathf.Min(bounceCount * 0.1f + 0.1f, 1.0f);
            globalLight.intensity = Mathf.Min(bounceCount * 0.05f, 1.5f);
        }

        #region free mode
        if (CurrentMode == Mode.Free) {
            if (pleaseChangeQuarter) {
                if (!quarterChangedOnThisBeat) {
                    isUpbeat = !isUpbeat;

                    globalLight.color = new Color (Random.Range(0, 255),
                                                Random.Range(0,255),
                                                Random.Range(0, 255));
                    quarterChangedOnThisBeat = true;
                }
            }
        }
        #endregion

        #region beat Mode
        if (CurrentMode == Mode.Beat) {
            if (Platforms.Count % 4 == 0 && Platforms.Count > 0) { //eighth
                CurrentRhythm = RhythmType.Eighth;
                if (pleaseChangeEighth) {
                    if (!eighthChangedOnThisBeat) {
                        isUpbeat = !isUpbeat;

                        globalLight.color = new Color (Random.Range(0, 255),
                                                    Random.Range(0,255),
                                                    Random.Range(0, 255));
                        eighthChangedOnThisBeat = true;

                        if (CurrentPlatformIndex <= 2)
                            CurrentPlatformIndex++;
                        else
                            CurrentPlatformIndex = 0;

                        //changing layer of each platforms
                        int loopIndex = CurrentPlatformIndex;
                        for (int i = 0; i < Platforms.Count; i++) {
                            if (i == loopIndex) {
                                Platforms[i].gameObject.layer = 6;
                                loopIndex += 4;
                            } else {
                                Platforms[i].gameObject.layer = 7;
                            }
                        }
                    }
                }
            } else if (Platforms.Count % 3 == 0 && Platforms.Count > 0) { //Triplet 
                CurrentRhythm = RhythmType.QuarterTriplet;
                if (pleaseChangeQuarterTriplet) {
                    if (!quarterTripletChangedOnThisBeat) {
                        isUpbeat = !isUpbeat;

                        globalLight.color = new Color (Random.Range(0, 255),
                                                    Random.Range(0,255),
                                                    Random.Range(0, 255));
                        quarterTripletChangedOnThisBeat = true;

                        if (CurrentPlatformIndex <= 1)
                            CurrentPlatformIndex++;
                        else
                            CurrentPlatformIndex = 0;

                        //changing layer of each platforms
                        int loopIndex = CurrentPlatformIndex;
                        for (int i = 0; i < Platforms.Count; i++) {
                            if (i == loopIndex) {
                                Platforms[i].gameObject.layer = 6;
                                loopIndex += 3;
                            } else {
                                Platforms[i].gameObject.layer = 7;
                            }
                        }
                    }
                }
            } else { //quater
                CurrentRhythm = RhythmType.Quarter;
                if (pleaseChangeQuarter) {
                    if (!quarterChangedOnThisBeat) {
                        isUpbeat = !isUpbeat;

                        globalLight.color = new Color (Random.Range(0, 255),
                                                    Random.Range(0,255),
                                                    Random.Range(0, 255));
                        quarterChangedOnThisBeat = true;

                        if (CurrentPlatformIndex <= 0)
                            CurrentPlatformIndex++;
                        else
                            CurrentPlatformIndex = 0;

                        //changing layer of each platforms
                        for (int i = 0; i < Platforms.Count; i++) {
                            if (i % 2 == CurrentPlatformIndex) {
                                Platforms[i].gameObject.layer = 6;
                            } else {
                                Platforms[i].gameObject.layer = 7;
                            }
                        }
                    }
                }
            }
        }
        #endregion

        // if (isUpbeat) {
        //     if (CurrentMode == Mode.Beat)
        //         globalLight.intensity += 0.2f;
        // } else {
        //     if (CurrentMode == Mode.Beat)
        //         globalLight.intensity = Mathf.Max(globalLight.intensity-0.2f, 0.0f);
        // }
        
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
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
        if(quarterValue == beatArgs.BeatVal) {
            ReactActionQuarter();
        }

        if (eighthValue == beatArgs.BeatVal) {
            ReactActionEighth();
        }

        if (quarterTripletValue == beatArgs.BeatVal) {
            ReactActionQuarterTriplet();
        }
    }

    void ReactActionQuarter() {
        quarterChangedOnThisBeat = false;
        pleaseChangeQuarter = true;
    }

    void ReactActionEighth() {
        eighthChangedOnThisBeat = false;
        pleaseChangeEighth = true;
    }

    void ReactActionQuarterTriplet() {
        quarterTripletChangedOnThisBeat = false;
        pleaseChangeQuarterTriplet = true;
    }
}
