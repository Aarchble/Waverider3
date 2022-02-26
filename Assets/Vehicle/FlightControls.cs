using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlightControls : MonoBehaviour
{
    public float PitchInceptor;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y) - new Vector2(Screen.width / 2, Screen.height / 2);
        //float delta = Vector2.SignedAngle(rb.GetRelativeVector(Vector2.right), mPos);
        PitchInceptor = Vector2.SignedAngle(Vector2.right, mPos);
    }
}
