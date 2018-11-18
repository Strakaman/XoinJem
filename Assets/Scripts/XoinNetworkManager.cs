using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.Networking.NetworkSystem;

public class XoinNetworkManager : NetworkManager {

    public NetworkConnection[] playersList;
    bool gameActive = false;
    public BGMManager myBGMPlayer;
    public GemSpawner gemSpawner;
    public int playersNeeded = 2;

    public Coroutine spawnCoroutine;
    // Use this for initialization
    static Color[] playercolors;
    void Start()
    {
        Debug.Log("start hit");
        playersList= new NetworkConnection[matchSize];
        playercolors = new Color[] { Color.white, Color.gray, Color.green, Color.blue };
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
            foreach (NetworkConnection nc in playersList)
            {
                if (nc != null)
                {
                    summary += "(" + nc.connectionId + ")";
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
                playersList[i] = newPlayerConn;
                PlayerMovement playerStuff = newPlayerConn.playerControllers[0].gameObject.GetComponent<PlayerMovement>();
                playerStuff.num = i;
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
        spawnCoroutine = StartCoroutine(gemSpawner.SpawnLoop());
        int res = Random.Range(0,3);
        myBGMPlayer.SendRPCToPlayClip(res);
    }

    void EndGame()
    {
        gameActive = false;
    }

    public void CoinHit(uint pID)
    {
        Debug.Log("looking for pid: " +pID);
        for (int j=0; j < playersList.Length;j++)
        {
            if (playersList[j] != null) Debug.Log("asda " + playersList[j].connectionId + playersList[j].hostId);
            if (playersList[j] != null && playersList[j].connectionId == pID)
            {
                //scores[j]++;
                break;
            }
        }
        Debug.Log("player gone, should not be hit");
    }
}
