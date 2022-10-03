using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(CPositionFollower))]
public class CViewBobbing : MonoBehaviour
{
    public float EffectIntesity;
    public float EffectIntensityX;
    public float EffectSpeed;

    private CPositionFollower FollowerInstance;
    private Vector3 OriginalOffset;
    private float SinTime;

    private void Start()
    {
        FollowerInstance = GetComponent<CPositionFollower>();
        OriginalOffset = FollowerInstance.offset;
    }

    private void Update()
    {
        Vector3 inputVector = new Vector3(Input.GetAxis("Vertical"), 0f, Input.GetAxis("Horizontal"));
        if (inputVector.magnitude > 0f)
        {
            SinTime += Time.deltaTime * EffectSpeed;
           
 
        }
        else
        {
            SinTime = 0f;
        }
        float sinAmountY = -Mathf.Abs(EffectIntesity * Mathf.Sign(SinTime));
        Vector3 sinAmoutX = FollowerInstance.transform.right * EffectIntesity * Mathf.Cos(SinTime) * EffectIntensityX;

        FollowerInstance.offset = new Vector3
        {
            x = OriginalOffset.x,
            y = OriginalOffset.y + sinAmountY,
            z = OriginalOffset.z
        };
        FollowerInstance.offset += sinAmoutX;

    }

}
