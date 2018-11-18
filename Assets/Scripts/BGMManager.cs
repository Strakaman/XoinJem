using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BGMManager : NetworkBehaviour {

    
    public List<AudioClip> BGMsToPlay;
    public AudioSource myMusicPlayer;
    public AudioClip lobbyBGM;
    public void OnEnable()
    {
        myMusicPlayer.clip = lobbyBGM;
        myMusicPlayer.Play();
    }

    [ClientRpc]
    public void RpcPlayAudioClip(int whichOne)
    {
        myMusicPlayer.Stop();
        myMusicPlayer.clip = BGMsToPlay[whichOne];
        myMusicPlayer.Play();
    }

    public void SendRPCToPlayClip(int whichone)
    {
        RpcPlayAudioClip(whichone);
    }
    public void PleaseStopWithTheMusic()
    {
        myMusicPlayer.Stop();
    }

    public int HowManyClipsYouGot()
    {
        return BGMsToPlay.Count;
    }
}
