using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AdvMouseInput : MonoBehaviour
{
    public static AdvMouseInput instance;
    public static Vector3 GetMoveGizPos() { return instance.moveGiz.GetChild(0).position; }

    public Vector3 worldPos;
    public float planeOffset;
    Plane plane;

    Transform camTransform, playerTransform;

    //Editor stuff
    bool mouseDown = false;
    public Transform moveGiz;

    Vector3 axisLock = Vector3.one;
    string moveGizAxis;

    GameObject prevHitObj;

    private void Start()
    {
        instance = this;

        camTransform = Camera.main.transform;
        playerTransform = camTransform.parent;
    }

    // Update is called once per frame
    void Update()
    {
        EditorGizOutlineGlow();

        //plane = new Plane(getPlaneAxis(), planeOffset);
        plane.SetNormalAndPosition(getPlaneAxis(moveGizAxis),moveGiz.position);

        float distance;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (plane.Raycast(ray, out distance))
        {
            worldPos = ray.GetPoint(distance);
        }

        if (mouseDown)
        {
            Vector3 moveOffset = MultiplyVec3s(worldPos, axisLock);
            Vector3 moveGizPos = moveGiz.position;

            if (moveOffset.x != 0) moveGizPos.x = moveOffset.x;
            if (moveOffset.y != 0) moveGizPos.y = moveOffset.y;
            if (moveOffset.z != 0) moveGizPos.z = moveOffset.z;

            moveGiz.position = moveGizPos;
            if(EditorManager.instance!=null)EditorManager.instance.UpdateSelectedPos();
        }

        //Move giz faces camera
        Transform childT = moveGiz.GetChild(0);
        Vector3 childPos = childT.position, camPos=Camera.main.transform.position;
        childT.localScale = new(childPos.x < camPos.x ? 1 : -1, childPos.y < camPos.y ? 1 : -1, childPos.z < camPos.z ? 1 : -1);
    }

    /*Vector3 getPlaneAxis()
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
    }*/

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
        if (mouseDown)
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
            {
                GameObject hitObj = hit.transform.gameObject;
                if (hitObj.CompareTag("editorGiz"))
                {
                    //Get giz axis
                    moveGizAxis = hitObj.name[(hitObj.name.IndexOf("move") + 4)..];

                    //Set plane and get world position
                    plane.SetNormalAndPosition(getPlaneAxis(moveGizAxis), moveGiz.position);
                    float distance;
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    if (plane.Raycast(ray, out distance))
                    {
                        worldPos = ray.GetPoint(distance);
                    }
                    //Offset Children
                    moveGiz.GetChild(0).localPosition = 10*MultiplyVec3s(moveGiz.position-worldPos, axisLock);
                }
            }
        }
        else
        {
            moveGizAxis = "";
            //Reset children offset
            moveGiz.position = moveGiz.GetChild(0).position;
            moveGiz.GetChild(0).localPosition = Vector3.zero;
        }
    }

    Vector3 getPlaneAxis(string axis)
    {
        float xAng = camTransform.localEulerAngles.x;
        float yAng = playerTransform.localEulerAngles.y; //between -180 to 180
        if (xAng > 180) xAng -= 360;
        if (yAng > 180) yAng -= 360;

        //Debug.Log(GetClosestPlaneAngle(xAng, yAng));

        switch (axis)
        {
            case ("X"):
                axisLock = Vector3.right;
                if (xAng >= 45 || xAng <= -45) return Vector3.up; //<<Need better calculation for which plane works best
                return Vector3.forward;
            case ("Y"):
                axisLock = Vector3.up;
                if ((yAng > 45 && yAng < 135) || (yAng > -135 && yAng < -45)) return Vector3.right;
                return Vector3.forward;
            case ("Z"):
                axisLock = Vector3.forward;
                if (xAng >= 45 || xAng <= -45) return Vector3.up;
                return Vector3.right;
            case ("XZ"):
                axisLock = Vector3.one;
                return Vector3.up;
            case ("XY"):
                axisLock = Vector3.one;
                return Vector3.forward;
            case ("ZY"):
                axisLock = Vector3.one;
                return Vector3.right;
            default: 
                axisLock = Vector3.zero;
                return Vector3.up;
        }
    }
    public Vector3 PlaneAngleCloseness(float xAng, float yAng) //Vec3: updown, leftright, forwardback
    {
        Vector3 closeness = new();

        if(xAng>=0)closeness.x = 90-xAng; //updown
        else closeness.x = 90+xAng;

        if(yAng > 0) //right
            closeness.y = 90 - yAng;
        else if (yAng < 0) //left
            closeness.y = 90+ yAng;

        if (closeness.y > 0) //check front for z
            closeness.z = yAng;
        else //check back for z
        {
            if (yAng >= 0) closeness.z = yAng-180;
            else closeness.z = 180+yAng;
        }
        return closeness;
    }
    public string GetClosestPlaneAngle(float xAng, float yAng)
    {
        Vector3 c = PlaneAngleCloseness(xAng, yAng);
        for (int i = 0; i < 3; i++) c[i] = Mathf.Abs(c[i]);

        if ((xAng >= 45 || xAng <= -45) || (c.x <= c.y && c.x <= c.z)) return "X"; //updown gets special priority
        if (c.y <= c.x && c.y <= c.z) return "Y";
        if (c.z <= c.y && c.z <= c.x) return "Z";

        return null;
    }

    public static Vector3 MultiplyVec3s(Vector3 vec1, Vector3 vec2)
    {
        return new Vector3(vec1.x * vec2.x, vec1.y * vec2.y, vec1.z * vec2.z);
    }

    public void EditorGizOutlineGlow()
    {
        if (prevHitObj != null)
        {
            //Turn off prevHitObj glow
            prevHitObj.GetComponent<Renderer>().materials[1].SetFloat("_Size",0);
            prevHitObj = null;
        }

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
        {
            GameObject hitObj = hit.transform.gameObject;
            if (hitObj.CompareTag("editorGiz"))
            {
                //Make hitObj glow
                hitObj.GetComponent<Renderer>().materials[1].SetFloat("_Size", 1.1f);
                prevHitObj = hitObj;
            }
        }
    }
}
