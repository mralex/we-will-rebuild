using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipExplosion : MonoBehaviour {

    public float TTL = 0.75f;

	// Use this for initialization
	void Start () {
        StartCoroutine(Timeout());
	}
	
	void SpawnDebris()
    {
        GameObject d = Resources.Load<GameObject>("Prefabs/ShipDebris");
        GameObject i = Instantiate(d, transform.position, Quaternion.identity, transform.parent);
    }

    IEnumerator Timeout()
    {
        yield return new WaitForSeconds(TTL / 2);

        SpawnDebris();

        yield return new WaitForSeconds(TTL / 2);

        Destroy(gameObject);
    }
}
