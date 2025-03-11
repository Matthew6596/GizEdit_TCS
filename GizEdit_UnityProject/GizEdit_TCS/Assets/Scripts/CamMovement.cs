using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CamMovement : MonoBehaviour
{
    public static GameObject player;

    public float maxSpeed;
    public float acceleration;
    public float friction;


    bool sp = false, sh=false, rightClicking=false;
    Vector2 moveVals;
    Vector3 velocity;
    // Start is called before the first frame update
    void Start()
    {
        player = gameObject;
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

        velocity = Vector3.ClampMagnitude(velocity, acceleration*maxSpeed);

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

    private float mult = 1;
    private bool prevScrollPositive = false;
    public void mouseScroll(InputAction.CallbackContext ctx)
    {
        float val = ctx.ReadValue<float>() / 500f; //aprox. scroll step?
        if (val == 0) return;

        mult = ((acceleration>1)?.05f:1) * Mathf.Pow(acceleration, 2);

        acceleration += val*mult;
        acceleration = Mathf.Clamp(acceleration, 0.01f, 20);
    }
}
