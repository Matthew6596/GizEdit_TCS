using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CamMovement : MonoBehaviour
{
    public float maxSpeed;
    public float acceleration;
    public float friction;


    bool sp = false, sh=false, rightClicking=false;
    Vector2 moveVals;
    Vector3 velocity;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Movement
        if (rightClicking)
        {
            velocity.x += moveVals.x * acceleration;
            velocity.z += moveVals.y * acceleration;
            if (sp) velocity.y += acceleration;
            if (sh) velocity.y -= acceleration;
        }

        velocity *= friction;

        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);

        transform.Translate(velocity * Time.deltaTime);
    }

    //Input Methods
    public void wasdMovement(InputAction.CallbackContext ctx)
    {
        moveVals = ctx.ReadValue<Vector2>();
    }
    public void spacePress(InputAction.CallbackContext ctx)
    {
        sp = ctx.performed;
    }
    public void shiftPress(InputAction.CallbackContext ctx)
    {
        sh = ctx.performed;
    }

    public void RClick(InputAction.CallbackContext ctx)
    {
        rightClicking = ctx.performed;
        if(rightClicking)velocity = Vector3.zero;
    }
}
