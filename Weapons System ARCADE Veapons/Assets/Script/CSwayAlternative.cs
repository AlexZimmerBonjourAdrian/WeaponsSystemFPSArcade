using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSwayAlternative : MonoBehaviour
{

    [Header("Position")]
    public float amount = 0.02f;
    public float smoothAmount = 0.06f;
    public float maxAmount = 6f;

    public float mouseSensitivity;


    [Header("Rotation")]
    public float rotationAmount = 4;
    public float maxRotationAmount = 5;
    public float smoothRotation = 12;

    [Space]
    public bool rotationX = true;
    public bool rotationY = true;
    public bool rotationZ = true;

    private Vector3 initialPosition;

    private Quaternion initialRotation;

    [SerializeField]private float InputX;
    [SerializeField]private float InputY;

    // Start is called ibefore the first frame update
    private void Start()
    {
        initialPosition = transform.localPosition;
        initialRotation = transform.localRotation;
    }

    // Update is called once per frame
    public void FixedUpdate()
    {
        CalculateSway();

        MoveSway();
        TiltSway();
    }

    public void CalculateSway()
    {
        InputX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        InputY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
    }

    private void MoveSway()
    {
        float mouveX = Mathf.Clamp(InputX * amount, -maxAmount, maxAmount);
        float mouveY = Mathf.Clamp(InputY * amount, -maxAmount, maxAmount);


        Vector3 finalPosition = new Vector3(mouveX, mouveY, 0);

        //target towards target rotation
        transform.localPosition = Vector3.Lerp(transform.localPosition, finalPosition + initialPosition, Time.deltaTime * smoothAmount);

    }

    private void TiltSway()
    {
        float titlY = Mathf.Clamp(InputX * rotationAmount, -maxRotationAmount, maxRotationAmount);
        float titlX = Mathf.Clamp(InputY * rotationAmount, -maxRotationAmount, maxRotationAmount);


        Quaternion finalRotation = Quaternion.Euler(new Vector3(rotationX ? -titlX : 0f, rotationY ? titlY : 0f, rotationZ ? titlY : 0));
        //target towards target rotation
        transform.localRotation = Quaternion.Slerp(transform.localRotation, finalRotation * initialRotation, Time.deltaTime * smoothAmount);

    }
}
