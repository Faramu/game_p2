using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// Mengelola pemutaran Video Intro menggunakan StreamingAssets, RenderTexture, AudioSource khusus video, dan VideoPanel/RawImage.
/// Kompatibel dengan Unity Editor, PC, Android, dan WebGL hosting.
/// </summary>
public class IntroVideoManager : MonoBehaviour
{
    [Header("Wajib Diisi")]
    public VideoPlayer videoPlayer;
    public RawImage videoRawImage;
    public RenderTexture renderTexture;

    [Header("Audio")]
    [Tooltip("AudioSource khusus untuk memutar suara video. Jika kosong, akan otomatis dibuat pada GameObject VideoPlayer.")]
    public AudioSource videoAudioSource;

    [Header("Pengaturan Video StreamingAssets")]
    [Tooltip("Nama file video di folder StreamingAssets, contoh: 'STAGE 1 - AWAL.mp4'")]
    public string videoFileName = "STAGE 1 - AWAL.mp4";

    [Header("Panel yang dimatikan saat video diputar")]
    public GameObject[] panelsToHide;

    [Header("Pengaturan")]
    public bool forcePlayForTesting = true; // set false setelah testing selesai
    [Tooltip("Key PlayerPrefs untuk menandai video sudah ditonton. Jika kosong, otomatis berdasarkan nama scene.")]
    public string playerPrefsKeyCustom = "";
    [Tooltip("Batas waktu tunggu Prepare sebelum fallback melanjutkan game (detik)")]
    public float prepareTimeoutSeconds = 8f;

    [Header("Event")]
    public UnityEvent onVideoFinished;

    private bool isPreparing = false;
    private bool isPlaying = false;
    private bool isFinished = false;
    private Coroutine prepareCoroutine;
    private Coroutine monitorCoroutine;

    private string EffectiveKey
    {
        get
        {
            if (!string.IsNullOrEmpty(playerPrefsKeyCustom))
            {
                return playerPrefsKeyCustom;
            }
            return "IntroVideoWatched_" + SceneManager.GetActiveScene().name;
        }
    }

    private void Awake()
    {
        // 1. Auto-resolve VideoPlayer
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

        // 2. Auto-resolve RawImage
        if (videoRawImage == null)
        {
            videoRawImage = GetComponent<RawImage>();
            if (videoRawImage == null)
            {
                videoRawImage = GetComponentInChildren<RawImage>(true);
            }
        }

        // 3. Auto-resolve atau buat AudioSource khusus video
        SetupAudioSourceInstance();
    }

    private void SetupAudioSourceInstance()
    {
        if (videoPlayer == null) return;

        if (videoAudioSource == null)
        {
            // Cari AudioSource di VideoPlayer GameObject
            AudioSource src = videoPlayer.GetComponent<AudioSource>();
            if (src == null)
            {
                src = GetComponent<AudioSource>();
            }

            // Jika masih null, tambahkan AudioSource baru khusus pada VideoPlayer
            if (src == null)
            {
                src = videoPlayer.gameObject.AddComponent<AudioSource>();
            }

            videoAudioSource = src;
        }

        // Pastikan bukan AudioSource BGM
        if (videoAudioSource != null)
        {
            string goName = videoAudioSource.gameObject.name.ToLower();
            if (goName.Contains("bgm") || goName.Contains("backgroundmusic") || BGMController.IsBgmSource(videoAudioSource, null))
            {
                Debug.LogWarning("[VIDEO] AudioSource terdeteksi sama dengan BGM! Membuat AudioSource terpisah khusus untuk video.");
                videoAudioSource = videoPlayer.gameObject.AddComponent<AudioSource>();
            }

            videoAudioSource.playOnAwake = false;
            videoAudioSource.loop = false;
            videoAudioSource.spatialBlend = 0f; // 2D Sound
            videoAudioSource.volume = 1f;
            videoAudioSource.mute = false;
        }
    }

