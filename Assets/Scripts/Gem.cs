using UnityEngine;
using UnityEngine.Networking;

public class Gem : NetworkBehaviour {

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
        if (localPlayerAuthority) //since GemSpawner is server only, host should be the only thing looking to detect collisions
        {
            PlayerMovement pm = collision.GetComponent<PlayerMovement>();
            if (pm != null)
            {
                hit = true;
                coll.enabled = false;
                spr.enabled = false;
                Debug.LogWarning("Player that hit it got a NetID of " + pm.netId);
                coinSound.Play();
                pm.CmdIncreaseScore();
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
