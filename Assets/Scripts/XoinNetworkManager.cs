using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.Networking.NetworkSystem;
using System;

public class XoinNetworkManager : NetworkManager {

    public PlayerMasterHandler[] playersList;
    bool gameActive = false;
    bool showReplayButton = false;
    //public BGMManager myBGMPlayer;
    //public GemSpawner gemSpawner;

    public Coroutine spawnCoroutine;
    // Use this for initialization
    void Start()
    {
        Debug.Log("start Network Manager");
        playersList= new PlayerMasterHandler[matchSize];
    }

    private void OnGUI()
    {
        if (showReplayButton)
            if (GUI.Button(new Rect(Screen.width - 150, 25, 100, 20), "Play Again?"))
            {
                PlayAgain();
            }
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
            foreach (PlayerMasterHandler nc in playersList)
            {
                if (nc != null)
                {
                    summary += "(" + nc.num + ")";
                }
            }
            Debug.Log("Network Manager Summary " + summary);
        }
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        StartCoroutine(WaitForGameToStart());
    }
    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);
        HandlePlayerCountChange();
    }
    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        Debug.Log("hit add player");
        base.OnServerAddPlayer(conn, playerControllerId);
        AddPlayer(conn);
        HandlePlayerCountChange();
        if (gameActive) { return; }//if the game is already active, no need to retrigger check
        if (IsGameReadyToStart())
        {
            gameActive = true;
        }
    }

    public void AddPlayer(NetworkConnection newPlayerConn)
    {
        for (int i=0;i<playersList.Length;i++)
        {
            if (playersList[i] == null)
            {
                PlayerMasterHandler playerData = newPlayerConn.playerControllers[0].gameObject.GetComponent<PlayerMasterHandler>();
                playerData.num = i;
                playersList[i] = playerData;
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
        BGMManager.instance.SendRPCToPlayClip(res);
    }

    void EndGame()
    {
        gameActive = false;
    }

    void HandlePlayerCountChange()
    {
        bool[] playerSlots = new bool[matchSize]; //true means there is a player at that slot
        for (int i=0; i < playersList.Length;i++)
        {
            if (playersList[i] != null)
            {
                playerSlots[i] = true;
            }
        }
        MultiPlayerHUDManager.instance.RpcActivateBasedOnActivePlayers(playerSlots);
    }
    internal void DeactivateLosersMovement(int winnerNum)
    {
        if (!gameActive) { return; } //we did this already
        gameActive = false;
        BGMManager.instance.RpcPlayVictoryMusic();
        showReplayButton = true;
        foreach (PlayerMasterHandler playa in playersList)
        {
            if (playa!= null && playa.num != winnerNum)
                {
                playa.RpcDeactivateYourMovement();
            }
        }
    }

    void PlayAgain()
    {
        /// <summary>
        /// TODO: ADD RESET
        /// RESTART GEM LOOP
        showReplayButton = false;
        foreach(PlayerMasterHandler pmh in playersList)
        {
            if (pmh != null)
            {
                pmh.RpcResetForRematch();
            }
        }
        if (IsGameReadyToStart())
        {
            gameActive = true;
        }
        StartCoroutine(WaitForGameToStart());
        Debug.Log("HIT");
    }
}
