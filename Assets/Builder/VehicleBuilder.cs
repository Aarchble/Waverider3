using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class VehicleBuilder : MonoBehaviour, IPointerDownHandler
{
    public GameObject PointObject;
    bool StreamDefinitionStarted;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (GameManager.Instance.State is GameState.Building)
        {
            if (!StreamDefinitionStarted)
            {
                StreamDefinitionStarted = true;
            }
            else
            {

            }
        }
    }

    private void StartStreamDefinition(Vector3 startPosition)
    {
        // I would like to be able to hold shift to append to the previous INLET
    }

    private void EndStreamDefinition()
    {
        // I would like to be able to hold shift to append to the previous OUTLET
    }
}
