using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    const float LOOK_INPUT_SCALE = 100f;
    const float MIN_VERITCAL_ROTATION = -60f;
    const float MAX_VERTICAL_ROTATION = 90f;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        float horizontalInput = Input.GetAxis("Mouse X");
        float verticalInput = Input.GetAxis("Mouse Y");

        Vector3 oldRotation = this.transform.rotation.eulerAngles;

        float xRotation = oldRotation.x + -verticalInput * Time.deltaTime * LOOK_INPUT_SCALE;
        // clamp within -180 to 180 range
        if (xRotation > 180) {
            xRotation -= 360;
        }
        if (xRotation < -180) {
            xRotation += 360;
        }
        xRotation = Mathf.Clamp(xRotation, MIN_VERITCAL_ROTATION, MAX_VERTICAL_ROTATION);
        this.transform.rotation = Quaternion.Euler(xRotation, oldRotation.y, oldRotation.z);


        // Update player horizontal rotation
        Vector3 playerNewRotation = this.transform.parent.rotation.eulerAngles + new Vector3(0f, horizontalInput * Time.deltaTime * LOOK_INPUT_SCALE, 0f);
        this.transform.parent.rotation = Quaternion.Euler(playerNewRotation);
    }
}
