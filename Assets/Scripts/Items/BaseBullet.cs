using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum Direction
{
    Forward,
    Back
}

[RequireComponent(typeof(SphereCollider))]
public class BaseBullet : MonoBehaviour
{
    [SerializeField] private float bulletSpeed = 1f;
    [SerializeField] private PathFollower pathFollower;

    private void OnEnable()
    {
        pathFollower = GetComponent<PathFollower>();
    }
    private void OnDisable()
    {
        pathFollower = null;
    }

    public float GetSpeed()
    {
        return bulletSpeed;
    }

    [SerializeField] private Direction connectDirection;

    private void Connected(PathFollower connectedFollower, Vector3 connectionPosition)
    {
        Vector2 centralPoint = new Vector2(connectedFollower.transform.position.x, connectedFollower.transform.position.z);
        Vector2 connectionPoint = new Vector2(connectionPosition.x, connectionPosition.z);

        Vector2 direction = connectionPoint - centralPoint;
        connectDirection = direction.y > 0 ? Direction.Forward : Direction.Back;

        if (connectedFollower.HasPathController() && CheckForDestruction())
            connectedFollower.DestroyItem();
    }

    private bool CheckForDestruction()
    {


        return true;
    }

    private void DestroyBullet()
    {
        GetComponent<SphereCollider>().isTrigger = false;

        Destroy(GetComponent<Rigidbody>());
        //Destroy(this);
        Destroy(this.gameObject);
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