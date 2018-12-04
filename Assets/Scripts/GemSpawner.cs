using UnityEngine.Networking;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GemSpawner : MonoBehaviour {

    public GameObject gem;
    public List<GameObject> gemSpawnLocations;
    //int max = 1;
    
    private void Start()
    {
        gemSpawnLocations = new List<GameObject>();
        foreach (Transform t in transform)
        {
            gemSpawnLocations.Add(t.gameObject);
        }
    }

    public IEnumerator SpawnLoop()
    {
        int count = 0;
        //check for gwms that might be active from previous match
        Gem[] activeGems = FindObjectsOfType<Gem>();
        Debug.LogWarning(activeGems.Length);
        foreach (Gem aGem in activeGems)
        {
            NetworkServer.Destroy(aGem.gameObject);
            //Destroy(aGem);
        }
        yield return new WaitForSeconds(.5f);
        Debug.Log("Starting GEM Spawn Loop");
        while (count<StaticGameData.instance.numberOfGemsToSpawn) {

            yield return new WaitForSeconds(3f);

            int ran = Random.Range(0, gemSpawnLocations.Count);
            GameObject newGem = (GameObject)Instantiate(gem, gemSpawnLocations[ran].transform.position, Quaternion.identity);

            newGem.gameObject.SetActive(true);

            //NetworkServer.SpawnWithClientAuthority(newGem,GetComponent<NetworkIdentity>().connectionToServer);
            NetworkServer.Spawn(newGem);
            count++;
        }
    }

    #region instance
    private static GemSpawner s_Instance = null;
    public static GemSpawner instance
    {
        get
        {
            if (s_Instance == null)
            {
                s_Instance = FindObjectOfType(typeof(GemSpawner)) as GemSpawner;
            }

            if (s_Instance == null)
            {
                Debug.LogWarning("Could not locate a GemSpawner object!");
            }

            return s_Instance;
        }
    }
    #endregion
}
