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
    [SerializeField]
    private float Bloom;
    [SerializeField]
    private float KickBack;
    [SerializeField]
    private GameObject CurrentWeapon;
    //[SerializeField]
    //private CGun Gun[];

    

    private Animator Animator;
    private float LastShootTime;

    [SerializeField]
    public Transform _WeaponTransform;

    [SerializeField]
    private GameObject _bulletHolePreb;


    private CRecoil Recoil_Script;
    private void Awake()
    {
        Animator = GetComponent<Animator>();
        Recoil_Script = transform.Find("CameraRot/CameraRecoil").GetComponent<CRecoil>();
        CurrentWeapon = GetComponent<GameObject>();
    }

    public void Shoot()
    {
        if(LastShootTime - ShootDelay < Time.time)
        {
            //Use an object pool instead for these! To keep this tutorial focused, we'll skip impementing one
            //far more details you can see: https://youtu.be/fsDE_mO4RZM  and if using unity 2021+: https://youtu.be/zyzqA_CPz2E 
            Transform t_spawn = transform.Find("Camera/Normal");

            //Vector3 t_bloom = t_spawn.position + t_spawn.forward * 1000f;
            //t_bloom += Random.Range(-)
            //Animator.set
           
            Vector3 t_bloom = t_spawn.position + t_spawn.forward * 1000f;
            t_bloom += Random.Range(-Bloom, Bloom) * t_spawn.up;
            t_bloom += Random.Range(-Bloom, Bloom) * t_spawn.right;
            t_bloom -= t_spawn.position;
            t_bloom.Normalize();
            RaycastHit t_hit = new RaycastHit();
            ShootingSystem.Play();
            Vector3 direction = GetDirection();

            if(Physics.Raycast(BulletSpawnPoint.position, t_bloom, out t_hit, float.MaxValue, Mask ))
            {
                TrailRenderer trail = Instantiate(BulletTrail, BulletSpawnPoint.position, Quaternion.identity);

                StartCoroutine(SpawnTrail(trail, t_hit.point, t_hit.normal, true));

                LastShootTime = Time.time;

            }

            else
            {
                TrailRenderer trail = Instantiate(BulletTrail, BulletSpawnPoint.position, Quaternion.identity);

                StartCoroutine(SpawnTrail(trail, transform.forward * 100, Vector3.zero, false));

                LastShootTime = Time.time;
            }

           
        }

        Recoil_Script.RecoilFire();
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
