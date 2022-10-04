using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CMotion : MonoBehaviour
{
    public float speed;
    public float sprintModifier;
    public Camera normalCam;
    public float jumpForce;
    private Rigidbody rig;
    public LayerMask ground;
    public Transform groundDetection;
    private float baseFOV;
    private float sprintFOVModifier = 1.5f;
    //public Camera NormalCam;
    //public Transform weaponParent;

    //public Vector3 weapons
    private void Start()
    {
        baseFOV = normalCam.fieldOfView;
        Camera.main.enabled = false;
        rig = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        //Axis
        float t_hmove = Input.GetAxisRaw("Horizontal");
        float t_vmove = Input.GetAxisRaw("Vertical");

        //Controls
        bool sprint = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        bool jump = Input.GetKeyDown(KeyCode.Space);

        //States
        bool isGrounded = Physics.Raycast(groundDetection.position, Vector3.down, 0.1F, ground);
        bool isJumping = jump;
        bool isSprinting = sprint && t_vmove > 0 && !isJumping && isGrounded;

        //Jumping
        if (isJumping)
        {
            rig.AddForce(Vector3.up * jumpForce);
        }

        //Movement
        Vector3 t_direction = new Vector3(t_hmove, 0, t_vmove);
        t_direction.Normalize();

        float t_adjustedSpeed = speed;
        if (isSprinting) t_adjustedSpeed *= sprintModifier;

        Vector3 t_targetVeolocity = transform.TransformDirection(t_direction) * t_adjustedSpeed * Time.deltaTime;
        t_targetVeolocity.y = rig.velocity.y;
        rig.velocity = t_targetVeolocity;

        //Find of view
        if (isSprinting) {  normalCam.fieldOfView = Mathf.Lerp(normalCam.fieldOfView, baseFOV * sprintFOVModifier,Time.deltaTime); }
        else {
            normalCam.fieldOfView = Mathf.Lerp(normalCam.fieldOfView,baseFOV, Time.deltaTime * 0f); ;}
    }

}
