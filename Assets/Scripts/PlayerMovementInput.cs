using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerMovementInput : NetworkBehaviour
{
    public SpriteRenderer spr;
	public CharacterController2D controller;
    public Animator animator;
	public float runSpeed = 40f;
    float horizontalMove = 0f;
    PlayerHUD myHUD;
    public GameObject winnerIcon;

    [SyncVar]
    bool facingRight = true;

    [SyncVar]
    bool jump = false;

    [SyncVar]
	bool crouch = false;

    //[SyncVar (hook ="OnScoreChanged")]
    public int score = 0;

    [SyncVar]
    public int num;

    public string pName = "";

    static Color[] playercolors = new Color[] { Color.yellow, Color.magenta, Color.green, Color.blue};

    void OnGUI()
    {
        //GUI.Label(new Rect(0,0,200, 50), pName);
    }

    private void OnEnable()
    {
        Invoke("PostSummoning", .5f);
        //nameLabel.text = pName;
    }

    void PostSummoning()
    {
        Debug.Log("Player num " + num);
        pName = "Player " + (num + 1);
        spr.color = playercolors[num];
        myHUD = MultiPlayerHUDManager.instance.RetrieveHUD(num);
        myHUD.UpdateYourScore(0);
        myHUD.UpdateBGColor(playercolors[num]);
        myHUD.UpdateYourName(pName);
    }
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

    //[Command]
    public void IncreaseScore()
    {
        //score++;
        Debug.LogWarning("command increase score hit- old score" + score);
        RpcSetScore(score + 1);
    }


    [Command]
    public void CmdSetYourName(string nombre)
    {
        pName = nombre;
    }

    void OnScoreChanged(int newScore)
    {
        myHUD.UpdateYourScore(newScore);
    }
    public void OnLanded()
    {
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

    [ClientRpc]
    public void RpcSetYourColor(Color c)
    {
        spr.color = c;
    }

    [ClientRpc]
    public void RpcSetYourName(string nombre)
    {
        pName = nombre;
    }

    public void GameOver(int winnerNum)
    {
        //controller.enabled = false;
        if (winnerNum == num)
        {
            winnerIcon.gameObject.SetActive(true);
            StartCoroutine(myHUD.VictoryFlicker());
        }
        Debug.Log("Winner is Player " + (winnerNum +1)+". I am " +pName);
        //this.enabled = false;
        if (isServer)
        { //server version of this client should find the network manager and tell it to deactivate non winners
            XoinNetworkManager serverNetworkManager = FindObjectOfType(typeof(XoinNetworkManager)) as XoinNetworkManager;
            serverNetworkManager.DeactivateLosersMovement(winnerNum);
        }
    }

    [ClientRpc]
    public void RpcDeactivateYourself()
    {
        this.enabled = false;
    }

    [ClientRpc]
    public void RpcSetScore(int newScore)
    {
        score = newScore;
        Debug.LogWarning("command increase score hit " + score);
        OnScoreChanged(score);
        //if (hasAuthority) //only trigger once, might as well have local player figure it out
        {
            if (score >= StaticGameData.instance.scoreNeededToWin)
            {
                GameOver(num);
            }
            else
            {
                Debug.Log(pName + "has this " + score + " points but needs a total of " + StaticGameData.instance.scoreNeededToWin + " to win.");
            }
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
