using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    public float forwardSpeed = 5.0f;
    public float sideSpeed = 4.0f;
    public float lookSpeed = 5.0f;
    public float vertLookRange = 60.0f;
    public float jumpPower = 10.0f;
    public float sprintMultiplier = 1.5f;

    private CharacterController myController;
    private float rotVertical = 0.0f;
    
    private float fallSpeed = 0.0f;

	// Use this for initialization
	void Start()
	{
        myController = GetComponent<CharacterController>();
	}

	// Update is called once per frame
	void Update()
	{
        #region Movement
        float forward = Input.GetAxis("LeftStickVertical") * forwardSpeed;
        float side = Input.GetAxis("LeftStickHorizontal") * sideSpeed;

        if (Input.GetButton("Sprint"))
        {
            forward *= sprintMultiplier;
        }
        #endregion

        #region Look Rotation
        float rotHorizontal = Input.GetAxis("RightStickHorizontal") * lookSpeed;
        rotVertical -= Input.GetAxis("RightStickVertical") * lookSpeed;
        rotVertical = Mathf.Clamp(rotVertical, -vertLookRange, vertLookRange);

        transform.Rotate(0, rotHorizontal, 0);
        Camera.main.transform.localRotation = Quaternion.Euler(rotVertical, 0, 0);
        #endregion

        #region Gravity
        fallSpeed += Physics.gravity.y * Time.deltaTime;
        #endregion

        if (myController.isGrounded && Input.GetButtonDown("Jump"))
        {
            fallSpeed = jumpPower;
        }

        Vector3 speed = transform.rotation * new Vector3(side, fallSpeed, forward);
        myController.Move(speed * Time.deltaTime);
	}
}