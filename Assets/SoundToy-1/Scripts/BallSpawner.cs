using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Beat;


//Part I: Spawning ball drop on click/keyboard press
//Part II: Spawning ball on beat using clock
    
public class BallSpawner : MonoBehaviour {

    [SerializeField]
    Clock clock;
    Args beatArgs;
    public Transform ball;

    [SerializeField]
    TickValue tickValue = TickValue.Quarter;

    bool pleaseSpawn;

    [SerializeField]
    int BPM = 120;

    [SerializeField] private Vector2 direction;
    [SerializeField] private float force;

    //getters & setters
    public Vector2 Direction {get=>direction;private set=>direction = value;}

    #region Delegates
    private void OnEnable() {
        //clock.Beat += SpawnBall;
    }
    private void OnDisable() {
        //clock.Beat -= SpawnBall;
    }
    #endregion

    void SpawnBall(Args beatArgs) {
        pleaseSpawn = true;
    }
    void SpawnBall() {
        pleaseSpawn = false;
        Debug.Log("Spawning ball");
        GameObject ballObj = Instantiate(ball, transform.position, Quaternion.identity).gameObject;
        Ball thisBall = ballObj.GetComponent<Ball>();
        if (ballObj == null) Debug.Log("ballObj is null");
        if (thisBall == null)  Debug.Log("thisBall is null");
        if (thisBall.RigidBody == null) Debug.Log("rigidbody of thisBall is null");
        if (Direction == null) Debug.Log("Direction is null");
        thisBall.RigidBody.AddForce(Direction * force);
    }

    void KeyboardInput() {
        if (Input.GetKeyDown(KeyCode.Space))
            SpawnBall();
    }
    private void Update() {
        Direction = ((Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - (Vector2)transform.position).normalized;
        if (pleaseSpawn) {
            SpawnBall();
        }
        KeyboardInput();
        LookAtCursor();
        //if (Input.GetKeyDown(KeyCode.Space)) {
        //    clock.SetBPM(BPM);
        //}
    }

    void LookAtCursor() {
        transform.up = Direction;
    }
}
