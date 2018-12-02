using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
    public AudioSource source;

    public static SoundManager instance;

    static TagList<AudioClip> ClipList = new TagList<AudioClip>();

    Coroutine delayPlay;

    [RuntimeInitializeOnLoadMethod]
    static void CreateManager()
    {
        if (instance != null) return;
        
        GameObject holder = new GameObject();
        holder.name = "Sound Manager";
        //holder.hideFlags = HideFlags.HideInHierarchy;
        DontDestroyOnLoad(holder);
        holder.AddComponent<AudioSource>();
        instance = holder.AddComponent<SoundManager>();
    }

    void Awake()
    {
        instance = this;
        source = GetComponent<AudioSource>();
    }

    void Start()
    {
        source.mute = DataManager.Get<bool>("isMute");
    }

    public static void Load(string Clip_Name, string[] tags)
    {
        if (instance == null) CreateManager();

        AudioClip clip = ClipList.FindName(Clip_Name);
        if(clip == null)
        {
            clip = Resources.Load("Sounds/" + Clip_Name) as AudioClip;
            if (clip == null)
                Debug.LogError("File \"Sounds/"+Clip_Name+"\" not found or not of type AudioClip");

            else
                ClipList.Add(Clip_Name, clip, tags);
        }
            
        else
            Debug.LogWarning("Clip \"" + Clip_Name + "\" was already loaded");
    }

    public static void Play(string name)
    {
        if (instance == null) return;
        AudioClip clip = ClipList.FindName(name);

        if (clip == null)
        {
            Debug.LogError("AudioClip " + name + " not loaded");
            return;
        }

        instance.source.PlayOneShot(clip);
    }

    IEnumerator BGDelayPlay(AudioClip clip, float delay = 0f)
    {
        // WaitForSeconds is needed to allow AudioSource to process the intro clip
        yield return new WaitForSeconds(1f);
        yield return new WaitWhile(() => source.isPlaying);
        yield return new WaitForSeconds(delay);
        source.clip = clip;
        source.loop = true;
        source.Play();
    }

    void PlayBGAfterIntro(AudioClip clip, float delay = 0f)
    {
        delayPlay = StartCoroutine(BGDelayPlay(clip, delay));
    }

    public static void PlayBG(string name, string introPart = null, float introDelay = 0f)
    {
        if (instance == null) return;
        AudioClip clip = ClipList.FindName(name);

        if (clip == null)
        {
            Debug.LogError("AudioClip " + name + " not loaded");
            return;
        }

        AudioClip clip2 = null;
        if (introPart != null)
            clip2 = ClipList.FindName(introPart);

        if (instance.source.clip == clip || (clip2 != null && instance.source.clip == clip2))
        {
            if (instance.source.isPlaying)
                Debug.LogWarning("Clip \"" + name + "\" is already playing");

            else
            {
                instance.source.Play();

                if (instance.source.clip == clip2)
                {
                    instance.StopCoroutine();
                    instance.PlayBGAfterIntro(clip);
                }
            }
            return;
        }

        if(introPart != null)
        {
            if (clip2 == null)
            {
                Debug.LogError("AudioClip " + name + " not loaded...");
                return;
            }

            instance.source.loop = false;
            instance.source.clip = clip2;
            instance.source.Play();

            instance.PlayBGAfterIntro(clip, introDelay);
        }
        else
        {
            instance.source.clip = clip;
            instance.source.Play();
            instance.source.loop = true;
        }
    }

    public static void SetVolume(float newVol)
    {
        if (instance == null) return;
        instance.source.volume = newVol;
    }

    public static float GetVolume()
    {
        if (instance == null) return 0;
        return instance.source.volume;
    }

    void StopCoroutine()
    {
        if (delayPlay != null)
        {
            StopCoroutine(delayPlay);
            delayPlay = null;
        }
    }

    public static void Stop()
    {
        if (instance == null) return;
        instance.StopCoroutine();
        instance.source.Stop();
    }

    public static void SetMute(bool newState)
    {
        instance.source.mute = newState;
    }

    static void RealUnload(List<Tagged<AudioClip>> list, int index)
    {
        Resources.UnloadAsset(list[index].value);
        list.RemoveAt(index);
    }

    public static void Unload(string tag)
    {
        if (instance == null) return;
        ClipList.Execute(tag, RealUnload);
    }
}
