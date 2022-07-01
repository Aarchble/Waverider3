using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class VehicleBuilder : MonoBehaviour//, IPointerDownHandler
{
    public static VehicleBuilder Instance;
    public GameObject PointObject;
    public float scale = 10f;

    public List<GameObject> VehiclePoints;
    public List<GameObject> LeadingPoints;
    public List<GameObject> TrailingPoints;

    public void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // Fill VehiclePoints List
        UpdateVehiclePoints();
    }

    private void Update()
    {
        foreach (GameObject pt in LeadingPoints)
        {
            VehiclePoint vpt = pt.GetComponent<VehiclePoint>();
            int streamlineCount = vpt.DownstreamPoint.Length - (vpt.PartnerPoint != null ? 1 : 0); // This is always 2 or 1, except when Next() == null where it is 0
            for (int streamlineIndex = 0; streamlineIndex < streamlineCount; streamlineIndex++)
            {
                while (vpt.Next(streamlineIndex) != null)
                {
                    VehiclePoint nxtvpt = vpt.Next(streamlineIndex).GetComponent<VehiclePoint>();
                    if (vpt.PartnerPoint != null && nxtvpt.PartnerPoint != null)
                    {
                        // Internal Flows
                        //Engine = new InternalStream(new Vector3[] { vpt.transform.localPosition, vpt.PartnerPoint.transform.localPosition }, new Vector3[] { nxtvpt.transform.localPosition, nxtvpt.PartnerPoint.transform.localPosition });
                    }
                    else
                    {
                        // External Flows
                        //NacelleRamp = new ExternalStream[] { new ExternalStream(vpt.transform.localPosition, vpt.PartnerPoint.transform.localPosition, false) }; // need to set upper bool properly
                    }
                }
            }
        }
    }

    public void UpdateVehiclePoints()
    {
        foreach (GameObject pt in GameObject.FindGameObjectsWithTag("BuildPoint"))
        {
            VehiclePoint vpt = pt.GetComponent<VehiclePoint>();
            if (vpt.DownstreamPoint.Length < 1)
            {
                // No downstream == trailing
                TrailingPoints.Add(pt);
            }
            else if (vpt.UpstreamPoint.Length < 1)
            {
                // No upstream == leading
                LeadingPoints.Add(pt);
            }
            VehiclePoints.Add(pt);
        }
    }

    //public void OnPointerDown(PointerEventData eventData)
    //{
    //    if (GameManager.Instance.State is GameState.Building)
    //    {
    //        if (!StreamDefinitionStarted)
    //        {
    //            StreamDefinitionStarted = true;
    //            StartStreamDefinition((Vector3)Vector3Int.RoundToInt(transform.position * scale) / scale);
    //        }
    //        else
    //        {
    //            if (eventData.button == PointerEventData.InputButton.Left)
    //            {
    //                EndStreamDefinition();
    //            }
    //        }
    //    }
    //}

    //private void StartStreamDefinition(Vector3 startPosition)
    //{
    //    // I would like to be able to hold shift to append to the previous INLET
    //}

    //private void EndStreamDefinition()
    //{
    //    // I would like to be able to hold shift to append to the previous OUTLET
    //}
}
