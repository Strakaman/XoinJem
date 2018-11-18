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
       
        yield return new WaitForSeconds(.5f);

        while (count<StaticGameData.instance.numberOfGemsToSpawn) {

            yield return new WaitForSeconds(3f);

            int ran = Random.Range(0, gemSpawnLocations.Count);
            GameObject newGem = (GameObject)Instantiate(gem, gemSpawnLocations[ran].transform.position, Quaternion.identity);

            newGem.gameObject.SetActive(true);

            NetworkServer.Spawn(newGem);
            count++;
        }
    }

}
