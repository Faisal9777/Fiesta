using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    
    
    public Character character;
    public CharacterController2D controller;
    public Animator animator;
    private AttackProcessor processor;

    public float RunSpeed { get; set; } = 20f;
    //float runSpeed = 20f;
    float horizontalMove = 0f;
    float moveDirection;
    bool jump = false;
    bool crouch = false;
    bool moving = false;
    
    // Start is called before the first frame update
    private void Awake()
    {
        character = GetComponent<Character>();
        processor = character.GetMainProcessor();
    }

    // Update is called once per frame
    void Update()
    {
        if (character.CanMove)
        {
            moveDirection = Input.GetAxisRaw("Horizontal");
            horizontalMove = moveDirection * RunSpeed;
            //Debug.Log("We hit" + horizontalMove);
            animator.SetFloat("Speed", Mathf.Abs(horizontalMove));


            if (Input.GetButtonDown("Jump"))
            {
                jump = true;
                animator.SetBool("IsJumping", true);
            }

            if (Input.GetButtonDown("Horizontal"))
            {
                moving = true;



            }
            else if (Input.GetButtonUp("Horizontal"))
            {
                moving = false;
            }

            if (moving && Input.GetButtonDown("Dodge"))
            {
                MovementCalculator calculator = new MovementCalculator();//this class may get deleted check
                Debug.Log("Dashing");
                float dashSpeed = character.GetStats("agility");
                float dashDistance = character.GetStats("dexterity");
                character.Dash(dashSpeed, dashDistance, moveDirection);
            }



            if (Input.GetButtonDown("Crouch"))
            {
                crouch = true;
            }
            else if (Input.GetButtonUp("Crouch"))
            {
                crouch = false;
            }
        }
        
    }

    public static PlayerMovement InitializePlayerMovement(Character character, AttackProcessor processor, PlayerMovement movement = null)
    {
        if(movement == null)
        {
            movement = (PlayerMovement)(Activator.CreateInstance(typeof(PlayerMovement)));
        }

        return movement.Initialize(character, processor);
    }

    private PlayerMovement Initialize(Character character, AttackProcessor processor)
    {
        if(this.character == null || this.character != character)
        {
            this.character = character;
        }
        if (this.processor == null || this.processor != processor)
        {
            this.processor = processor;
        }
        return this;
    }

    public void onLand()
    {
        animator.SetBool("IsJumping", false);

    }

    public void SetDefault()
    {
        RunSpeed = 20f;
    }

    void FixedUpdate()
    {
        
        controller.Move(horizontalMove * Time.fixedDeltaTime, crouch, jump);
        jump = false;
    }
}

