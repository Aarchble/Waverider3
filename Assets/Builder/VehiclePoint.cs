using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteInEditMode]
public class VehiclePoint : MonoBehaviour
{
    //public bool gridSnap = false;
    public List<NearStream> ConnectedStreams;

    public GameObject[] UpstreamPoint;
    public GameObject[] DownstreamPoint;

    // Internal streams
    public GameObject PartnerPoint; // Build internal stream based on Upstream/Downstream PartnerPoint ALSO not null

    Vector3 ClickOffset;
    Camera Cam;

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
        transform.position = (Vector3)Vector3Int.RoundToInt((GetMousePos() + ClickOffset) * VehicleBuilder.Instance.scale) / VehicleBuilder.Instance.scale;
    }

    Vector3 GetMousePos()
    {
        Vector3 mousePos = Cam.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;
        return mousePos;
    }

    public GameObject Next(int streamLineIndex)
    {
        if (DownstreamPoint != null)
        {
            if (DownstreamPoint.Length > 1)
            {
                return DownstreamPoint[streamLineIndex];
            }
            else
            {
                return DownstreamPoint[0];
            }
        }
        else
        {
            return null;
        }
    }

}
