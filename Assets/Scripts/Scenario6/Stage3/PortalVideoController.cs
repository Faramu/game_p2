using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using System;
using System.Collections;

public class PortalVideoController : MonoBehaviour
{
    [Header("Wajib Diisi")]
    public VideoPlayer videoPlayer;
    public RawImage videoRawImage;
    public RenderTexture renderTexture;

    [Header("Audio")]
    [Tooltip("AudioSource khusus untuk memutar suara video portal. Jika kosong, akan otomatis dibuat.")]
    public AudioSource videoAudioSource;

    [Header("Pengaturan StreamingAssets")]
    [Tooltip("Nama file video di StreamingAssets, contoh: 'STAGE 6 AKHIR .mp4'")]
    public string videoFileName = "STAGE 6 AKHIR .mp4";

    [Header("Root objek video")]
    public GameObject videoRoot;

    [Header("Panel yang dimatikan saat video")]
    public GameObject[] panelsToHide;

    [Header("Timeout")]
    public float prepareTimeoutSeconds = 8f;

    public Action OnVideoSelesai;

    private bool isFinished = false;
    private bool playRequested = false; // cegah Start() mematikan video
    private bool isPlaying = false;
    private bool isPreparing = false;
    private Coroutine monitorCoroutine;

    private void Awake()
    {
        if (videoRoot == null)
            videoRoot = gameObject;

        if (videoPlayer == null)
        {
            videoPlayer = GetComponent<VideoPlayer>();
            if (videoPlayer == null)
            {
                videoPlayer = GetComponentInChildren<VideoPlayer>(true);
            }
        }

        if (videoPlayer != null)
        {
            videoPlayer.playOnAwake = false;
            videoPlayer.playbackSpeed = 1f;
            if (videoPlayer.isPlaying)
            {
                videoPlayer.Stop();
            }
        }

        if (videoRawImage == null)
        {
            videoRawImage = GetComponent<RawImage>();
            if (videoRawImage == null)
            {
                videoRawImage = GetComponentInChildren<RawImage>(true);
            }
        }

        SetupAudioSourceInstance();
    }

    private void SetupAudioSourceInstance()
    {
        if (videoPlayer == null) return;

        if (videoAudioSource == null)
        {
            AudioSource src = videoPlayer.GetComponent<AudioSource>();
            if (src == null) src = GetComponent<AudioSource>();
            if (src == null) src = videoPlayer.gameObject.AddComponent<AudioSource>();
            videoAudioSource = src;
        }

        if (videoAudioSource != null)
        {
            string goName = videoAudioSource.gameObject.name.ToLower();
            if (goName.Contains("bgm") || goName.Contains("backgroundmusic") || BGMController.IsBgmSource(videoAudioSource, null))
            {
                Debug.LogWarning("[VIDEO] AudioSource terdeteksi sama dengan BGM! Membuat AudioSource khusus untuk video portal.");
                videoAudioSource = videoPlayer.gameObject.AddComponent<AudioSource>();
            }

            videoAudioSource.playOnAwake = false;
            videoAudioSource.loop = false;
            videoAudioSource.spatialBlend = 0f;
            videoAudioSource.volume = 1f;
            videoAudioSource.mute = false;
        }
    }

    private void Start()
    {
        // Kalau PlayDariPortal sudah dipanggil di frame yang sama, JANGAN matikan root
        if (playRequested)
        {
            Debug.Log("[PortalVideo] Start di-skip (play sudah diminta)");
            return;
        }

        SetupPlayer();

        // Hanya matikan root jika belum diminta play
        if (videoRoot != null && !playRequested)
            videoRoot.SetActive(false);
    }

    private void SetupPlayer()
    {
        if (videoPlayer == null) return;

        videoPlayer.playOnAwake = false;
        videoPlayer.isLooping = false;
        videoPlayer.skipOnDrop = true;
        videoPlayer.playbackSpeed = 1f;
        videoPlayer.renderMode = VideoRenderMode.RenderTexture;

        if (renderTexture != null)
            videoPlayer.targetTexture = renderTexture;

        if (!string.IsNullOrEmpty(videoFileName))
        {
            videoPlayer.source = VideoSource.Url;
            string url = VideoUrlHelper.GetStreamingAssetsUrl(videoFileName);
            videoPlayer.url = url;
            Debug.Log($"[PortalVideo] Video URL: {url}");
        }
        else if (videoPlayer.clip != null)
        {
            videoPlayer.source = VideoSource.VideoClip;
        }

        SetupAudioSourceInstance();

        videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
        videoPlayer.controlledAudioTrackCount = 1;
        videoPlayer.EnableAudioTrack(0, true);
        if (videoAudioSource != null)
        {
            videoPlayer.SetTargetAudioSource(0, videoAudioSource);
        }

        for (ushort i = 1; i < videoPlayer.audioTrackCount; i++)
        {
            videoPlayer.EnableAudioTrack(i, false);
        }
    }

