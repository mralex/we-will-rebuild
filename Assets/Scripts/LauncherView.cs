using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LauncherView : MonoBehaviour {

    public Transform Pods;
    public float MinPodRotation = 0;
    public float MaxPodRotation = 50;


    public float RotationSpeed = 1.5f;
    public Transform[] RocketSpawns;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(transform.up, RotationSpeed * Time.deltaTime);
	}
}
