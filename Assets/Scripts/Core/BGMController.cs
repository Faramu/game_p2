using UnityEngine;
using UnityEngine.Video;
using System.Collections.Generic;

/// <summary>
/// Mengontrol pemutaran BGM (Background Music) saat video diputar di seluruh scene.
/// Memastikan BGM dijeda (pause) selama video berlangsung dan dipulihkan kembali saat video selesai,
/// tanpa memengaruhi audio/SFX/suara dialog dari video itu sendiri.
/// </summary>
public static class BGMController
{
    private static readonly List<AudioSource> pausedBgmSources = new List<AudioSource>();
    private static readonly List<AudioSource> scheduledPlaySources = new List<AudioSource>();
    private static bool isBgmPausedForVideo = false;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void ResetStaticState()
    {
        pausedBgmSources.Clear();
        scheduledPlaySources.Clear();
        isBgmPausedForVideo = false;
    }

    /// <summary>
    /// Menghentikan/Pause semua BGM di scene yang sedang aktif saat video mulai dimainkan.
    /// Tidak menghentikan audio video, dialog video, ataupun SFX.
    /// </summary>
    public static void HentikanBGMUntukVideo(VideoPlayer videoPlayer = null)
    {
        if (isBgmPausedForVideo)
        {
            Debug.Log("[BGM] BGM sudah dalam status pause untuk video.");
            return;
        }

        pausedBgmSources.Clear();
        scheduledPlaySources.Clear();

        // Cari semua AudioSource di scene
        AudioSource[] allAudioSources = Object.FindObjectsByType<AudioSource>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (AudioSource audio in allAudioSources)
        {
            if (audio == null) continue;

            if (IsBgmSource(audio, videoPlayer))
            {
                if (audio.isPlaying)
                {
                    audio.Pause();
                    pausedBgmSources.Add(audio);
                    Debug.Log($"<color=orange>[BGM] BGM di-PAUSE untuk video: '{audio.gameObject.name}'</color>");
                }
                else if (audio.playOnAwake && audio.isActiveAndEnabled)
                {
                    // Dijadwalkan oleh Unity playOnAwake tapi belum sempat bersuara
                    audio.playOnAwake = false;
                    audio.Stop();
                    scheduledPlaySources.Add(audio);
                    Debug.Log($"<color=orange>[BGM] BGM PlayOnAwake di-STOP untuk video: '{audio.gameObject.name}'</color>");
                }
            }
        }

        isBgmPausedForVideo = true;
    }

    /// <summary>
    /// Memulihkan kembali BGM yang sebelumnya sedang dimainkan sebelum video diputar.
    /// Jika sebelumnya BGM memang tidak sedang dimainkan, tidak dipaksa dimainkan.
    /// </summary>
    public static void PulihkanBGMSetelahVideo()
    {
        if (!isBgmPausedForVideo)
        {
            return;
        }

        foreach (AudioSource audio in pausedBgmSources)
        {
            if (audio != null)
            {
                audio.UnPause();
                Debug.Log($"<color=green>[BGM] BGM dipulihkan (UnPause): '{audio.gameObject.name}'</color>");
            }
        }

        foreach (AudioSource audio in scheduledPlaySources)
        {
            if (audio != null)
            {
                audio.playOnAwake = true;
                audio.Play();
                Debug.Log($"<color=green>[BGM] BGM dipulihkan (Play): '{audio.gameObject.name}'</color>");
            }
        }

        pausedBgmSources.Clear();
        scheduledPlaySources.Clear();
        isBgmPausedForVideo = false;
    }

    /// <summary>
    /// Mengecek apakah suatu AudioSource adalah Background Music (BGM).
    /// Mengecualikan audio dari VideoPlayer dan SFX.
    /// </summary>
    public static bool IsBgmSource(AudioSource src, VideoPlayer vp)
    {
        if (src == null) return false;

        // JANGAN PERNAH menghentikan audio video!
        if (vp != null)
        {
            if (src.gameObject == vp.gameObject || src.transform.IsChildOf(vp.transform))
                return false;

            if (vp.audioOutputMode == VideoAudioOutputMode.AudioSource)
            {
                for (ushort i = 0; i < vp.controlledAudioTrackCount; i++)
                {
                    if (vp.GetTargetAudioSource(i) == src)
                        return false;
                }
            }
        }

        string goName = src.gameObject.name.ToLower();

        // 1. Identifikasi nama GameObject umum untuk BGM
        if (goName.Contains("backgroundmusic") || goName.Contains("bgm") || goName.Contains("music"))
        {
            return true;
        }

        // 2. Audio looping dengan clip yang bukan SFX
        if (src.loop && src.clip != null)
        {
            if (!goName.Contains("sfx") && !goName.Contains("sound") && !goName.Contains("effect"))
            {
                return true;
            }
        }

        return false;
    }
}
