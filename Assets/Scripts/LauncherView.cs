using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LauncherView : MonoBehaviour {

    public bool IsLive = false;
    public float FireRate = 2;
    float NextFire;

    public Transform Pods;
    public float MinPodRotation = 0;
    public float MaxPodRotation = 50;

    float podRotationTime;

    public float RotationSpeed = 1.5f;
    public Transform[] RocketSpawns;

	// Use this for initialization
	void Start () {
        NextFire = FireRate;
	}
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(transform.up, RotationSpeed * Time.deltaTime);

        podRotationTime += Time.deltaTime;
        float sin = (Mathf.Sin(podRotationTime / (RotationSpeed / 20)) + 1) / 2f;
        Pods.localRotation = Quaternion.Euler(Mathf.Lerp(0, -MaxPodRotation, sin), 0, 0);

        if (IsLive)
        {
            NextFire -= Time.deltaTime;

            if (NextFire <= 0)
            {
                NextFire = FireRate;

                StartCoroutine(FireMissiles());
            }
        }
	}

    IEnumerator FireMissiles()
    {
        GameObject RocketPrefab = Resources.Load<GameObject>("Prefabs/Rocket");
        
        foreach(Transform s in RocketSpawns)
        {
            GameObject r = Instantiate(RocketPrefab, Vector3.zero, Quaternion.identity, transform.parent);
            r.transform.rotation = s.rotation;
            r.transform.position = s.position;

            yield return new WaitForSeconds(0.075f);
        }
    }
}
