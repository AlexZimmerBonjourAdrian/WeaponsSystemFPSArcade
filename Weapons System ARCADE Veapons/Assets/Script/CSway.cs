using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSway : MonoBehaviour
{

    public float intensity;
    public float smooth;
    private Quaternion original_rotation;
    
    // Start is called before the first frame update
    void Start()
    {
        original_rotation = transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateSway();
    }

    public void UpdateSway()
    {
        float t_x_mmouseX = Input.GetAxis("Mouse X");
        float t_y_mmouseY = Input.GetAxis("Mouse Y");

        //Calculate target rotation
        Quaternion  t_x_adj= Quaternion.AngleAxis(intensity * t_x_mmouseX, Vector3.up);
        Quaternion t_y_adj = Quaternion.AngleAxis(intensity * t_y_mmouseY, Vector3.right);
        Quaternion target_rotation = original_rotation * t_x_adj * t_y_adj;

        

        //target towards target rotation
        transform.localRotation = Quaternion.Lerp(transform.localRotation, target_rotation, Time.deltaTime *smooth);


    }
}
