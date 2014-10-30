//////////////////////////////////////////////////////////////
// FirstPersonControl.cs
//
// FirstPersonControl creates a control scheme where the camera 
// location and controls directly map to being in the first person.
// The left pad is used to move the character, and the
// right pad is used to rotate the character. A quick double-tap
// on the right joystick will make the character jump.
//
// If no right pad is assigned, then tilt is used for rotation
// you double tap the left pad to jump
//////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonControlMobile : MonoBehaviour 
{

	// This script must be attached to a GameObject that has a CharacterController
	public Joystick moveTouchPad;
	public Joystick rotateTouchPad;						// If unassigned, tilt is used

	public Transform cameraPivot;							// The transform used for camera rotation

	public float forwardSpeed = 4;
	public float backwardSpeed = 1;
	public float sidestepSpeed= 1;
	public float jumpSpeed = 8;
	public float inAirMultiplier = 0.25f;					// Limiter for ground speed while jumping
	public Vector2 rotationSpeed = new Vector2( 50, 25 );	// Camera rotation speed for each axis
	public float tiltPositiveYAxis = 0.6f;
	public float tiltNegativeYAxis = 0.4f;
	public float tiltXAxisMinimum = 0.1f;

	private Transform thisTransform;
	public CharacterController character;
	private Vector3 cameraVelocity;
	private Vector3 velocity;						// Used for continuing momentum while in air
	private bool canJump = true;

	// Use this for initialization
	void Start () 
	{
		if(moveTouchPad == null || rotateTouchPad == null)
		{
			Debug.LogError("TouchPad = NULL!!!");
		}
		else
		{
			// Cache component lookup at startup instead of doing this every frame		
			//thisTransform = GetComponent( Transform );
			thisTransform = cameraPivot;
	
			// Move the character to the correct start position in the level, if one exists
			GameObject spawn = GameObject.Find("PlayerSpawn");
			if ( spawn )
				thisTransform.position = spawn.transform.position;
		}
	}
	
	void OnEndGame()
	{
		// Disable joystick when the game ends	
		moveTouchPad.Disable();
		
		if ( rotateTouchPad )
			rotateTouchPad.Disable();	

		// Don't allow any more control changes when the game ends
		this.enabled = false;
	}

	
	// Update is called once per frame
	void Update () 
	{
		Vector3 movement = thisTransform.TransformDirection(new Vector3(moveTouchPad.position.x, 0, moveTouchPad.position.y));
		
		// We only want horizontal movement
		movement.y = 0;
		movement.Normalize();

		// Apply movement from move joystick
		Vector2 absJoyPos = new Vector2( Mathf.Abs( moveTouchPad.position.x ), Mathf.Abs( moveTouchPad.position.y ) );	
		if ( absJoyPos.y > absJoyPos.x )
		{
			if ( moveTouchPad.position.y > 0 )
				movement *= forwardSpeed * absJoyPos.y;
			else
				movement *= backwardSpeed * absJoyPos.y;
		}
		else
			movement *= sidestepSpeed * absJoyPos.x;		
		
		// Check for jump
		if ( character.isGrounded )
		{		
			bool jump = false;
			Joystick touchPad;
			if ( rotateTouchPad )
				touchPad = rotateTouchPad;
			else
				touchPad = moveTouchPad;
		
			if (!touchPad.isFingerDown)
				canJump = true;
			
			if ( canJump && touchPad.tapCount >= 2 )
			{
				jump = true;
				canJump = false;
			}	
			
			if ( jump )
			{
				// Apply the current movement to launch velocity		
				velocity = character.velocity;
				velocity.y = jumpSpeed;	
			}
		}
		else
		{			
			// Apply gravity to our velocity to diminish it over time
			velocity.y += Physics.gravity.y * Time.deltaTime;
					
			// Adjust additional movement while in-air
			movement.x *= inAirMultiplier;
			movement.z *= inAirMultiplier;
		}
			
		movement += velocity;	
		movement += Physics.gravity;
		movement *= Time.deltaTime;
		
		// Actually move the character	
		character.Move( movement );
		
		if ( character.isGrounded )
			// Remove any persistent velocity after landing	
			velocity = Vector3.zero;
		
		// Apply rotation from rotation joystick
		if ( character.isGrounded )
		{
			Vector2 camRotation = Vector2.zero;
			
			if ( rotateTouchPad )
				camRotation = rotateTouchPad.position;
			else
			{
				// Use tilt instead
	//			print( iPhoneInput.acceleration );
				var acceleration = Input.acceleration;
				var absTiltX = Mathf.Abs( acceleration.x );
				if ( acceleration.z < 0 && acceleration.x < 0 )
				{
					if ( absTiltX >= tiltPositiveYAxis )
						camRotation.y = (absTiltX - tiltPositiveYAxis) / (1 - tiltPositiveYAxis);
					else if ( absTiltX <= tiltNegativeYAxis )
						camRotation.y = -( tiltNegativeYAxis - absTiltX) / tiltNegativeYAxis;
				}
				
				if ( Mathf.Abs( acceleration.y ) >= tiltXAxisMinimum )
					camRotation.x = -(acceleration.y - tiltXAxisMinimum) / (1 - tiltXAxisMinimum);
			}
			
			camRotation.x *= rotationSpeed.x;
			camRotation.y *= rotationSpeed.y;
			camRotation *= Time.deltaTime;
			
			// Rotate the character around world-y using x-axis of joystick
			thisTransform.Rotate( 0, camRotation.x, 0, Space.World );
			
			// Rotate only the camera with y-axis input
			cameraPivot.Rotate( -camRotation.y, 0, 0 );
		}
	}
}
