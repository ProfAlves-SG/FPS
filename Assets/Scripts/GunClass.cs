using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GunClass : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform bulletPoint;
    [SerializeField] private float timeBetweenShoots = 0.15f;

    private float nextShootTime;

    [SerializeField] private TextMeshProUGUI ammoText; 
    [SerializeField] private float magazineSize; 

    private float bulletsLeft; 


    [SerializeField] private float reloadTime;

    private bool isReloading;


    [SerializeField] private float aimDistance = 100f;
    [SerializeField] private float minAimDistance = 2f;
    [SerializeField] private LayerMask aimMask;

    [SerializeField] private Camera fpsCam;
    [SerializeField] private GameObject muzzlePrefab;

    

    private void Awake()
    {
        bulletsLeft = magazineSize;
        UpdateText();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            TryReload();
        }
        if(Input.GetKey(KeyCode.Mouse0) && CanShoot())
        {
            Shoot();
        }
    }

    private void TryReload()
    {
        if (isReloading || bulletsLeft >= magazineSize) return;

        StartCoroutine(Reload());
    }

    private IEnumerator Reload()
    {
        isReloading = true;
        yield return new WaitForSeconds(reloadTime);

        bulletsLeft = magazineSize;
        isReloading = false;
        UpdateText();
    }

    private bool CanShoot()
    {
        return bulletsLeft > 0 && Time.time >= nextShootTime;
    }

    private void Shoot()
    {
        Instantiate(muzzlePrefab, bulletPoint.position, bulletPoint.rotation);
        Instantiate(bulletPrefab, bulletPoint.position, GetAimRotation());
       
        bulletsLeft--;

        nextShootTime = Time.time + timeBetweenShoots;

        UpdateText(); //
    }

    private void UpdateText() 
    {
        ammoText.SetText(bulletsLeft + " / " + magazineSize);
    }

    private Quaternion GetAimRotation()
    {
        Ray aimRay = fpsCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        float distance = aimDistance;

        if (Physics.Raycast(aimRay, out RaycastHit hit, aimDistance, aimMask))
        {
            distance = Mathf.Max(hit.distance, minAimDistance);
        }

        Vector3 aimPoint = aimRay.GetPoint(distance);
        return Quaternion.LookRotation(aimPoint - bulletPoint.position);
    }
}
