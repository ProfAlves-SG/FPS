using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletClass : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float speed;
    [SerializeField] private float lifetime = 5f;

    [SerializeField] private GameObject impactEffectPrefab;


    void Start()
    {
        Launch();
        Destroy(gameObject, lifetime);
    }

    private void Launch()
    {
        rb.AddForce(transform.forward * speed, ForceMode.VelocityChange);
    }

    private void OnCollisionEnter(Collision collision)
    {
        SpawnImpact(collision);
        Destroy(gameObject);
    }

    private void SpawnImpact(Collision collision)
    {
        Vector3 point = transform.position;
        Vector3 normal = -transform.forward;

        if(collision.contactCount > 0)
        {
            ContactPoint contact = collision.GetContact(0);
            point = contact.point;
            normal = contact.normal;
        }

        Quaternion rotation = Quaternion.FromToRotation(Vector3.up, normal);
        Instantiate(impactEffectPrefab, point + normal * 0.02f, rotation);
    }
}
