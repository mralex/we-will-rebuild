using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveController : MonoBehaviour {

    public static WaveController instance;

    public WingSpawner SpawnRed;
    public WingSpawner SpawnBlue;
    public float TimeBetweenWaves = 20;
    public float TimeBetweenWings = 2;
    public float Wings = 5;

    public Action<int> OnSpawn;

    private void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    void Start () {
        StartCoroutine(TmpSpawner());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void SpawnWing()
    {
        int count = UnityEngine.Random.Range(2, 9);
        OnSpawn(count);
    }



    IEnumerator TmpSpawner()
    {
        while(true)
        {
            yield return new WaitForSeconds(TimeBetweenWaves);

            for (int i = 0; i < Wings; i++)
            {
                SpawnWing();
                yield return new WaitForSeconds(TimeBetweenWings);
            }
        }
    }
}
