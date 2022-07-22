using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    GameObject FollowTarget;
    [Range(-1f, 1f)]
    public float Xoffset;
    [Range(-1f, 1f)]
    public float Yoffset;
    public float Zoffset;

    // Start is called before the first frame update
    void Start()
    {
        FollowTarget = VehiclePhysics.Instance.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        float height = GetComponent<Camera>().orthographicSize * 2f;
        float width = height * Screen.width / Screen.height;
        transform.position = FollowTarget.transform.position + Vector3.back * Zoffset + Vector3.left * width * Xoffset / 2f + Vector3.down * height * Yoffset / 2f;
    }
}
