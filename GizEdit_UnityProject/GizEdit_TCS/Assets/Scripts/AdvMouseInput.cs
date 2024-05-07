using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;
using UnityEngine.InputSystem;

public class AdvMouseInput : MonoBehaviour
{
    public static AdvMouseInput instance;

    public Vector3 worldPos;
    public float planeOffset;
    Plane plane;

    Transform camTransform, playerTransform;

    //Editor stuff
    bool mouseDown = false;
    public Transform moveGiz;

    private void Start()
    {
        instance = this;

        camTransform = Camera.main.transform;
        playerTransform = camTransform.parent;
    }

    // Update is called once per frame
    void Update()
    {
        //plane = new Plane(getPlaneAxis(), planeOffset);
        plane.SetNormalAndPosition(getPlaneAxis(),moveGiz.position);

        float distance;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (plane.Raycast(ray, out distance))
        {
            worldPos = ray.GetPoint(distance);
        }

        if (mouseDown)
        {
            moveGiz.position = worldPos;
        }
    }

    Vector3 getPlaneAxis()
    {
        float xAng = camTransform.localEulerAngles.x;
        float yAng = playerTransform.localEulerAngles.y; //between -180 to 180
        if (xAng > 180) xAng -= 360;
        if (yAng > 180) yAng -= 360;

        if (xAng >= 45) return Vector3.up; //Looking down
        if (xAng <= -45) return Vector3.down; //looking up

        if (yAng >= -45 && yAng <=45) return Vector3.back; //facing forwards
        if(yAng >45 && yAng<135) return Vector3.left; //facing right
        if(yAng >-135 && yAng<-45) return Vector3.right; //facing left

        return Vector3.forward; //facing backwards
    }

    private void OnDrawGizmos()
    {
        if (UnityEditor.EditorApplication.isPlaying)
        {
            //draw sphere grid of mouse world positions
            Gizmos.color = Color.yellow;
            if (mouseDown) Gizmos.color = Color.red;
            Gizmos.DrawSphere(worldPos, 0.15f);

            Gizmos.color = Color.magenta;
            Gizmos.DrawCube(camTransform.position + (plane.normal * plane.distance), Vector3.one * 0.1f);
        }
    }

    public void MouseDown(InputAction.CallbackContext ctx)
    {
        mouseDown = ctx.performed;
    }
}
