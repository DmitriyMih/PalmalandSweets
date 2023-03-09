using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

[RequireComponent(typeof(PathCreator))]
public class PathController : MonoBehaviour
{
    [SerializeField] private Distributor distributor;
    private PathCreator pathCreator;

    [SerializeField] private float moveSpeed = 1f;
    private float tempSpeed;

    [SerializeField] private List<PathFollower> itemsList = new List<PathFollower>();

    private void Awake()
    {
        pathCreator = GetComponent<PathCreator>();
        DebugInitializationList();
    }

    private void DebugInitializationList()
    {
        for (int i = 0; i < itemsList.Count; i++)
        {
            itemsList[i].SetDistanceTravelled(itemsList.Count - i);
            itemsList[i].SetIndex(i + 1);
        }
    }

    private void Update()
    {
        if (tempSpeed != moveSpeed)
        {
            UpdateItemsMoveSpeed();
            tempSpeed = moveSpeed;
        }
    }

    public PathCreator GetPathCreator()
    {
        return pathCreator;
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

    public void AddWithOffset(PathFollower newItem, PathFollower itemInList, Direction direction)
    {
        int index = itemsList.IndexOf(itemInList);
        Debug.Log("Index: " + index);

        if (direction == Direction.Forward)
        {
            if (itemsList.Count > 1)
            {
                PathFollower[] forwardItems = new PathFollower[0];
                itemsList.CopyTo(forwardItems, index);

                for (int i = 0; i < forwardItems.Length; i++)
                {
                    forwardItems[i].MoveToNewDistance(itemInList, (i * 1f));
                }

            }
        }
        else
        {

        }
    }

    public void AddItemInPath(PathFollower sphereItem, int index = -1)
    {
        if (sphereItem == null)
            return;

        sphereItem.ChangeMoveSpeed(moveSpeed);
        sphereItem.ChangePathController(this);

        if (index != -1)
        {
            index = Mathf.Clamp(index, 0, itemsList.Count - 1);
            itemsList.Insert(index, sphereItem);
        }
        else
            itemsList.Add(sphereItem);

        sphereItem.SetMoveDistance(0f);
        sphereItem.transform.parent = transform;
    }

    public void RemoveSphereItem(PathFollower sphereItem)
    {
        if (sphereItem == null)
            return;

        sphereItem.ChangePathController(null);
        itemsList.Remove(sphereItem);
    }
}