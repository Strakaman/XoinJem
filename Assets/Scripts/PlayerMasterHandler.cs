using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerMasterHandler : NetworkBehaviour
{
    public SpriteRenderer spr;
    public PlayerMovementInput inputMovement;
    PlayerHUD myHUD;
    public GameObject winnerIcon;

    /// </summary>
    //[SyncVar (hook ="OnScoreChanged")]
    public int score = 0;

    [SyncVar]
    public int num;

    public string pName = "";

    static Color[] playercolors = new Color[] { Color.yellow, Color.magenta, Color.green, Color.blue};

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

    //[Command]
    public void IncreaseScore()
    {
        //score++;
        RpcSetScore(score + 1);
    }

    void OnScoreChanged(int newScore)
    {
        myHUD.UpdateYourScore(newScore);
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
    public void RpcDeactivateYourMovement()
    {
        inputMovement.enabled = false;
    }

    [ClientRpc]
    public void RpcSetScore(int newScore)
    {
        score = newScore;
        Debug.Log("rpc received increase score hit " + score);
        OnScoreChanged(score);
        //server sent rpc to update score, have each local player figure out if they won or lost
        if (score >= StaticGameData.instance.scoreNeededToWin)
        {
            GameOver(num);
        }
        else
        {
            Debug.Log(pName + "has this " + score + " points but needs a total of " + StaticGameData.instance.scoreNeededToWin + " to win.");

        }
    }

    [ClientRpc]
    public void RpcResetForRematch()
    {
        score = 0;
        OnScoreChanged(score);
        inputMovement.enabled = true;
        winnerIcon.SetActive(false);
        myHUD.StopVictoryFlicker();
        myHUD.UpdateBGColor(playercolors[num]);
    }
}
