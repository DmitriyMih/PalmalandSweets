using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonController : MonoBehaviour
{
    [Header("Tower Settings")]
    [SerializeField] private Transform towerTransform;

    private Vector3 directionTarget;
    [SerializeField] private bool IsInput;
   
    [Header("Shoot Settings")]
    [SerializeField] private Transform muzzlePoint;
    [SerializeField] private BaseBullet bulletPrefab;

    private void Update()
    {
        GetInput();
        TowerRotation();
    }

    private void GetInput()
    {
        if (Input.GetMouseButton(0))
        {
            IsInput = true;

            Vector2 mousePosition = new Vector2(Mathf.Clamp(Input.mousePosition.x, 0, Screen.width), Mathf.Clamp(Input.mousePosition.y, 0, Screen.height));
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);

            if (Physics.Raycast(ray, out RaycastHit raycastHit))
            {
                directionTarget = raycastHit.point;
            }
        }
        else
        {
            if (IsInput)
                Shoot();

            IsInput = false;
        }
    }

    private void TowerRotation()
    {
        if (!IsInput || towerTransform == null)
            return;

        towerTransform.rotation = Quaternion.LookRotation(directionTarget);
    }

    private void Shoot()
    {
        if (bulletPrefab == null)
            return;

        Debug.Log("Shoot");
        BaseBullet bulletClone = Instantiate(bulletPrefab, muzzlePoint.position, towerTransform.rotation);
        bulletClone.GetComponent<Rigidbody>().velocity = towerTransform.forward * bulletClone.GetSpeed();
    }
}