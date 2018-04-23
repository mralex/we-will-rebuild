using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveController : MonoBehaviour {

    public float InitialWaveTime = 90;
    public float MinWaveTime = 10;

    public float NextWaveDivisor = 0.8f;
    public float NextWaveTime;

    public static WaveController instance;

    public WingSpawner SpawnRed;
    public WingSpawner SpawnBlue;
    public float TimeBetweenWaves = 20;
    public float TimeBetweenWings = 2;
    public float Wings = 3;

    public int WaveCount = 1;

    public Action<int, float> OnWaveAlert;
    public Action OnWaveBegin;
    public Action<int> OnWaveEnded;

    public Action<int> OnSpawn;

    private void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    void Start () {
        NextWaveTime = InitialWaveTime;
    }

    // Update is called once per frame
    void Update () {
		
	}

    void SpawnWing()
    {
        int count = UnityEngine.Random.Range(2, 9);
        OnSpawn(count);
    }

    public void EnqueueWave()
    {
        StartCoroutine(WaveSpawner());
    }

    IEnumerator WaveSpawner()
    {
        OnWaveAlert(WaveCount, NextWaveTime);

        yield return new WaitForSeconds(NextWaveTime);

        OnWaveBegin();

        for (int i = 0; i < Wings * WaveCount; i++)
        {
            SpawnWing();
            yield return new WaitForSeconds(TimeBetweenWings);
        }

        yield return new WaitForSeconds(6);

        OnWaveEnded(WaveCount);

        WaveCount++;
        NextWaveTime *= NextWaveDivisor;
        NextWaveTime = Mathf.Floor(Mathf.Max(NextWaveTime, MinWaveTime));
    }
}
