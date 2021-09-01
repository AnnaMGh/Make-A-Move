using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalAudioHandler : MonoBehaviour
{
    public enum AudioType { START, GAME, GAME_OVER, FINISH, SILENCE };

    private AudioSource audioSource;
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
   

    // Start is called before the first frame update
    void Awake()
    {
        audioSource = this.GetComponent<AudioSource>();

        //set audio clips
        gameOverClip = Resources.Load<AudioClip>("Sounds/game_over_sound");
        finishClip = Resources.Load<AudioClip>("Sounds/finish_sound");
        gameClips = Resources.LoadAll<AudioClip>("Sounds/Loops");
    }

    // Update is called once per frame
    void Update()
    {
        //continue playing background music in GAME state
        if (audioSource.enabled)
        {
            if (currentType == AudioType.GAME)
            {
                if (audioSource.clip.length - audioSource.time < 1f)
                {
                    //play new loop
                    audioSource.volume -= 0.005f;
                    lastVolumeBeforGameTypeOff = audioSource.volume;
                    if (audioSource.clip.length - audioSource.time < 0.01f)
                    {
                        //play new loop
                        audioSource.volume = VOLUME_OFF;
                        PlaySound(currentType);
                    }
                }
                else
                {
                    if (audioSource.volume < VOLUME_MAX)
                    {
                        audioSource.volume += 0.001f;
                    }
                }
            }
        }
    }

    private void ChooseGameLoop()
    {
        int random = Random.Range(0, gameClips.Length);
        audioSource.clip = gameClips[random];
    }

    public void EnableAudioSource(bool enable)
    {
        if(!enable)
        {
            StoreCurrentData(true);  
            
            audioSource.Stop();
            audioSource.enabled = false;
        }
        if (enable)
        {
            audioSource.enabled = true;
            if (currentType == AudioType.GAME && typeBeforeDisabled == AudioType.GAME)
            {
                audioSource.clip = clipBeforeDisabled;
                audioSource.time = clipTimeBeforeDisabled;
                audioSource.Play();
            }
            else {
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
                    audioSource.clip = null;
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
                        if (audioSource.volume == VOLUME_OFF)
                        {
                            volume = lastVolumeBeforGameTypeOff;
                        }
                        else {
                            volume = audioSource.volume;
                        }
                    }
                    ChooseGameLoop();
                    break;
                }
            case AudioType.GAME_OVER:
                {
                    audioSource.clip = gameOverClip;
                    break;
                }
            case AudioType.FINISH:
                {
                    audioSource.clip = finishClip;
                    break;
                }
            case AudioType.SILENCE:
                {
                    StoreCurrentData(false);
                    play = false;
                    audioSource.clip = null;
                    break;
                }
        }

        if (play && audioSource.enabled)
        {
            audioSource.Play();
        }
        else {
            audioSource.Stop();
        }

        audioSource.volume = volume;
        audioSource.time = 0;
        currentType = type;
    }

    private void StoreCurrentData(bool store) {
        if (store)
        {
            typeBeforeDisabled = currentType;
            clipBeforeDisabled = audioSource.clip;
            clipTimeBeforeDisabled = audioSource.time;
            volumeBeforeDisabled = audioSource.volume;
        }
        else {
            typeBeforeDisabled = AudioType.SILENCE;
            clipBeforeDisabled = null;
            clipTimeBeforeDisabled = 0;
            volumeBeforeDisabled = 0;
        }
    }

}
