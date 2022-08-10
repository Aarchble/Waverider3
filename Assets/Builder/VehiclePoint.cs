using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//[ExecuteInEditMode]
public class VehiclePoint : MonoBehaviour
{
    Vector3 ClickOffset;
    Camera mainCam;
    float SnapScale = 10f;
    InputAction mouseClick;

    //private void OnMouseDown()
    //{
    //    ClickOffset = transform.position - GetMousePos();
    //}

    //private void OnMouseDrag()
    //{
    //    transform.position = (Vector3)Vector3Int.RoundToInt((GetMousePos() + ClickOffset) * SnapScale) / SnapScale;
    //}

    //Vector3 GetMousePos()
    //{
    //    Vector3 mousePos = Cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
    //    mousePos.z = 0f;
    //    return mousePos;
    //}

}
