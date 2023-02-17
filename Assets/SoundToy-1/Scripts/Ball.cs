using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    private Rigidbody2D rigidbody;
    [SerializeField] private float maxCooldown;
    private float cooldownTimer = 0.0f; 

    //getters & setters
    public Rigidbody2D RigidBody {get=>rigidbody;private set=>rigidbody=value;}

    void Awake() {
        RigidBody = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        RigidBody = GetComponent<Rigidbody2D>();
        if (RigidBody == null) Debug.Log("rigidbody is null");
    }

    
    void Update()
    {
        cooldownTimer = Mathf.Max(cooldownTimer - Time.deltaTime, 0.0f);
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Platform")) {
            if (cooldownTimer <= 0.0f) {
                VisualManager.Instance.addBounceCount();
            }
        }
    }

    
}
