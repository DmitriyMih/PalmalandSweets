using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item SO", fileName = "Base Sphere Item SO")]
public class BaseSphereItemSO : ScriptableObject
{
    public Material material;
    public int typeId;
}