using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalAudioHandler : MonoBehaviour
{
    public enum AudioType { START, GAME, GAME_OVER, FINISH, SILENCE };

    private AudioSource[] audioSource;
    private AudioClip gameOverClip;
    private AudioClip finishClip;
    private AudioClip[] gameClips;
    private AudioType currentType;

    private const float VOLUME_MAX = 0.7f;
    private const float VOLUME_MIN = 0.2f;
    private const float VOLUME_OFF = 0.2f;

    private float lastVolumeBeforGameTypeOff;

    private AudioClip clipBeforeDisabled;
    private AudioType typeBeforeDisabled;
    private float clipTimeBeforeDisabled;
    private float volumeBeforeDisabled;

    private int s1 = 0;
    private int s2 = 50;
    private int s3 = 110;


    // Start is called before the first frame update
    void Awake()
    {
        audioSource = this.GetComponents<AudioSource>();

        //set audio clips
        gameOverClip = Resources.Load<AudioClip>("Sounds/game_over_sound");
        finishClip = Resources.Load<AudioClip>("Sounds/finish_sound");
        gameClips = Resources.LoadAll<AudioClip>("Sounds/Loops");
    }

    // Update is called once per frame
    void Update()
    {
        //continue playing background music in GAME state
        if (audioSource[0].enabled || audioSource[1])
        {
            if (currentType == AudioType.GAME)
            {
                if (audioSource[0].clip.length - audioSource[0].time < 1f)
                {
                    //play new loop
                    audioSource[0].volume -= 0.005f;
                    lastVolumeBeforGameTypeOff = audioSource[0].volume;
                    if (audioSource[0].clip.length - audioSource[0].time < 0.01f)
                    {
                        //play new loop
                        audioSource[0].volume = VOLUME_OFF;
                        PlaySound(currentType);
                    }
                }
                else
                {
                    if (audioSource[0].volume < VOLUME_MAX)
                    {
                        audioSource[0].volume += 0.001f;
                    }
                }


               /* if (audioSource.time > 20f && audioSource.time < 20.5f)
                {
                    audioSource.time = s2;
                    Debug.Log("Change: " + audioSource.time);
                }
                else if (audioSource.time > 60 && audioSource.time < 60 + 0.5f)
                {
                    audioSource.time = s3;
                    Debug.Log("Change: " + audioSource.time);
                }*/

               // Debug.Log("Time: " + audioSource[0].time);
            }
        }
    }

    private void ChooseGameLoop()
    {
        int random = Random.Range(0, gameClips.Length);
        audioSource[0].clip = gameClips[random];
       // audioSource.clip = gameClips[0];
    }

    public void EnableAudioSource(bool enable)
    {
        if (!enable)
        {
            StoreCurrentData(true);

            audioSource[0].Stop();
            audioSource[0].enabled = false;
        }
        if (enable)
        {
            audioSource[0].enabled = true;
            if (currentType == AudioType.GAME && typeBeforeDisabled == AudioType.GAME)
            {
                audioSource[0].clip = clipBeforeDisabled;
                audioSource[0].time = clipTimeBeforeDisabled;
                audioSource[0].Play();
            }
            else
            {
                PlaySound(AudioType.GAME);
            }
        }


    }

    public void PlaySound(AudioType type)
    {
        //check sound
        if (PlayerPrefs.GetInt(Constants.KEY_SOUND) == 0)
        {
            return;
        }

        float volume = VOLUME_MAX;
        bool play = true;

        switch (type)
        {
            case AudioType.START:
                {
                    audioSource[0].clip = null;
                    break;
                }
            case AudioType.GAME:
                {
                    if (currentType != AudioType.GAME)
                    {
                        volume = VOLUME_MIN;
                    }
                    else
                    {
                        if (audioSource[0].volume == VOLUME_OFF)
                        {
                            volume = lastVolumeBeforGameTypeOff;
                        }
                        else
                        {
                            volume = audioSource[0].volume;
                        }
                    }
                    ChooseGameLoop();
                    break;
                }
            case AudioType.GAME_OVER:
                {
                    audioSource[0].clip = gameOverClip;
                    break;
                }
            case AudioType.FINISH:
                {
                    audioSource[0].clip = finishClip;
                    break;
                }
            case AudioType.SILENCE:
                {
                    StoreCurrentData(false);
                    play = false;
                    audioSource[0].clip = null;
                    break;
                }
        }

        if (play && audioSource[0].enabled)
        {
            audioSource[0].Play();
        }
        else
        {
            audioSource[0].Stop();
        }

        audioSource[0].volume = volume;
        audioSource[0].time = 0;
        currentType = type;
    }

    private void StoreCurrentData(bool store)
    {
        if (store)
        {
            typeBeforeDisabled = currentType;
            clipBeforeDisabled = audioSource[0].clip;
            clipTimeBeforeDisabled = audioSource[0].time;
            volumeBeforeDisabled = audioSource[0].volume;
        }
        else
        {
            typeBeforeDisabled = AudioType.SILENCE;
            clipBeforeDisabled = null;
            clipTimeBeforeDisabled = 0;
            volumeBeforeDisabled = 0;
        }
    }

}
