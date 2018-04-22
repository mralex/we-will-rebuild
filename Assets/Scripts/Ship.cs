using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour {

    public GameObject BombPrefab;
    public float Speed = 3;
    public float TTL = 15;
    public bool IsBomber = false;

    public int BombsPerSet = 10;
    public float DelayBeforeBombs = 2;
    public float TimeBetweenBombs = 0.25f;

    Vector3 BombOffset = new Vector3(0, -0.2f, 0);

	// Use this for initialization
	void Start () {

        IsBomber = Random.Range(0f, 1f) > 0.68f;

        StartCoroutine(Timeout());

        if (IsBomber)
            StartCoroutine(BombThings());

        Speed += Random.Range(-0.2f, 0.2f);

	}
	
	// Update is called once per frame
	void Update () {
        transform.position += transform.forward * Speed * Time.deltaTime;
	}

    void DoBomb()
    {
        GameObject b = Instantiate(BombPrefab, Vector3.zero, Quaternion.identity, transform.parent);
        b.transform.position = transform.position + BombOffset;
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject DebrisPrefab = Resources.Load<GameObject>("Prefabs/ShipExplosion");

        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            GameObject d = Instantiate(DebrisPrefab, Vector3.zero, Quaternion.identity, transform.parent);

            d.transform.position = collision.contacts[0].point;

            Destroy(gameObject);

            return;
        }

        if (!collision.collider.gameObject.CompareTag("Ship"))
            return;

        GameObject debris = Instantiate(DebrisPrefab, Vector3.zero, Quaternion.identity, transform.parent);

        debris.transform.position = collision.contacts[0].point;

        Destroy(collision.collider.gameObject);
        Destroy(gameObject);
    }

    IEnumerator Timeout()
    {
        yield return new WaitForSeconds(TTL);

        Destroy(gameObject);
    }

    IEnumerator BombThings()
    {
        while(true)
        {
            yield return new WaitForSeconds(DelayBeforeBombs + Random.Range(-0.5f, 0.5f));

            for (int i = 0; i < BombsPerSet; i++)
            {
                DoBomb();
                yield return new WaitForSeconds(TimeBetweenBombs + Random.Range(-0.1f, 0.1f));
            }
        }
    }
}
