using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiPlayerHUDManager : MonoBehaviour {

    public PlayerHUD[] allPlayerHUDS;

    public PlayerHUD RetrieveHUD(int playerNum)
    {
        return allPlayerHUDS[playerNum];
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