    void Start()
    {
        if (isPreparing || isPlaying || isFinished) return;

        if (forcePlayForTesting)
        {
            PlayerPrefs.DeleteKey(EffectiveKey);
            PlayerPrefs.DeleteKey("IntroVideoWatched_Skenario1");
            PlayerPrefs.Save();
        }

        bool sudahNonton = PlayerPrefs.GetInt(EffectiveKey, 0) == 1 || PlayerPrefs.GetInt("IntroVideoWatched_Skenario1", 0) == 1;

        if (sudahNonton && !forcePlayForTesting)
        {
            Debug.Log("[VIDEO] Video di-skip karena sudah pernah ditonton.");
            SkipTanpaPlay();
            return;
        }

        // Matikan semua panel UI sementara
        HideAllPanels();

        if (videoPlayer == null || videoRawImage == null || renderTexture == null)
        {
            Debug.LogError("[VIDEO] Referensi VideoPlayer / RawImage / RenderTexture belum lengkap di Inspector! Melewati video...");
            SkipTanpaPlay();
            return;
        }

        // Setup RenderTexture & RawImage
        videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        videoPlayer.targetTexture = renderTexture;
        videoRawImage.texture = renderTexture;
        videoRawImage.gameObject.SetActive(true);

        // Setup Source URL dari StreamingAssets
        string targetUrl = "";
        if (!string.IsNullOrEmpty(videoFileName))
        {
            targetUrl = VideoUrlHelper.GetStreamingAssetsUrl(videoFileName);
            videoPlayer.source = VideoSource.Url;
            videoPlayer.url = targetUrl;
            Debug.Log($"[VIDEO] StreamingAssets Video: {videoFileName}");
            Debug.Log($"[VIDEO] URL: {targetUrl}");
        }
        else if (videoPlayer.clip != null)
        {
            Debug.LogWarning("[VIDEO] videoFileName kosong, menggunakan fallback videoPlayer.clip.");
            videoPlayer.source = VideoSource.VideoClip;
        }
        else
        {
            Debug.LogError("[VIDEO] Tidak ada videoFileName ataupun videoClip! Melewati video...");
            SkipTanpaPlay();
            return;
        }

        // Setup Audio Output Mode ke AudioSource untuk mencegah AudioSampleProvider buffer overflow
        SetupAudioSourceInstance();

        videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
        videoPlayer.controlledAudioTrackCount = 1;
        videoPlayer.EnableAudioTrack(0, true);
        if (videoAudioSource != null)
        {
            videoPlayer.SetTargetAudioSource(0, videoAudioSource);
        }

        // Nonaktifkan audio track selain track 0 jika ada
        for (ushort i = 1; i < videoPlayer.audioTrackCount; i++)
        {
            videoPlayer.EnableAudioTrack(i, false);
        }

        videoPlayer.playOnAwake = false;
        videoPlayer.isLooping = false;
        videoPlayer.skipOnDrop = true;
        videoPlayer.playbackSpeed = 1f;

        // Pasang callback loopPointReached dan errorReceived
        videoPlayer.loopPointReached -= OnLoopPointReached;
        videoPlayer.loopPointReached += OnLoopPointReached;

        videoPlayer.errorReceived -= OnVideoErrorReceived;
        videoPlayer.errorReceived += OnVideoErrorReceived;

        // Hentikan/Pause BGM sebelum video dimainkan
        BGMController.HentikanBGMUntukVideo(videoPlayer);

        // Mulai flow Prepare
        if (prepareCoroutine != null)
        {
            StopCoroutine(prepareCoroutine);
        }
        prepareCoroutine = StartCoroutine(PrepareDanPutarVideo());
    }

    private IEnumerator PrepareDanPutarVideo()
    {
        isPreparing = true;
        Debug.Log("[VIDEO] Prepare dimulai");
        videoPlayer.Prepare();

        float timer = 0f;
        while (!videoPlayer.isPrepared && timer < prepareTimeoutSeconds)
        {
            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        if (!videoPlayer.isPrepared)
        {
            Debug.LogError($"[VIDEO] Video error = Prepare timeout ({prepareTimeoutSeconds}s) atau gagal memuat video.");
            isPreparing = false;
            OnVideoSelesai(videoPlayer);
            yield break;
        }

        Debug.Log("[VIDEO] Prepare selesai");
        isPreparing = false;

        if (isFinished || isPlaying)
        {
            yield break;
        }

        isPlaying = true;
        videoPlayer.playbackSpeed = 1f;

        // Play dipanggil tepat satu kali
        videoPlayer.Play();
        Debug.Log("[VIDEO] Play dipanggil");
        Debug.Log($"[VIDEO] Playback Speed = {videoPlayer.playbackSpeed}");
        Debug.Log($"[VIDEO] Audio Output Mode = {videoPlayer.audioOutputMode}");
        Debug.Log($"[VIDEO] Audio Source = {(videoAudioSource != null ? videoAudioSource.gameObject.name : "None")}");

        // Monitor playback time dan fallback jika loopPointReached macet di ujung
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

            // Fallback safety timeout jika video sudah mencapai batas akhir durasi
            if (videoPlayer.length > 0 && elapsedPlayTime > maxAllowedDuration)
            {
                Debug.LogWarning("[VIDEO] Video mencapai durasi penuh melalui safety timer. Menyelesaikan video...");
                OnVideoSelesai(videoPlayer);
                yield break;
            }

            yield return null;
        }
    }

