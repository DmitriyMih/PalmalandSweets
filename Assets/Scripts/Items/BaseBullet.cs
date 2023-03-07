using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class BaseBullet : MonoBehaviour
{
    [SerializeField] private float bulletSpeed = 1f;

    public float GetSpeed()
    {
        return bulletSpeed;
    }

    [SerializeField] private Direction connectDirection;
    private enum Direction
    {
        Forward,
        Back
    }

    private void Connected(PathFollower pathFollower, Vector3 connectionPosition)
    {
        Vector2 centralPoint = new Vector2(pathFollower.transform.position.x, pathFollower.transform.position.z);
        Vector2 connectionPoint = new Vector2(connectionPosition.x, connectionPosition.z);

        Vector2 direction = connectionPoint - centralPoint;
        connectDirection = direction.y > 0 ? Direction.Forward : Direction.Back;
    }

    private void DestroyBullet()
    {
        GetComponent<SphereCollider>().isTrigger = false;

        Destroy(GetComponent<Rigidbody>());
        Destroy(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PathFollower pathFollower))
        {
            Connected(pathFollower, other.ClosestPoint(transform.position));
            DestroyBullet();
        }
    }
}