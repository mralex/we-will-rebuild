using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WingSpawner : MonoBehaviour {

	// Use this for initialization
	void Start () {
        WaveController.instance.OnSpawn += DoSpawn;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void DoSpawn(int count)
    {
        float cellWidth = 1.4f;
        float startX = 0 - ((cellWidth * (count - 1)) / 2f);

        for (int i = 0; i < count; i++)
        {
            float x = i * cellWidth;
            SpawnAttacker(startX + x);
        }
    }

    void SpawnAttacker(float x)
    {
        GameObject p = Resources.Load<GameObject>("Prefabs/Ship1");
        GameObject s = Instantiate(p, Vector3.zero, Quaternion.identity, transform);

        s.transform.position = transform.position + new Vector3(x + Random.Range(-0.3f, 0.3f), Random.Range(-0.2f, 0.2f), Random.Range(-0.3f, 0.3f));
        s.transform.rotation = transform.rotation;
    }
}
