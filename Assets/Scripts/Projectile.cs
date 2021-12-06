using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    Rigidbody2D rigidbody2d;


    void Awake()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
    }

    public void Launch(Vector2 direction, float force)
    {
        rigidbody2d.AddForce(direction * force);
    }

    void Update()
    {
        if (transform.position.magnitude > 1000.0f)
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {

        EnemyController e = other.collider.GetComponent<EnemyController>();
        HardEnemyController a = other.collider.GetComponent<HardEnemyController>();
        GoblinScript g= other.collider.GetComponent<GoblinScript>();

        if (e != null)
        {
            e.Fix();


        }
        if (a != null)
        {
            a.Fix();


        }
        if(g !=null)
        {
            g.Fix();
        }

        Destroy(gameObject);
    }
}
