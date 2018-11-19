using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.Networking.NetworkSystem;
using System;

public class XoinNetworkManager : NetworkManager {

    public PlayerMovement[] playersList;
    bool gameActive = false;
    //public BGMManager myBGMPlayer;
    //public GemSpawner gemSpawner;

    public Coroutine spawnCoroutine;
    // Use this for initialization
    void Start()
    {
        Debug.Log("start Network Manager");
        playersList= new PlayerMovement[matchSize];
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F3))
        {
            string summary = "";
            //foreach (byte score in scores)
            {
           //     summary += "[" + score + "]";
            }
            foreach (PlayerMovement nc in playersList)
            {
                if (nc != null)
                {
                    summary += "(" + nc.num + ")";
                }
            }
            Debug.Log("Network Manager Summary " + summary);
        }
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        Debug.Log("hit add player");
        base.OnServerAddPlayer(conn, playerControllerId);
        AddPlayer(conn);

        if (gameActive) { return; }//if the game is already active, no need to retrigger courintine
        if (IsGameReadyToStart())
        {
            gameActive = true;
        }
        StartCoroutine(WaitForGameToStart());
    }

    public void AddPlayer(NetworkConnection newPlayerConn)
    {
        for (int i=0;i<playersList.Length;i++)
        {
            if (playersList[i] == null)
            {
                PlayerMovement playerStuff = newPlayerConn.playerControllers[0].gameObject.GetComponent<PlayerMovement>();
                playerStuff.num = i;
                playersList[i] = playerStuff;
                //playerStuff.RpcSetYourColor(playercolors[i]);
                //playerStuff.RpcSetYourName("Player " + (i+1));
                break;
            }
        }
    }

    public bool IsGameReadyToStart()
    {
        int count = 0;
        for (int j = 0; j < playersList.Length; j++)
        {
            if (playersList[j] != null)
            {
                count++;
            }
        }
        return count >= StaticGameData.instance.playersNeededForMatch;
    }

    IEnumerator WaitForGameToStart()
    {
        while (!gameActive) { yield return new WaitForSeconds(1f); }
        StartGame();
    }

    void StartGame()
    {
        spawnCoroutine = StartCoroutine(GemSpawner.instance.SpawnLoop());
        int res = UnityEngine.Random.Range(0,BGMManager.instance.HowManyClipsYouGot()); //play from random list of playable songs
        BGMManager.instance.playOnJoinClipIndex = res;
        BGMManager.instance.SendRPCToPlayClip();
    }

    void EndGame()
    {
        gameActive = false;
    }

    internal void DeactivateLosersMovement(int winnerNum)
    {
        gameActive = false;
        foreach (PlayerMovement playa in playersList)
        {
            if (playa!= null && playa.num != winnerNum)
                {
                playa.RpcDeactivateYourself();
            }
        }
    }
}
