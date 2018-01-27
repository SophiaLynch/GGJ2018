﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    //Rotation and punch speed
    public float rotationSpeed = 50f;
    public float punchForce = 10f;

    //Timers
    public float animationTime = .4f;
    public float recoverTime = .2f;
    private float animationReset;
    private float recoverReset;

    [HideInInspector]
    public int playerNum;
    private float rotationAmount = 0;

    private GameObject cameraObj;
    private List<GameObject> extraCameras;

    //Whether the player is punching ornot
    private bool punching = false;
    private bool blocking = false;

	// Use this for initialization
	void Start () {
        //Set the reset timers
        animationReset = animationTime;
        recoverReset = recoverTime;

        cameraObj = transform.Find("Camera").gameObject;
	}
	
	// Update is called once per frame
	void Update () {
        //Test for inputes
        InputCommands();
	}

    public void ResetCamera(GameObject obj = null)
    {
        if (obj == null) { obj = gameObject; }
        cameraObj.transform.SetParent(obj.transform);
        cameraObj.transform.localPosition = Vector3.zero;
        cameraObj.transform.localRotation = Quaternion.identity;
    }

    private void InputCommands()
    {
        //Get rotation inpute
        if (Input.GetAxis("Player" + playerNum + "Move") < 0) { RotateLeft(); }
        if (Input.GetAxis("Player" + playerNum + "Move") > 0) { RotateRight(); }

        if (animationTime <= 0 && recoverTime <= 0)
        {
            GetComponent<Rigidbody>().velocity = Vector3.zero;

            //Make the player punch
            if (Input.GetAxis("Player" + playerNum + "Action") < 0) { StartPunch(); }
            //Make the player block
            if (Input.GetAxis("Player" + playerNum + "Action") > 0) { StartBlock(); }

        //Start the recover timer
        } else if (animationTime <= 0)
        {
            recoverTime -= Time.deltaTime;

            //Set the the player to not do anything and to not move
            punching = false;
            blocking = false;
            GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
        else
        {
            //Try to run the two player actions
            Punch();
            Block();

            //Start the animation timer, and reset the recover timer
            animationTime -= Time.deltaTime;
            recoverTime = recoverReset;
        }
    }

    /// <summary>
    /// Rotate the object left and right
    /// </summary>
    public void RotateLeft()
    {
        //Calc and set the rotation
        rotationAmount -= rotationSpeed * Time.deltaTime;
        transform.localRotation = Quaternion.Euler(0, rotationAmount, 0);
    }
    public void RotateRight()
    {
        //Calc and set the rotation
        rotationAmount += rotationSpeed * Time.deltaTime;
        transform.localRotation = Quaternion.Euler(0, rotationAmount, 0);
    }

    /// <summary>
    /// Start functions for blocking and punching
    /// </summary>
    public void StartPunch()
    {
        punching = true;

        //Reset the timers
        animationTime = animationReset;
        recoverTime = recoverReset;
    }
    private void Punch()
    {
        //Make sure the player isn't blocking and is punching
        if (blocking || !punching) { return; }

        GetComponent<Rigidbody>().velocity = transform.forward * punchForce;
    }
    public void StartBlock()
    {
        blocking = true;

        //Reset the timers
        animationTime = animationReset * 2;
        recoverTime = recoverReset * .8f;
    }
    private void Block()
    {
        //Make sure the player isn't punching and is blocking
        if (punching || !blocking) { return; }

        if (animationTime > 0 && blocking)
        {

        }
    }

    private void OnCollisionEnter(Collision col)
    {
        //If a player doesn't hit you ignore it
        if (!col.gameObject.name.Contains("Player")) { return; }

        //If the 
        if (blocking) {
            col.gameObject.GetComponent<Player>().recoverTime = recoverReset * 3;
            col.gameObject.GetComponent<Player>().animationTime = 0;

            GetComponent<Rigidbody>().velocity = Vector3.zero;
            return;
        }

        //If the player that hit you is punching teleport you down
        if (col.gameObject.GetComponent<Player>().punching)
        {
            transform.position += Vector3.down * 10;

            ResetCamera(col.gameObject);
        }
    }
}