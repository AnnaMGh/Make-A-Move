using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class SplashHandler : MonoBehaviour
{
    private VideoPlayer videoPlayer;
    private bool changingScene;
    private bool preapearPlaying;
    private bool startPlaying;

    // Start is called before the first frame update
    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        videoPlayer.Prepare();

        //check sound
        if (PlayerPrefs.GetInt(Constants.KEY_SOUND) == 0)
        {
            videoPlayer.EnableAudioTrack(0, false);
        }



    }

    // Update is called once per frame
    void Update()
    {
        /*if (!preapearPlaying
            && videoPlayer.isPrepared && !videoPlayer.isPlaying)
        {
            preapearPlaying = true;
            GlobalSingleton.GetInstance().SetTimeAsync(1000, (obj) =>
            {
                startPlaying = true;
                videoPlayer.Play();
            });
        }
        else if (!changingScene && startPlaying
            && videoPlayer.isPrepared && !videoPlayer.isPlaying)
        {
            changingScene = true;
            SceneManager.LoadScene(1);
        }*/

        if (!changingScene
           && videoPlayer.isPrepared && !videoPlayer.isPlaying)
        {
            changingScene = true;
            SceneManager.LoadScene(1);
        }
    }
}
