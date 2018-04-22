using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CameraController : MonoBehaviour {
    public static CameraController instance;

    public float speed = 5;
    public Vector2 extents = new Vector2(25, 32);

    public Action<Vector3> OnCursor;

    private void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 motion = Vector3.zero;

        Vector3 p = transform.position;

        motion = Input.GetAxis("Horizontal") * transform.right;
        motion += Input.GetAxis("Vertical") * transform.forward;
        p += motion * Time.deltaTime * speed;

        p.x = Mathf.Clamp(p.x, -extents.x, extents.x);
        p.z = Mathf.Clamp(p.z, -extents.y, extents.y);

        transform.position = p;
	}
}
