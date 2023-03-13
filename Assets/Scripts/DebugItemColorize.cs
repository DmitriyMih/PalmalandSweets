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
                SetColorInItem(sphereItem, itemSO: GetRandomSO());
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
                SetColorInItem(sphereItem, itemSO: GetRandomSO());
            else
                SetColorInItem(sphereItem, defaultMaterial);
        }
    }

    private BaseSphereItemSO GetRandomSO()
    {
        int index = Random.Range(0, sphereItemsSO.Count);
        return sphereItemsSO[index];
    }

    private void SetColorInItem(BaseSphereItem sphereItem, Material newMaterial = null, BaseSphereItemSO itemSO = null)
    {
        if (sphereItem == null)
            return;

        if (itemSO != null)
        {
            sphereItem.SetSphereItemSO(itemSO);
            sphereItem.SetRenderMaterial(itemSO.material);
        }
        else
        {
            if (sphereItem.HasRenderer() && newMaterial != null)
                sphereItem.SetRenderMaterial(newMaterial);
        }
    }
}