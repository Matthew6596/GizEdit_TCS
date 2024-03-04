using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseLook : MonoBehaviour
{
    public enum RotationAxis{
        MouseXY=0, MouseX=1, MouseY=2
    }

    public RotationAxis axis = RotationAxis.MouseXY;
    public float sensitivityX=10f, sentitivityY=10f;
    public float minVertical=-80f, maxVertical=80f;

    float mouseX, mouseY, verticalRotation=0;
    bool rightClicking = false;

    SelectorScript ss;

    // Start is called before the first frame update
    void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if(rb!=null) rb.freezeRotation = true;
        ss = GameObject.Find("EditorManager").GetComponent<SelectorScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (rightClicking)
        {
            switch (axis)
            {
                case RotationAxis.MouseXY: //Both XY
                    verticalRotation -= mouseY * sentitivityY * Time.deltaTime;
                    verticalRotation = Mathf.Clamp(verticalRotation, minVertical, maxVertical);

                    float deltaRotation = mouseX * sensitivityX * Time.deltaTime;
                    float hozRotation = transform.localEulerAngles.y + deltaRotation;

                    transform.localEulerAngles = new Vector3(verticalRotation, hozRotation, 0);
                    break;
                case RotationAxis.MouseX: //Only X
                    transform.Rotate(0, mouseX * sensitivityX * Time.deltaTime, 0);
                    break;
                case RotationAxis.MouseY: //Only Y
                    verticalRotation -= mouseY * sentitivityY * Time.deltaTime;
                    verticalRotation = Mathf.Clamp(verticalRotation, minVertical, maxVertical);

                    transform.localEulerAngles = new Vector3(verticalRotation, transform.localEulerAngles.y, 0);
                    break;
            }
        }
    }

    public void RClick(InputAction.CallbackContext ctx)
    {
        rightClicking = ctx.performed;
        if (rightClicking)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
    public void LookValues(InputAction.CallbackContext ctx)
    {
        //Debug.Log(ctx.ReadValue<Vector2>());
        mouseX = ctx.ReadValue<Vector2>().x;
        mouseY = ctx.ReadValue<Vector2>().y;
        ss.mouseDelta.Set(mouseX,mouseY);
    }

    public void LClick(InputAction.CallbackContext ctx)
    {
        //ss.mouseDelta = new Vector2(mouseX, mouseY);
        if (ctx.performed)
        {
            ss.mouseHeld = true;
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray,out hit, 100f)){
                if (hit.transform.CompareTag("objectGiz"))
                    ss.SelectGizmo(hit.transform.gameObject);
                else if (hit.transform.CompareTag("editorGiz"))
                    ss.selectedEditorGiz = hit.transform.name;
            }
            else
            {
                ss.DeselectGizmo();
            }
        }
        else
        {
            ss.mouseHeld = false;
        }
    }
}