    public void PlayDariPortal()
    {
        if (isPlaying || isPreparing) return;

        playRequested = true;
        isFinished = false;

        float t0 = Time.realtimeSinceStartup;

        if (videoRoot != null)
            videoRoot.SetActive(true);

        gameObject.SetActive(true);
        HideAllPanels();

        if (videoPlayer == null || videoRawImage == null || renderTexture == null)
        {
            Debug.LogError("[PortalVideo] Assign belum lengkap → skip");
            SelesaiDanCallback();
            return;
        }

        SetupPlayer();

        if (string.IsNullOrEmpty(videoPlayer.url) && videoPlayer.clip == null)
        {
            Debug.LogError("[PortalVideo] Video Clip & URL KOSONG → skip");
            SelesaiDanCallback();
            return;
        }

        videoPlayer.targetTexture = renderTexture;
        videoRawImage.texture = renderTexture;
        videoRawImage.color = Color.white;
        videoRawImage.gameObject.SetActive(true);

        videoPlayer.loopPointReached -= OnVideoSelesaiInternal;
        videoPlayer.loopPointReached += OnVideoSelesaiInternal;

        videoPlayer.errorReceived -= OnVideoErrorReceived;
        videoPlayer.errorReceived += OnVideoErrorReceived;

        if (videoPlayer.isPlaying)
            videoPlayer.Stop();

        // Hentikan BGM di background saat video portal mulai
        BGMController.HentikanBGMUntukVideo(videoPlayer);

        // Play lewat coroutine
        StopAllCoroutines();
        StartCoroutine(PlayWhenReady(t0));
    }

    private IEnumerator PlayWhenReady(float t0)
    {
        isPreparing = true;
        Debug.Log("[VIDEO] Prepare dimulai");

        if (!videoPlayer.isPrepared)
        {
            videoPlayer.Prepare();
            float t = 0f;
            while (!videoPlayer.isPrepared && t < prepareTimeoutSeconds)
            {
                t += Time.unscaledDeltaTime;
                yield return null;
            }
        }

        if (!videoPlayer.isPrepared)
        {
            Debug.LogError($"[VIDEO] Video error = Prepare timeout ({prepareTimeoutSeconds}s)");
            isPreparing = false;
            SelesaiDanCallback();
            yield break;
        }

        Debug.Log("[VIDEO] Prepare selesai");
        isPreparing = false;

        if (isFinished || isPlaying) yield break;

        isPlaying = true;
        videoPlayer.playbackSpeed = 1f;
        videoPlayer.Play();

        Debug.Log("[VIDEO] Play dipanggil");
        Debug.Log($"[VIDEO] Playback Speed = {videoPlayer.playbackSpeed}");
        Debug.Log($"[VIDEO] Audio Output Mode = {videoPlayer.audioOutputMode}");
        Debug.Log($"[VIDEO] Audio Source = {(videoAudioSource != null ? videoAudioSource.gameObject.name : "None")}");

        if (monitorCoroutine != null) StopCoroutine(monitorCoroutine);
        monitorCoroutine = StartCoroutine(MonitorPlaybackProgress());
    }

    private IEnumerator MonitorPlaybackProgress()
    {
        float logInterval = 2f;
        float nextLogTime = Time.unscaledTime + logInterval;
        float maxAllowedDuration = (float)videoPlayer.length + 3.0f;
        float elapsedPlayTime = 0f;

        while (isPlaying && !isFinished)
        {
            elapsedPlayTime += Time.unscaledDeltaTime;

            if (Time.unscaledTime >= nextLogTime)
            {
                nextLogTime = Time.unscaledTime + logInterval;
                Debug.Log($"[VIDEO] Time = {videoPlayer.time:F2} / {videoPlayer.length:F2}");
            }

            if (videoPlayer.length > 0 && elapsedPlayTime > maxAllowedDuration)
            {
                Debug.LogWarning("[VIDEO] Video portal mencapai durasi penuh melalui safety timer.");
                OnVideoSelesaiInternal(videoPlayer);
                yield break;
            }

            yield return null;
        }
    }

    private void OnVideoErrorReceived(VideoPlayer vp, string message)
    {
        Debug.LogError($"[VIDEO] Video error = {message}");
        if (!isFinished)
        {
            OnVideoSelesaiInternal(vp);
        }
    }

    private void OnVideoSelesaiInternal(VideoPlayer vp)
    {
        if (isFinished) return;
        isFinished = true;
        isPlaying = false;
        isPreparing = false;

        Debug.Log("[VIDEO] Video selesai");
        CleanupVideo();
        SelesaiDanCallback();
    }

    private void CleanupVideo()
    {
        StopAllCoroutines();

        if (monitorCoroutine != null)
        {
            StopCoroutine(monitorCoroutine);
            monitorCoroutine = null;
        }

        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoSelesaiInternal;
            videoPlayer.errorReceived -= OnVideoErrorReceived;
            if (videoPlayer.isPlaying)
                videoPlayer.Stop();
            videoPlayer.targetTexture = null;
        }

        if (videoAudioSource != null && videoAudioSource.isPlaying)
        {
            videoAudioSource.Stop();
        }

        if (videoRawImage != null)
        {
            videoRawImage.texture = null;
            videoRawImage.gameObject.SetActive(false);
        }

        if (renderTexture != null)
        {
            renderTexture.Release();
        }

        if (videoRoot != null)
            videoRoot.SetActive(false);

        playRequested = false;

        // Pulihkan BGM kembali sesuai kondisi sebelum video diputar
        BGMController.PulihkanBGMSetelahVideo();
    }

    private void OnDisable()
    {
        BGMController.PulihkanBGMSetelahVideo();
    }

    private void SelesaiDanCallback()
    {
        CleanupVideo();
        OnVideoSelesai?.Invoke();
    }

    public void Skip()
    {
        if (isFinished) return;
        isFinished = true;

        if (videoPlayer != null && videoPlayer.isPlaying)
            videoPlayer.Stop();

        CleanupVideo();
        SelesaiDanCallback();
    }

    private void HideAllPanels()
    {
        if (panelsToHide == null) return;
        foreach (var p in panelsToHide)
        {
            if (p != null) p.SetActive(false);
        }
    }
}