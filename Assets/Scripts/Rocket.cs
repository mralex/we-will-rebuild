using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour {

    public float Speed = 10;
    public float TTL = 5;
    public ParticleSystem Explosion;

    bool isMoving = true;

	// Use this for initialization
	void Start () {
        StartCoroutine(Timeout());
	}
	
	// Update is called once per frame
	void Update () {
        if (isMoving)
            transform.position += transform.forward * Speed * Time.deltaTime;
	}

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            Debug.Log("HIT!");

            Destroy(collision.gameObject);

            GameController.instance.city.damageDealt += 50;
        }
        
        TTL = 0.1f;
        StartCoroutine(Timeout());
    }

    IEnumerator Timeout()
    {
        yield return new WaitForSeconds(TTL);

        isMoving = false;
        Explosion.Play();

        yield return new WaitForSeconds(0.5f);

        Destroy(gameObject);
    }
}
