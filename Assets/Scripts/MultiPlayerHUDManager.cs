using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MultiPlayerHUDManager : NetworkBehaviour {

    public PlayerHUD[] allPlayerHUDS;

    public PlayerHUD RetrieveHUD(int playerNum)
    {
        return allPlayerHUDS[playerNum];
    }

    [ClientRpc]
    public void RpcActivateBasedOnActivePlayers(PlayerMovementInput[] playerList)
    {
        for (int index =0;index < playerList.Length; index++)
        {
            if (playerList[index] == null)
            {
                allPlayerHUDS[index].gameObject.SetActive(false);
            }
            else
            {
                allPlayerHUDS[index].gameObject.SetActive(true);
            }
        }
    }
    #region instance
    private static MultiPlayerHUDManager s_Instance = null;
    public static MultiPlayerHUDManager instance
    {
        get
        {
            if (s_Instance == null)
            {
                s_Instance = FindObjectOfType(typeof(MultiPlayerHUDManager)) as MultiPlayerHUDManager;
            }

            if (s_Instance == null)
            {
                Debug.LogWarning("Could not locate a MultiPlayerHUDManager object!");
            }

            return s_Instance;
        }
    }
    #endregion
}
