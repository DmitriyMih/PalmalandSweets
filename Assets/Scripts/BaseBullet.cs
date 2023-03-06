using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseBullet : MonoBehaviour
{
    [SerializeField] private float bulletSpeed = 1f;

    public float GetSpeed()
    {
        return bulletSpeed;
    }

    private void Connected(BaseSphereItem baseSphereItem)
    {
        Debug.Log("Connect");
    }

    private void DestroyBullet()
    {
        Destroy(GetComponent<Rigidbody>());
        Destroy(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out BaseSphereItem baseSphereItem))
        {
            Connected(baseSphereItem); 
            DestroyBullet();
        }
    }
}