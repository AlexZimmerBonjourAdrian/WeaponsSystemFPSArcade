using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CGun : MonoBehaviour
{
    [SerializeField]
    private bool AddBulletSpread = true;
    [SerializeField]
    private Vector3 BulletSpreadVariance = new Vector3(0.1f, 0.1f, 0.1f);
    [SerializeField]
    private ParticleSystem ShootingSystem;
    [SerializeField]
    private Transform BulletSpawnPoint;
    [SerializeField]
    private ParticleSystem ImpactParticleSystem;
    [SerializeField]
    private TrailRenderer BulletTrail;
    [SerializeField]
    private float ShootDelay = 0.5f;
    [SerializeField]
    private LayerMask Mask;
    [SerializeField]
    private float BulletSpeed = 100;

    private Animator Animator;
    private float LastShootTime;

    [SerializeField]
    public Transform _WeaponTransform;

    [SerializeField]
    private GameObject _bulletHolePreb;

    private void Awake()
    {
        Animator = GetComponent<Animator>();
    }

    public void Shoot()
    {
        if(LastShootTime - ShootDelay < Time.time)
        {
            //Use an object pool instead for these! To keep this tutorial focused, we'll skip impementing one
            //far more details you can see: https://youtu.be/fsDE_mO4RZM  and if using unity 2021+: https://youtu.be/zyzqA_CPz2E 

            //Animator.set
            ShootingSystem.Play();
            Vector3 direction = GetDirection();

            if(Physics.Raycast(BulletSpawnPoint.position, direction, out RaycastHit hit, float.MaxValue, Mask ))
            {
                TrailRenderer trail = Instantiate(BulletTrail, BulletSpawnPoint.position, Quaternion.identity);

                StartCoroutine(SpawnTrail(trail, hit.point, hit.normal, true));

                LastShootTime = Time.time;

            }
            else
            {
                TrailRenderer trail = Instantiate(BulletTrail, BulletSpawnPoint.position, Quaternion.identity);

                StartCoroutine(SpawnTrail(trail, transform.forward * 100, Vector3.zero, false));

                LastShootTime = Time.time;
            }
        }
    }

    private Vector3 GetDirection()
    {
        Vector3 direction = transform.forward;
       if(AddBulletSpread)
        {
            direction += new Vector3(
                Random.Range(-BulletSpreadVariance.x, BulletSpreadVariance.x),
                Random.Range(-BulletSpreadVariance.y,BulletSpreadVariance.y),
                Random.Range(-BulletSpreadVariance.z,BulletSpreadVariance.z)
                
                );
        }
        return direction;
    }

    private IEnumerator SpawnTrail(TrailRenderer Trail, Vector3 HitPoint, Vector3 HitNormal, bool MadeImpact)
    {
        // This has been updated from the video implementation to fix a commonly raised issue about the bullet trails
        // moving slowly when hitting something close, and not
        Vector3 startPosition = Trail.transform.position;
        float distance = Vector3.Distance(Trail.transform.position, HitPoint);
        float remainingDistance = distance;

        while (remainingDistance > 0)
        {
            Trail.transform.position = Vector3.Lerp(startPosition, HitPoint, 1 - (remainingDistance / distance));

            remainingDistance -= BulletSpeed * Time.deltaTime;

            yield return null;
        }
        Animator.SetBool("IsShooting", false);
        Trail.transform.position = HitPoint;
        if (MadeImpact)
        {
            Instantiate(ImpactParticleSystem, HitPoint, Quaternion.LookRotation(HitNormal));
        }

        Destroy(Trail.gameObject, Trail.time);
    }


    //Code Provisional Bullet Hole

    public void BulletHole()
    {
        Ray rayOriginal = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;

        if(Physics.Raycast(rayOriginal,out hitInfo))
        {
            if(hitInfo.collider.tag == "wall")
            {
                //Inantiate bullet hole
                Instantiate(_bulletHolePreb, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));

                Vector3 direction = hitInfo.point - _WeaponTransform.position;
                _WeaponTransform.rotation = Quaternion.LookRotation(direction);
            }

        }

    }
}
