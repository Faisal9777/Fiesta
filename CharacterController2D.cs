using System;
using UnityEngine;
using UnityEngine.Events;

public class CharacterController2D : MonoBehaviour
{
	[SerializeField] private float m_JumpForce = 400f;							// Amount of force added when the player jumps.
	[Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f;			// Amount of maxSpeed applied to crouching movement. 1 = 100%
	[Range(0, .3f)] [SerializeField] internal float m_MovementSmoothing = .05f;	// How much to smooth out the movement
	[SerializeField] private bool m_AirControl = false;							// Whether or not a player can steer while jumping;
	[SerializeField] private LayerMask m_WhatIsGround;							// A mask determining what is ground to the character
	[SerializeField] private Transform m_GroundCheck;							// A position marking where to check if the player is grounded.
	[SerializeField] private Transform m_CeilingCheck;							// A position marking where to check for ceilings
	[SerializeField] private Collider2D m_CrouchDisableCollider;                // A collider that will be disabled when crouching
	public float SpeedModifier { get; set; } = 1f;
	const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
	private bool m_Grounded;            // Whether or not the player is grounded.
	const float k_CeilingRadius = .2f; // Radius of the overlap circle to determine if the player can stand up
	private Rigidbody2D m_Rigidbody2D;
	private bool m_FacingRight = true;  // For determining which way the player is currently facing.



    protected Vector3 m_Velocity = Vector3.zero;
	public static float MovementSmoothing { get; } = .05f;
	private Run run;
	private Jump jump;
	private Dash dash;
    /*protected Vector3 M_Velocity 
	{
        get
        {
			return m_Velocity;
        } 
		 
	}

	protected float M_MovementSmoothing
	{
		get
		{
			return m_MovementSmoothing;
		}

	}*/
    [Header("Events")]
	[Space]

	public UnityEvent OnLandEvent;

	[System.Serializable]
	public class BoolEvent : UnityEvent<bool> { }

	public BoolEvent OnCrouchEvent;
	private bool m_wasCrouching = false;

	private void Awake()
	{
		m_Rigidbody2D = GetComponent<Rigidbody2D>();

		if (OnLandEvent == null)
			OnLandEvent = new UnityEvent();

		if (OnCrouchEvent == null)
			OnCrouchEvent = new BoolEvent();
	}

	private void FixedUpdate()
	{
		bool wasGrounded = m_Grounded;
		m_Grounded = false;

		// The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
		// This can be done using layers instead but Sample Assets will not overwrite your project settings.
		Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject)
			{
				m_Grounded = true;
				if (!wasGrounded)
					OnLandEvent.Invoke();
			}
		}
	}

    public void Initialize(Run run, Jump jump, Dash dash, bool canAirControl)
    {
		this.run = run;
		this.jump = jump;
		this.dash = dash;
		m_AirControl = canAirControl;
    }

    //Instead of the move and flip of the following two the static one will ve used instead
    public void Move(float move, bool crouch, bool jump, float moveDistance = 10f, float dashSpeed = 1, bool flip = true)
	{
		//Debug.Log("We go to move" + move);
		// If crouching, check to see if the character can stand up
		if (!crouch)
		{
			// If the character has a ceiling preventing them from standing up, keep them crouching
			if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
			{
				crouch = true;
			}
		}

		//only control the player if grounded or airControl is turned on
		if (m_Grounded || m_AirControl)
		{

			// If crouching
			if (crouch)
			{
				if (!m_wasCrouching)
				{
					m_wasCrouching = true;
					OnCrouchEvent.Invoke(true);
				}

				// Reduce the speed by the crouchSpeed multiplier
				move *= m_CrouchSpeed;

				// Disable one of the colliders when crouching
				if (m_CrouchDisableCollider != null)
					m_CrouchDisableCollider.enabled = false;
			} else
			{
				// Enable the collidejr when not crouching
				if (m_CrouchDisableCollider != null)
					m_CrouchDisableCollider.enabled = true;

				if (m_wasCrouching)
				{
					m_wasCrouching = false;
					OnCrouchEvent.Invoke(false);
				}
			}

			// Move the character by finding the target velocity
			if(dashSpeed > 1)
            {
				Debug.Log("The dash speed is " + m_MovementSmoothing/dashSpeed);
			}
			
			Vector3 targetVelocity = new Vector2(move*moveDistance, m_Rigidbody2D.velocity.y* SpeedModifier);
			// And then smoothing it out and applying it to the character
			m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing/dashSpeed);

            // If the input is moving the player right and the player is facing left...
            if (flip)
            {
				if (move > 0 && !m_FacingRight)
				{
					// ... flip the player.
					Flip();
				}
				// Otherwise if the input is moving the player left and the player is facing right...
				else if (move < 0 && m_FacingRight)
				{
					// ... flip the player.
					Flip();
				}
			}
			
		}
		// If the player should jump...
		if (m_Grounded && jump)
		{
			// Add a vertical force to the player.
			m_Grounded = false;
			m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
		}
	}

	public void Run<T>(Vector2 target)
    {
		if (m_Grounded || m_AirControl)
		{

			run.Execute<T>(target);

		}
		
	}
	public void Jump<T>()
	{
        if (m_Grounded)
        {
			jump.Execute<T>();
        }
		
	}
#nullable enable
	public ITask? Dash<T>(Vector2 target)
	{
		if (m_Grounded || m_AirControl)
		{

			return dash.Execute<T>(target);

		}
		return null;
	}
	public ITask? Dash<T>()
	{
		if (m_Grounded || m_AirControl)
		{

			return dash .Execute<T>();

		}
		return null;

	}

	public void Crouch()
    {

    }


	private void Flip()
	{
		// Switch the way the player is labelled as facing.
		m_FacingRight = !m_FacingRight;

		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

	public static void MoveTo(IMovable movable, Vector2 target, Vector3 currentVelocity, float movementSmoothing)
	{


		// And then smoothing it out and applying it to the character
		movable.Velocity = Vector3.SmoothDamp(movable.Velocity, target, ref currentVelocity, movementSmoothing);

		
		if(target.x > 0 && !movable.IsFacingRight)
        {
			Flip(movable);
        }
		else if (target.x < 0 && movable.IsFacingRight)
		{
			// ... flip the player.
			Flip(movable);
		}

	}


	private static void Flip(IMovable movable)
	{
		// Switch the way the player is labelled as facing.
		movable.IsFacingRight = !movable.IsFacingRight;

		// Multiply the player's x local scale by -1.
		Vector3 theScale = movable.Transform.localScale;
		theScale.x *= -1;
		movable.Transform.localScale = theScale;
	}
}
