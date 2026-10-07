using System.Collections;
using UnityEngine;
using TMPro;

public class Gun : MonoBehaviour
{
    [Header("Shoot")]
    [SerializeField] private float timeBetweenShots = 0.15f;
    
    [Header("Aim")]
    [SerializeField] private float aimDistance = 100f;
    [SerializeField] private float minAimDistance = 2f;
    [SerializeField] private LayerMask aimMask = ~0;

    [Header("Bullets")]
    [SerializeField] private int magazineSize = 30;
    [SerializeField] private float reloadTime = 1.5f;

    [Header("References")]
    [SerializeField] private GameObject muzzlePrefab;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private Camera fpsCam;
    [SerializeField] private TextMeshProUGUI ammoText;

    private int bulletsLeft;
    private float nextShotTime;
    private bool reloading;

    private void Awake()
    {
        bulletsLeft = magazineSize;
        UpdateAmmoText();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            TryReload();
        }

        if (Input.GetKey(KeyCode.Mouse0) && CanShoot())
        {
            Shoot();
        }
    }

    private bool CanShoot()
    {
        return !reloading && bulletsLeft > 0 && Time.time >= nextShotTime;
    }

    private void Shoot()
    {
        Instantiate(muzzlePrefab, firePoint.position, firePoint.rotation, firePoint);
        Instantiate(bulletPrefab, firePoint.position, GetAimRotation());

        bulletsLeft--;
        nextShotTime = Time.time + timeBetweenShots;
        UpdateAmmoText();
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
        return Quaternion.LookRotation(aimPoint - firePoint.position);
    }

    private void TryReload()
    {
        if (reloading || bulletsLeft >= magazineSize) return;

        StartCoroutine(Reload());
    }

    private IEnumerator Reload()
    {
        reloading = true;
        yield return new WaitForSeconds(reloadTime);

        bulletsLeft = magazineSize;
        reloading = false;
        UpdateAmmoText();
    }

    private void UpdateAmmoText()
    {
        if (ammoText != null)
        {
            ammoText.SetText(bulletsLeft + " / " + magazineSize);
        }
    }
}