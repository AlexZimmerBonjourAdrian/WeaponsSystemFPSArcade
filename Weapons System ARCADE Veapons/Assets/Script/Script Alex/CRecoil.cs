using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CRecoil : MonoBehaviour
{
    [SerializeField] private CPlayer Player_Script;

    private bool isAiming;

    private Vector3 currentRotation;
    private Vector3 targetRotation;

    //hipFire Recoil
    [SerializeField] private float recoilX;
    [SerializeField] private float recoilY;
    [SerializeField] private float recoilZ;

    [SerializeField] private float aimrRecoilX;
    [SerializeField] private float aimRecoilY;
    [SerializeField] private float aimRecoilZ;
    //Settings
    [SerializeField] private float snappiness;
    [SerializeField] private float returnSpeed;


    void Start()
    {
        
    }

 
    void Update()
    {
        //isAiming = Player_Script.aiming;
        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, returnSpeed * Time.deltaTime);
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, snappiness * Time.fixedDeltaTime);
        transform.localRotation = Quaternion.Euler(currentRotation);
        
    }

    public void RecoilFire()
    {
        targetRotation += new Vector3(recoilX, Random.Range(-recoilY, recoilY), Random.Range(-recoilZ, recoilZ));
    }
}
