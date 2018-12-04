using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BGMManager : NetworkBehaviour {

    
    public List<AudioClip> BGMsToPlay;
    public AudioSource myMusicPlayer;
    public AudioClip lobbyBGM;
    public AudioClip victoryMusic;

    [SyncVar]
    public int playOnJoinClipIndex = -1; //set as sync var so players that join late get the right music

   /* public override void OnStartClient()
    {
        base.OnStartClient();
        InitialPlay();
    }*/

    private void OnEnable()
    {
        Invoke("InitialPlay", .5f);
    }
    /// <summary>
    /// Check if should play lobby music if hosting/match hasn't started or if should play battle music
    /// </summary>
    private void InitialPlay()
    {
        if (playOnJoinClipIndex == -1)
        {
            myMusicPlayer.clip = lobbyBGM;
        }
        else
        {
            myMusicPlayer.clip = BGMsToPlay[playOnJoinClipIndex];
        }
        myMusicPlayer.loop = true;
        myMusicPlayer.Play();
    }

    [ClientRpc]
    public void RpcPlayAudioClip(int whichOne)
    {
        myMusicPlayer.Stop();
        //playOnJoinClipIndex = whichOne;
        myMusicPlayer.loop = true;
        myMusicPlayer.clip = BGMsToPlay[whichOne];
        myMusicPlayer.Play();
    }

    [ClientRpc]
    public void RpcPlayVictoryMusic()
    {
        myMusicPlayer.loop = false;
        myMusicPlayer.clip = victoryMusic;
        myMusicPlayer.Play();
    }

    //called by server object to initiate RPC from server version of BGM Manager
    public void SendRPCToPlayClip(int whichOne)
    {
        RpcPlayAudioClip(whichOne);
    }

    public void PleaseStopWithTheMusic()
    {
        myMusicPlayer.Stop();
    }

    public int HowManyClipsYouGot()
    {
        return BGMsToPlay.Count;
    }

    #region instance
    private static BGMManager s_Instance = null;
    public static BGMManager instance
    {
        get
        {
            if (s_Instance == null)
            {
                s_Instance = FindObjectOfType(typeof(BGMManager)) as BGMManager;
            }

            if (s_Instance == null)
            {
                Debug.LogWarning("Could not locate a BGMManager object!");
            }

            return s_Instance;
        }
    }
    #endregion

}
