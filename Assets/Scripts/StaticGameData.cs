using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticGameData : MonoBehaviour {

	public int playersNeededForMatch;
    public int scoreNeededToWin;
    public int numberOfGemsToSpawn;

    #region instance
    private static StaticGameData s_Instance = null;
    public static StaticGameData instance
    {
        get
        {
            if (s_Instance == null)
            {
                s_Instance = FindObjectOfType(typeof(StaticGameData)) as StaticGameData;
            }

            if (s_Instance == null)
            {
                Debug.LogWarning("Could not locate a StaticGameData object!");
            }

            return s_Instance;
        }
    }
    #endregion
}
