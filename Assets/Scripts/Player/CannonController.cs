using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonController : MonoBehaviour
{
    [Header("Tower Settings")]
    [SerializeField] private Transform towerTransform;

    private Vector3 directionTarget;

    [SerializeField] private bool lockTowerOnXZ = true;
    [SerializeField, Space(5)] private bool IsTraking;

    [Header("Charged Settings")]

    [SerializeField] private float maxChargedTime = 0.25f;
    [SerializeField] private float currentChargedTime;
    [SerializeField] private bool isCharged => loadedBullet;

    [Header("Shoot Settings")]
    [SerializeField] private Transform muzzlePoint;
    [SerializeField] private Rigidbody bulletPrefab;

    [SerializeField, Space(5)] private List<BaseSphereItemSO> baseSphereItemSO = new List<BaseSphereItemSO>();
    [SerializeField] private BaseBullet loadedBullet;

    private void Awake()
    {
        currentChargedTime = maxChargedTime;
    }

    private void Update()
    {
        Recharge();
        GetInput();
        TowerRotation();
    }

    private void Recharge()
    {
        if (isCharged)
            return;

        if (currentChargedTime <= 0)
        {
            currentChargedTime = maxChargedTime;
            Rigidbody bulletClone = Instantiate(bulletPrefab, muzzlePoint.transform.position, Quaternion.identity);
            bulletClone.transform.parent = muzzlePoint.transform;

            loadedBullet = bulletClone.gameObject.GetComponent<BaseBullet>();
        }
        else
            currentChargedTime -= Time.deltaTime;
    }

    private void GetInput()
    {
        if (Input.GetMouseButton(0))
        {
            IsTraking = true;

            Vector2 mousePosition = new Vector2(Mathf.Clamp(Input.mousePosition.x, 0, Screen.width), Mathf.Clamp(Input.mousePosition.y, 0, Screen.height));
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);

            if (Physics.Raycast(ray, out RaycastHit raycastHit))
            {
                directionTarget = raycastHit.point;
            }
        }
        else
        {
            if (isCharged && IsTraking)
                Shoot();

            IsTraking = false;
        }
    }

    private void TowerRotation()
    {
        if (!IsTraking || towerTransform == null)
            return;

        towerTransform.rotation = Quaternion.LookRotation(directionTarget);

        if (lockTowerOnXZ)
            towerTransform.eulerAngles = new Vector3(0f, towerTransform.transform.eulerAngles.y, 0f);
    }

    private void Shoot()
    {
        if (loadedBullet == null)
            return;

        Debug.Log("Shot");
        loadedBullet.GetComponent<Rigidbody>().velocity = towerTransform.forward * loadedBullet.GetSpeed();
        
        loadedBullet.transform.parent = null;
        loadedBullet = null;
    }
}