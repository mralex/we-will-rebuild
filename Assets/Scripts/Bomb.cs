using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour {

    public ParticleSystem Explosion;
    public float Damage = 32;
    public float TTL = 0.5f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter(Collision collision)
    {
        if (Explosion != null)
            Explosion.Play();

        StartCoroutine(Timeout());

        GameController.instance.OnBomb(transform.position, Damage);
    }

    IEnumerator Timeout()
    {
        yield return new WaitForSeconds(TTL);

        Destroy(gameObject);
    }
}
