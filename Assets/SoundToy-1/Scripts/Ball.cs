using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    private Rigidbody2D rigidbody;

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
        
    }
}
