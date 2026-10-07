using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    
    [Header("Bullet")]
    [SerializeField] private float speed = 60f;
    [SerializeField] private float lifetime = 5f;
    
    [SerializeField] private GameObject impactEffectPrefab;
    

    private void Start()
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
        SpawnImpactEffect(collision);
        Destroy(gameObject);
    }
    
    private void SpawnImpactEffect(Collision collision)
    {
        if (impactEffectPrefab == null) return;
 
        Vector3 point = transform.position;
        Vector3 normal = -transform.forward;
 
        if (collision.contactCount > 0)
        {
            ContactPoint contact = collision.GetContact(0);
            point = contact.point;
            normal = contact.normal;
        }
        
        Quaternion rotation = Quaternion.FromToRotation(Vector3.up, normal);
        Instantiate(impactEffectPrefab, point + normal * 0.02f, rotation);
    }
    
}