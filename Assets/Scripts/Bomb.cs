using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour {

    public ParticleSystem Explosion;
    public float Damage = 32;
    public float TTL = 0.5f;

    AudioSource Audio;

	// Use this for initialization
	void Start () {
        Audio = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter(Collision collision)
    {
        if (Explosion != null)
        {
            Explosion.Play();

            if (Random.Range(0f, 1f) > 0.4f)
            {
                Audio.pitch += Random.Range(-0.25f, 0.35f);
                Audio.Play();
            }
        }
            

        StartCoroutine(Timeout());

        GameController.instance.OnBomb(transform.position, Damage);
    }

    IEnumerator Timeout()
    {
        yield return new WaitForSeconds(TTL);

        Destroy(gameObject);
    }
}
