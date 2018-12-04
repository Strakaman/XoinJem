using UnityEngine;
using UnityEngine.Networking;

public class Gem : NetworkBehaviour
{

    XoinNetworkManager myManager = null; //TODO: gotta be a better way to communicate back to server
    public AudioSource coinSound;
    public SpriteRenderer spr;
    public Collider2D coll;
    bool hit = false; //because of net delays, same object can trigger multiple collisions it seems
    public void SetManager(XoinNetworkManager managerToSet)
    {
        myManager = managerToSet;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hit) { return; }

        PlayerMasterHandler pm = collision.GetComponent<PlayerMasterHandler>();
        if (pm != null)
        {
            hit = true;
            coll.enabled = false;
            spr.enabled = false;
            coinSound.Play();
            if (isServer) //since GemSpawner is server only, host should be the only thing looking to report collisions
            {
                Debug.Log("Player that hit it got a NetID of " + pm.netId);
                pm.IncreaseScore();
                //myManager.CoinHit(pm.netId.Value);
                //playsound
                Invoke("RealDestroy", .5f);
            }
        }

    }

    void RealDestroy()
    {
        Destroy(this.gameObject);
    }
}
