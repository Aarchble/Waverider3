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


    private void Awake()
    {
        mainCam = Camera.main;
    }

    private void OnEnable()
    {
        mouseClick.Enable();
        mouseClick.performed += MousePressed;
    }

    private void OnDisable()
    {
        mouseClick.performed -= MousePressed;
        mouseClick.Enable();
    }

    private void MousePressed(InputAction.CallbackContext context)
    {
        Debug.Log("Clicked");
        Ray ray = mainCam.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            Debug.Log("Hit");
            if (hit.collider != null && hit.transform.tag == "BuildPoint")
            {
                StartCoroutine(MouseDragUpdate(hit.collider.gameObject));
            }
        }
    }

    private IEnumerator MouseDragUpdate(GameObject clickedObject)
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
