using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

[RequireComponent(typeof(PathCreator))]
public class PathController : MonoBehaviour
{
    private PathCreator pathCreator;

    [SerializeField] private List<BaseSphereItem> itemsList = new List<BaseSphereItem>();

    private void Awake()
    {
        pathCreator = GetComponent<PathCreator>();
    }

    public PathCreator GetPathCreator()
    {
        return pathCreator;
    }

    public void AddSphereItem(BaseSphereItem sphereItem, int index = -1)
    {
        if (sphereItem == null)
            return;

        sphereItem.GetComponent<PathFollower>().AddPathCreator(pathCreator);

        if (index != -1)
        {
            index = Mathf.Clamp(index, 0, itemsList.Count - 1);
            itemsList.Insert(index, sphereItem);
        }
        else
            itemsList.Add(sphereItem);
    }

    public void RemoveSphereItem(BaseSphereItem sphereItem)
    {
        if (sphereItem == null)
            return;

        itemsList.Remove(sphereItem);
    }
}