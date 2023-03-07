using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

[RequireComponent(typeof(PathCreator))]
public class PathController : MonoBehaviour
{
    private PathCreator pathCreator;
    [SerializeField] private Distributor distributor;

    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float tempSpeed;

    [SerializeField] private List<PathFollower> itemsList = new List<PathFollower>();

    private void Awake()
    {
        pathCreator = GetComponent<PathCreator>();
    }

    public PathCreator GetPathCreator()
    {
        return pathCreator;
    }

    private void Update()
    {
        if (tempSpeed != moveSpeed)
        {
            UpdateItemsMoveSpeed();
            tempSpeed = moveSpeed;
        }
    }

    private void UpdateItemsMoveSpeed()
    {
        if (distributor != null)
            distributor.ChangeSpeedAcceleration(moveSpeed);

        for (int i = 0; i < itemsList.Count; i++)
        {
            if (itemsList[i] == null)
                continue;

            itemsList[i].ChangeMoveSpeed(moveSpeed);
        }
    }

    public void AddItemInPath(PathFollower sphereItem, int index = -1)
    {
        if (sphereItem == null)
            return;

        sphereItem.AddPathCreator(pathCreator);
        sphereItem.ChangeMoveSpeed(moveSpeed);

        if (index != -1)
        {
            index = Mathf.Clamp(index, 0, itemsList.Count - 1);
            itemsList.Insert(index, sphereItem);
        }
        else
            itemsList.Add(sphereItem);
    }

    public void RemoveSphereItem(PathFollower sphereItem)
    {
        if (sphereItem == null)
            return;

        itemsList.Remove(sphereItem);
    }
}