using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugItemColorize : MonoBehaviour
{
    [SerializeField] private Material defaultMaterial;

    [Header("Distributor Settings")]
    [SerializeField] private Distributor distributor;
    [SerializeField] private List<BaseSphereItemSO> sphereItemsSO = new List<BaseSphereItemSO>();

    [SerializeField] private bool isRandomColor = true;

    [Header("Custom Test Settings")]
    [SerializeField] private List<PathFollower> debugFollowersList = new List<PathFollower>();

    private void Start()
    {
        if (distributor != null)
            distributor.InstatiateAction += CheckItem;

        CustomDebug();
    }

    private void CheckItem(PathFollower pathFollower)
    {
        if (pathFollower == null)
            return;

        BaseSphereItem sphereItem;
        if (pathFollower.HasSphereItem())
        {
            sphereItem = pathFollower.GetSphereItem();

            if (isRandomColor)
                SetColorInItem(sphereItem, GetRandomMaterial());
            else
                SetColorInItem(sphereItem, defaultMaterial);
        }
    }

    private void CustomDebug()
    {
        for (int i = 0; i < debugFollowersList.Count; i++)
        {
            if (debugFollowersList[i] == null)
                continue;

            BaseSphereItem sphereItem = new BaseSphereItem();
            if (debugFollowersList[i].HasSphereItem())
                sphereItem = debugFollowersList[i].GetSphereItem();

            if (isRandomColor)
                SetColorInItem(sphereItem, GetRandomMaterial());
            else
                SetColorInItem(sphereItem, defaultMaterial);
        }
    }

    private Material GetRandomMaterial()
    {
        int index = Random.Range(0, sphereItemsSO.Count);
        Debug.Log("Index: " + index);
        return sphereItemsSO[index].material;
    }

    private void SetColorInItem(BaseSphereItem sphereItem, Material newMaterial)
    {
        if (sphereItem == null || newMaterial == null)
            return;

        Debug.Log($"Set Material: {newMaterial} | In: {sphereItem}");
        if (sphereItem.HasRenderer())
            sphereItem.SetRenderMaterial(newMaterial);
    }
}