    private void OnLoopPointReached(VideoPlayer vp)
    {
        OnVideoSelesai(vp);
    }

    private void OnVideoErrorReceived(VideoPlayer vp, string message)
    {
        Debug.LogError($"[VIDEO] Video error = {message}");
        OnVideoSelesai(vp);
    }

    void HideAllPanels()
    {
        if (panelsToHide == null) return;

        foreach (GameObject panel in panelsToHide)
        {
            if (panel != null)
                panel.SetActive(false);
        }
    }

    void ShowAllPanels()
    {
        if (panelsToHide == null) return;

        foreach (GameObject panel in panelsToHide)
        {
            if (panel != null)
                panel.SetActive(true);
        }
    }

    public void OnVideoSelesai(VideoPlayer vp)
    {
        if (isFinished) return;
        isFinished = true;
        isPlaying = false;
        isPreparing = false;

        Debug.Log("[VIDEO] Video selesai");

        if (prepareCoroutine != null)
        {
            StopCoroutine(prepareCoroutine);
            prepareCoroutine = null;
        }

        if (monitorCoroutine != null)
        {
            StopCoroutine(monitorCoroutine);
            monitorCoroutine = null;
        }

        // Tandai sudah ditonton
        PlayerPrefs.SetInt(EffectiveKey, 1);
        PlayerPrefs.Save();

        // Pulihkan BGM sesuai kondisi sebelumnya
        BGMController.PulihkanBGMSetelahVideo();

        // Bersihkan VideoPlayer
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnLoopPointReached;
            videoPlayer.errorReceived -= OnVideoErrorReceived;
            if (videoPlayer.isPlaying)
            {
                videoPlayer.Stop();
            }
            videoPlayer.targetTexture = null;
        }

        // Hentikan AudioSource video jika masih bersuara
        if (videoAudioSource != null && videoAudioSource.isPlaying)
        {
            videoAudioSource.Stop();
        }

        // Bersihkan VideoPanel / RawImage
        if (videoRawImage != null)
        {
            videoRawImage.texture = null;
            videoRawImage.gameObject.SetActive(false);
        }

        // Bersihkan RenderTexture jika tersedia
        if (renderTexture != null)
        {
            renderTexture.Release();
        }

        // Matikan objek video jika bukan root scene
        if (gameObject != null && gameObject != this.gameObject.scene.GetRootGameObjects()[0])
        {
            gameObject.SetActive(false);
        }

        // Nyalakan kembali semua panel yang sebelumnya disembunyikan
        ShowAllPanels();

        // Lanjutkan ke alur dialog existing secara universal
        MonoBehaviour[] allScripts = FindObjectsOfType<MonoBehaviour>();
        foreach (MonoBehaviour script in allScripts)
        {
            if (script.GetType().GetMethod("LanjutkanSetelahVideo") != null)
            {
                script.Invoke("LanjutkanSetelahVideo", 0f);
            }
        }

        // Invoke UnityEvent jika ada listener
        onVideoFinished?.Invoke();
    }

    private void SkipTanpaPlay()
    {
        if (isFinished) return;
        isFinished = true;
        isPlaying = false;
        isPreparing = false;

        if (videoRawImage != null)
        {
            videoRawImage.gameObject.SetActive(false);
        }

        ShowAllPanels();
        gameObject.SetActive(false);

        MonoBehaviour[] allScripts = FindObjectsOfType<MonoBehaviour>();
        foreach (MonoBehaviour script in allScripts)
        {
            if (script.GetType().GetMethod("LanjutkanSetelahVideo") != null)
            {
                script.Invoke("LanjutkanSetelahVideo", 0f);
            }
        }

        onVideoFinished?.Invoke();
    }

    private void OnDisable()
    {
        // Pastikan BGM selalu dipulihkan jika objek video dinonaktifkan
        BGMController.PulihkanBGMSetelahVideo();
    }

    public void Skip()
    {
        if (videoPlayer != null && videoPlayer.isPlaying)
        {
            videoPlayer.Stop();
        }
        OnVideoSelesai(videoPlayer);
    }
}