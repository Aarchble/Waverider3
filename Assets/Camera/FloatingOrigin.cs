using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FloatingOrigin : MonoBehaviour
{
    public float threshold = 5000f;
    Vector3 TotalDisplacement = Vector3.zero;
    public Vector3 LiveDisplacement;

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 cameraPosition = transform.position;
        cameraPosition.z = 0f;

        if (cameraPosition.magnitude > threshold)
        {
            foreach (GameObject g in GameObject.FindGameObjectsWithTag("Moving"))
            {
                g.transform.position -= cameraPosition; // Move each object back to origin
            }

            TotalDisplacement += cameraPosition;
        }

        LiveDisplacement = TotalDisplacement + cameraPosition;
        //Debug.Log("Downrange = " + LiveDisplacement.x + ", Altitude = " + LiveDisplacement.y);
    }
}
