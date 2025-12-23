using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource bgSource;
    public AudioSource sfxSource;

    [Header("Background Music Clips")]
    public AudioClip mainMenuBG;
    public AudioClip gameBG;

    [Header("Sound Effects Clips")]
    public AudioClip correctSFX;
    public AudioClip wrongSFX;
    public AudioClip buttonClickSFX;

    public AudioClip tilePickSFX;
    public AudioClip winSFX;
    public AudioClip loseSFX;


    private Dictionary<string, AudioClip> sfxDict = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioClip> bgDict = new Dictionary<string, AudioClip>();

    private void Awake()
    {
        // SINGLETON: only 1 AudioManager allowed
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadDictionaries();

    }

    private void LoadDictionaries()
    {
        // Add Background Music
        bgDict["mainmenu"] = mainMenuBG;
        bgDict["game"] = gameBG;

        // Add SFX
        sfxDict["correct"] = correctSFX;
        sfxDict["wrong"] = wrongSFX;
        sfxDict["button"] = buttonClickSFX;


        //  MISSING ONES (ADD THESE)
        sfxDict["tile"] = tilePickSFX;
        sfxDict["win"] = winSFX;
        sfxDict["lose"] = loseSFX;
    }


    // ---------------------
    //  PLAY BACKGROUND
    public void PlayBG(string bgName)
    {
        if (bgDict.ContainsKey(bgName))
        {
            bgSource.clip = bgDict[bgName];
            bgSource.loop = true;
            bgSource.Play();
        }
        else
        {
            Debug.LogWarning("BG Music not found: " + bgName);
        }
    }

    public void StopBG()
    {
        bgSource.Stop();
    }


    //  PLAY SOUND EFFECTS
    public void PlaySFX(string sfxName)
    {
        if (sfxDict.ContainsKey(sfxName))
        {
            sfxSource.PlayOneShot(sfxDict[sfxName]);
        }
        else
        {
            Debug.LogWarning("SFX not found: " + sfxName);
        }
    }
}
