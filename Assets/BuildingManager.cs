using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BuildingManager : MonoBehaviour
{
    Camera mainCam;
    float SnapScale = 10f;
    [SerializeField]
    InputAction mouseClick;
    [SerializeField]
    InputAction shiftMouseClick;
    public GameObject BuildPoint;


    private void Awake()
    {
        mainCam = Camera.main;
    }

    private void OnEnable()
    {
        mouseClick.Enable();
        mouseClick.performed += MousePressed;
        mouseClick.canceled += UpdateVehicle;

        shiftMouseClick.Enable();
        shiftMouseClick.performed += ShiftMousePressed;
        shiftMouseClick.canceled += UpdateVehicle;
    }

    private void OnDisable()
    {
        mouseClick.performed -= MousePressed;
        mouseClick.canceled -= UpdateVehicle;
        mouseClick.Disable();

        shiftMouseClick.performed -= ShiftMousePressed;
        shiftMouseClick.canceled -= UpdateVehicle;
        shiftMouseClick.Disable();
    }

    private void MousePressed(InputAction.CallbackContext context)
    {
        Debug.Log("Clicked");
        Ray ray = mainCam.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit)) // Clicked something
        {
            Debug.Log("Hit");
            if (hit.collider != null && hit.transform.tag == "BuildPoint")
            {
                StartCoroutine(MovePointUpdate(hit.collider.gameObject));
            }
        }
    }

    private void ShiftMousePressed(InputAction.CallbackContext context)
    {
        Debug.Log("Clicked");
        Vector3 newPointPosition = mainCam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        newPointPosition.z = 0f;
        Debug.Log("Add at " + Mouse.current.position.ReadValue());
        // Instantiate new point at position
        GameObject newPoint = Instantiate(BuildPoint, newPointPosition, transform.rotation, VehicleStatic.Instance.gameObject.transform);

        // Add ramp point to vehicle
        //VehiclePhysics.Instance.StartSimPause();
        VehicleStatic.Instance.AddRampPoint(newPoint);
        //VehiclePhysics.Instance.StopSimPause();
    }

    private void UpdateVehicle(InputAction.CallbackContext context)
    {
        VehicleStatic.Instance.BuildVehicle();
    }

    private IEnumerator MovePointUpdate(GameObject clickedObject)
    {
        float rayLength = Vector3.Distance(clickedObject.transform.position, mainCam.transform.position);
        while (mouseClick.ReadValue<float>() != 0f)
        {
            Ray ray = mainCam.ScreenPointToRay(Mouse.current.position.ReadValue());
            clickedObject.transform.position = ray.GetPoint(rayLength);
            yield return null;
        }
    }
}
