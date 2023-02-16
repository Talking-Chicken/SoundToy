using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

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
            bloom.intensity.value = bounceCount * 0.1f;
            chromaticAberration.intensity.value = bounceCount * 0.1f + 0.2f;
            globalLight.intensity = bounceCount * 0.1f;

            
        }

        // switch(bounceCount) {
        //     case 0:
        //         Debug.Log("0");
        //         if (bloom.intensity.value >= 0.1f) bloom.intensity.value = Mathf.Lerp(bloom.intensity.value, 0.0f, lerpTime);
        //         else bloom.intensity.value = 0.0f;

        //         if (globalLight.intensity >= 0.1f) globalLight.intensity = Mathf.Lerp(globalLight.intensity, 0.0f, lerpTime);
        //         else globalLight.intensity = 0.0f;

        //         break;
        //     case 1:
        //         Debug.Log("1");
        //         bloom.intensity.value = Mathf.Lerp(bloom.intensity.value, 0.15f, lerpTime);

        //         globalLight.intensity = Mathf.Lerp(globalLight.intensity, 0.15f, lerpTime);
        //         break;
        //     case 4:
        //         Debug.Log("case 4");
        //         bloom.intensity.value = Mathf.Lerp(bloom.intensity.value, 0.55f, lerpTime + 1.0f);

        //         globalLight.intensity = Mathf.Lerp(globalLight.intensity, 0.5f, lerpTime + 1.0f);
        //         break;
        // }
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
}
