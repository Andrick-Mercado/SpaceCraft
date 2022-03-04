using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharController : MonoBehaviour
{
    //Movement/Camera properties
    public float walkSpeed = 7f;
    public float runSpeed = 11f;
    public float jumpSpeed = 8f;
    public float gravity = 20f;
    public float camSpeed = 2f;
    public float lookUpDownLimit = 45f;



    //Grab reference to player Camera
    public Camera playerCamera;
    //Declare reference to character controller inspector component
    CharacterController characterController;

    //Vars used to calculate Camera and Player movement
    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;
    float curSpeedX;
    float curSpeedY;

    //Bool to use in future to dictate when player is not allowed to move
    public bool canMove = true;

    bool camAtPlayer = false;

    void Start()
    {
        //Grab reference to character controller component
        characterController = GetComponent<CharacterController>();



        //Locks and makes Mouse Cursor invisible when focused on game window
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        //Get transform position for forward and right based on current direction facing
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        

        //If canMove, if playing holding shift, apply running speed, else, apply walking speed
        if (canMove)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                curSpeedX = runSpeed * Input.GetAxis("Vertical");
                curSpeedY = runSpeed * Input.GetAxis("Horizontal");
            }
            else
            {
                curSpeedX = walkSpeed * Input.GetAxis("Vertical");
                curSpeedY = walkSpeed * Input.GetAxis("Horizontal");
            }
        }
        else
        {
            curSpeedX = curSpeedY = 0;
        }

        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        //If able to move and not on ground and input = jump, make char jump, else, char is moving on ground
        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpSpeed;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        //If Char not on ground, apply gravity
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        //Move Character
        characterController.Move(moveDirection * Time.deltaTime);

        //Camera Rotations
        if (canMove)
        {
            //Calculate X rotation movement
            rotationX += -Input.GetAxis("Mouse Y") * camSpeed;
            //Calculate X rotation clamp so character doesent bend neck backwards or into self
            rotationX = Mathf.Clamp(rotationX, -lookUpDownLimit, lookUpDownLimit);

            //Rotate Camera on X axis
            if (camAtPlayer)
            {
                playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 180, 0);
            }
            else playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            //Rotate Player Character on Y axis
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * camSpeed, 0);
        }
        //First Person
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            camAtPlayer = false;
            playerCamera.transform.localPosition = new Vector3(0,1,0);
        }
        //Third Person
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            camAtPlayer = false;
            playerCamera.transform.localPosition = new Vector3(0,2,-5);
        }
        //Third Person Looking at Character
        else if (Input.GetKey(KeyCode.Alpha3))
        {
            camAtPlayer = true;
            playerCamera.transform.localPosition = new Vector3(0, 2, 5);
        }
    }
}
