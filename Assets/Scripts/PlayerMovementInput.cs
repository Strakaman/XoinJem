using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerMovementInput : NetworkBehaviour
{
	public CharacterController2D controller;
    public Animator animator;
	public float runSpeed = 40f;
    float horizontalMove = 0f;

    [SyncVar]
    bool facingRight = true;

    [SyncVar]
    bool jump = false;

    [SyncVar]
	bool crouch = false;

    // Update is called once per frame
    void Update () {
        if (isLocalPlayer)
        {
           
            horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
            int testInt = facingRight ? 1 : -1;
            if (horizontalMove != 0 && testInt != Mathf.Sign(horizontalMove))
            {
                CmdSetFacingDirection(Mathf.Sign(horizontalMove)>0);
            }
            if (Input.GetButtonDown("Jump"))
            {
                jump = true;
               // CmdSetJumpVal(true);
            }

            if (Input.GetButtonDown("Crouch"))
            {
                //playerInputCrouch = true;
                CmdSetCrouchVal(true);
                animator.SetBool("Crouch", true);

            }
            else if (Input.GetButtonUp("Crouch"))
            {
                //playerInputCrouch = false;
                //animator.SetBool("Crouch", false);
                CmdSetCrouchVal(false);
            }
            animator.SetFloat("Speed", Mathf.Abs(horizontalMove));
            //animator.SetBool("Crouch", (playerMustCrouch || playerInputCrouch));
            animator.SetBool("Jump", jump);
        }

	}

    [Command]
    public void CmdSetJumpVal(bool val)
    {
        //jump = val;
    }

    [Command]
    public void CmdSetFacingDirection(bool val)
    {
        facingRight = val;
    }

    [Command]
    public void CmdSetCrouchVal(bool val)
    {
        crouch = val;
    }

    public void OnLanded()
    {
        Debug.Log("landed");
        if (isLocalPlayer)
        {
            jump = false;
            //CmdSetJumpVal(false);
        }
    }

    public void OnCrouchEvent(bool isCrouching)
    {

        if (isLocalPlayer)
        {
            animator.SetBool("Crouch", isCrouching);

            //playerMustCrouch = isCrouching;
            //CmdSetCrouchVal(isCrouching);
        }
    }

    void FixedUpdate ()
	{
        // Move our character i
        if (isLocalPlayer)
        {
            controller.Move(horizontalMove * Time.fixedDeltaTime, crouch, jump);
        }
        else
        {
            controller.EvalCrouchSituation(crouch);
            controller.EvalFlipSituation((facingRight ? 1 : -1));
        }
    }
}
