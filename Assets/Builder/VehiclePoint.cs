using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteInEditMode]
public class VehiclePoint : MonoBehaviour
{
    Vector3 ClickOffset;
    Camera Cam;
    float SnapScale = 10f;

    private void Awake()
    {
        Cam = Camera.main;
    }

    private void OnMouseDown()
    {
        ClickOffset = transform.position - GetMousePos();
    }

    private void OnMouseDrag()
    {
        transform.position = (Vector3)Vector3Int.RoundToInt((GetMousePos() + ClickOffset) * SnapScale) / SnapScale;
    }

    Vector3 GetMousePos()
    {
        Vector3 mousePos = Cam.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;
        return mousePos;
    }

}
