using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.EventSystems;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEditor.Events;

public class GameSetupManagerWindow : EditorWindow
{
    private int currentTab = 0;
    private readonly string[] tabTitles = new string[] { "Mobile Controls", "Video Hosting", "Validation" };

    // ========================================================
    // TAB 1: MOBILE CONTROLS
    // ========================================================
    [System.Serializable]
    public class SceneItem
    {
        public string scenePath;
        public string sceneName;
        public bool isGameScene;
        public bool isSelected;
        public bool hasVideo;
    }

    private float majuPosX = -318f;
    private float majuPosY = 87f;
    private float majuWidth = 150f;
    private float majuHeight = 150f;

    private float mundurPosX = -580f;
    private float mundurPosY = 87f;
    private float mundurWidth = 150f;
    private float mundurHeight = 150f;

    private float lompatPosX = -2094f;
    private float lompatPosY = 215f;
    private float lompatWidth = 180f;
    private float lompatHeight = 180f;

    private bool createMissing = false;
    private bool validateEventTrigger = true;
    private bool syncMobileControlsReferences = true;

    private List<SceneItem> sceneList = new List<SceneItem>();
    private Vector2 sceneScrollPos;

    // ========================================================
    // TAB 2: VIDEO HOSTING (VIDEO MANAGER)
    // ========================================================
    private string[] availableVideoFiles = new string[0];
    private int selectedIntroVideoIndex = 0;
    private int selectedEndingVideoIndex = 0;
    private RenderTexture targetRenderTexture;

    // References for active scene inspection
    private VideoPlayer activeVideoPlayer;
    private GameObject activeVideoPanel;
    private RawImage activeRawImage;
    private IntroVideoManager activeIntroManager;
    private PortalVideoController activePortalController;

    // ========================================================
    // TAB 3: VALIDATION
    // ========================================================
    public class ValidationEntry
    {
        public string sceneName;
        public List<string> items = new List<string>();
        public int successCount = 0;
        public int warningCount = 0;
        public int errorCount = 0;
    }

    private List<ValidationEntry> validationResults = new List<ValidationEntry>();
    private int totalSuccess = 0;
    private int totalWarnings = 0;
    private int totalErrors = 0;
    private Vector2 validationScrollPos;
    private bool hasRunValidation = false;

    // Common
    private Vector2 mainScrollPos;
    private List<string> processLogs = new List<string>();
    private Vector2 logScrollPos;

    // ========================================================
    // MENU ITEMS
    // ========================================================
    [MenuItem("Tools/Game Setup Manager", false, 0)]
    public static void OpenGameSetupManager()
    {
        GameSetupManagerWindow window = GetWindow<GameSetupManagerWindow>("Game Setup Manager");
        window.minSize = new Vector2(520, 720);
        window.Show();
    }

    [MenuItem("Tools/Mobile Controls Manager", false, 1)]
    public static void OpenMobileControlsTab()
    {
        GameSetupManagerWindow window = GetWindow<GameSetupManagerWindow>("Game Setup Manager");
        window.currentTab = 0;
        window.minSize = new Vector2(520, 720);
        window.Show();
    }

    [MenuItem("Tools/Video Manager", false, 2)]
    public static void OpenVideoManagerTab()
    {
        GameSetupManagerWindow window = GetWindow<GameSetupManagerWindow>("Game Setup Manager");
        window.currentTab = 1;
        window.minSize = new Vector2(520, 720);
        window.Show();
    }

    private void OnEnable()
    {
        LoadPreferences();
        RefreshStreamingAssetsVideos();
        FindDefaultRenderTexture();
        RefreshSceneList();
        RefreshActiveSceneVideoReferences();
    }

    private void OnDisable()
    {
        SavePreferences();
    }

    private void LoadPreferences()
    {
        majuPosX = EditorPrefs.GetFloat("GSM_MajuPosX", -318f);
        majuPosY = EditorPrefs.GetFloat("GSM_MajuPosY", 87f);
        majuWidth = EditorPrefs.GetFloat("GSM_MajuWidth", 150f);
        majuHeight = EditorPrefs.GetFloat("GSM_MajuHeight", 150f);

        mundurPosX = EditorPrefs.GetFloat("GSM_MundurPosX", -580f);
        mundurPosY = EditorPrefs.GetFloat("GSM_MundurPosY", 87f);
        mundurWidth = EditorPrefs.GetFloat("GSM_MundurWidth", 150f);
        mundurHeight = EditorPrefs.GetFloat("GSM_MundurHeight", 150f);

        lompatPosX = EditorPrefs.GetFloat("GSM_LompatPosX", -2094f);
        lompatPosY = EditorPrefs.GetFloat("GSM_LompatPosY", 215f);
        lompatWidth = EditorPrefs.GetFloat("GSM_LompatWidth", 180f);
        lompatHeight = EditorPrefs.GetFloat("GSM_LompatHeight", 180f);

        createMissing = EditorPrefs.GetBool("GSM_CreateMissing", false);
        validateEventTrigger = EditorPrefs.GetBool("GSM_ValidateEventTrigger", true);
        syncMobileControlsReferences = EditorPrefs.GetBool("GSM_SyncReferences", true);

        selectedIntroVideoIndex = EditorPrefs.GetInt("GSM_IntroVideoIndex", 0);
        selectedEndingVideoIndex = EditorPrefs.GetInt("GSM_EndingVideoIndex", 0);
    }

    private void SavePreferences()
    {
        EditorPrefs.SetFloat("GSM_MajuPosX", majuPosX);
        EditorPrefs.SetFloat("GSM_MajuPosY", majuPosY);
        EditorPrefs.SetFloat("GSM_MajuWidth", majuWidth);
        EditorPrefs.SetFloat("GSM_MajuHeight", majuHeight);

        EditorPrefs.SetFloat("GSM_MundurPosX", mundurPosX);
        EditorPrefs.SetFloat("GSM_MundurPosY", mundurPosY);
        EditorPrefs.SetFloat("GSM_MundurWidth", mundurWidth);
        EditorPrefs.SetFloat("GSM_MundurHeight", mundurHeight);

        EditorPrefs.SetFloat("GSM_LompatPosX", lompatPosX);
        EditorPrefs.SetFloat("GSM_LompatPosY", lompatPosY);
        EditorPrefs.SetFloat("GSM_LompatWidth", lompatWidth);
        EditorPrefs.SetFloat("GSM_LompatHeight", lompatHeight);

        EditorPrefs.SetBool("GSM_CreateMissing", createMissing);
        EditorPrefs.SetBool("GSM_ValidateEventTrigger", validateEventTrigger);
        EditorPrefs.SetBool("GSM_SyncReferences", syncMobileControlsReferences);

        EditorPrefs.SetInt("GSM_IntroVideoIndex", selectedIntroVideoIndex);
        EditorPrefs.SetInt("GSM_EndingVideoIndex", selectedEndingVideoIndex);
    }

    private void FindDefaultRenderTexture()
    {
        if (targetRenderTexture != null) return;

        RenderTexture dedicated = AssetDatabase.LoadAssetAtPath<RenderTexture>("Assets/RenderTexture/VideoRenderTexture.renderTexture");
        if (dedicated != null)
        {
            targetRenderTexture = dedicated;
            return;
        }

        // Cek apakah ada di Assets/RenderTexture atau Assets/
        string[] guids = AssetDatabase.FindAssets("t:RenderTexture");
        RenderTexture fallback1080 = null;
        RenderTexture anyRt = null;

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            RenderTexture rt = AssetDatabase.LoadAssetAtPath<RenderTexture>(path);
            if (rt != null)
            {
                if (anyRt == null) anyRt = rt;
                if (rt.width >= 1280 && rt.height >= 720)
                {
                    fallback1080 = rt;
                    if (path.Contains("Video") || path.Contains("Platformer"))
                    {
                        targetRenderTexture = rt;
                        return;
                    }
                }
            }
        }

        targetRenderTexture = fallback1080 != null ? fallback1080 : anyRt;
    }

    private void RefreshStreamingAssetsVideos()
    {
        string streamingPath = Path.Combine(Application.dataPath, "StreamingAssets");
        if (!Directory.Exists(streamingPath))
        {
            Directory.CreateDirectory(streamingPath);
            AssetDatabase.Refresh();
        }

        string[] extensions = new string[] { "*.mp4", "*.webm", "*.mov", "*.m4v" };
        List<string> files = new List<string>();

        foreach (string ext in extensions)
        {
            string[] found = Directory.GetFiles(streamingPath, ext, SearchOption.TopDirectoryOnly);
            foreach (string f in found)
            {
                files.Add(Path.GetFileName(f));
            }
        }

        files.Sort();
        availableVideoFiles = files.ToArray();

        // Default indices if available
        for (int i = 0; i < availableVideoFiles.Length; i++)
        {
            if (availableVideoFiles[i].IndexOf("AWAL", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                selectedIntroVideoIndex = i;
                break;
            }
        }

        for (int i = 0; i < availableVideoFiles.Length; i++)
        {
            if (availableVideoFiles[i].IndexOf("AKHIR", StringComparison.OrdinalIgnoreCase) >= 0 &&
                availableVideoFiles[i].IndexOf("STAGE 6", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                selectedEndingVideoIndex = i;
                break;
            }
        }
    }

    public void RefreshSceneList()
    {
        sceneList.Clear();
        HashSet<string> addedPaths = new HashSet<string>();

        foreach (EditorBuildSettingsScene bScene in EditorBuildSettings.scenes)
        {
            if (string.IsNullOrEmpty(bScene.path)) continue;
            if (!File.Exists(bScene.path)) continue;

            string sName = Path.GetFileNameWithoutExtension(bScene.path);
            bool isGame = IsGameScene(bScene.path, sName);

            sceneList.Add(new SceneItem
            {
                scenePath = bScene.path,
                sceneName = sName,
                isGameScene = isGame,
                isSelected = isGame,
                hasVideo = IsVideoScene(bScene.path, sName)
            });
            addedPaths.Add(bScene.path);
        }

        string[] allGuids = AssetDatabase.FindAssets("t:Scene", new[] { "Assets/Scenes" });
        foreach (string guid in allGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (addedPaths.Contains(path)) continue;

            string sName = Path.GetFileNameWithoutExtension(path);
            bool isGame = IsGameScene(path, sName);

            sceneList.Add(new SceneItem
            {
                scenePath = path,
                sceneName = sName,
                isGameScene = isGame,
                isSelected = isGame,
                hasVideo = IsVideoScene(path, sName)
            });
            addedPaths.Add(path);
        }
    }

    private void RefreshActiveSceneVideoReferences()
    {
        activeIntroManager = UnityEngine.Object.FindFirstObjectByType<IntroVideoManager>(FindObjectsInactive.Include);
        activePortalController = UnityEngine.Object.FindFirstObjectByType<PortalVideoController>(FindObjectsInactive.Include);

        // Cari VideoPlayer
        activeVideoPlayer = null;
        if (activeIntroManager != null && activeIntroManager.videoPlayer != null)
        {
            activeVideoPlayer = activeIntroManager.videoPlayer;
        }
        else if (activePortalController != null && activePortalController.videoPlayer != null)
        {
            activeVideoPlayer = activePortalController.videoPlayer;
        }
        else
        {
            activeVideoPlayer = UnityEngine.Object.FindFirstObjectByType<VideoPlayer>(FindObjectsInactive.Include);
        }

        // Cari VideoPanel / RawImage
        activeVideoPanel = null;
        activeRawImage = null;

        if (activeIntroManager != null && activeIntroManager.videoRawImage != null)
        {
            activeRawImage = activeIntroManager.videoRawImage;
            activeVideoPanel = activeRawImage.gameObject;
        }
        else if (activePortalController != null && activePortalController.videoRawImage != null)
        {
            activeRawImage = activePortalController.videoRawImage;
            activeVideoPanel = activeRawImage.gameObject;
        }
        else
        {
            GameObject[] allGos = UnityEngine.Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var go in allGos)
            {
                if (go.name.Equals("VideoPanel", StringComparison.OrdinalIgnoreCase))
                {
                    activeVideoPanel = go;
                    activeRawImage = go.GetComponentInChildren<RawImage>(true);
                    break;
                }
            }

            if (activeRawImage == null)
            {
                activeRawImage = UnityEngine.Object.FindFirstObjectByType<RawImage>(FindObjectsInactive.Include);
                if (activeRawImage != null) activeVideoPanel = activeRawImage.gameObject;
            }
        }
    }

    private bool IsGameScene(string path, string name)
    {
        string lower = name.ToLower();
        if (lower.Contains("menu") || lower.Contains("checkpoint") || lower.Contains("pencapaian") || lower.Contains("petunjuk"))
            return false;
        if (lower.Contains("samplescene"))
            return false;
        return lower.Contains("stage") || lower.Contains("level");
    }

    private bool IsVideoScene(string path, string name)
    {
        string pLower = path.ToLower().Replace('\\', '/');
        // Scene yang memiliki intro / ending video: Level 1 di tiap Stage, dan Stage 6 Level 3 (Final Boss)
        if (pLower.Contains("level1") || pLower.Contains("finalboss") || pLower.Contains("stage6_finalboss"))
            return true;
        return false;
    }

    // ========================================================
    // GUI RENDERING
    // ========================================================
    private void OnGUI()
    {
        EditorGUILayout.Space(6);
        currentTab = GUILayout.Toolbar(currentTab, tabTitles, GUILayout.Height(30));
        EditorGUILayout.Space(6);

        mainScrollPos = EditorGUILayout.BeginScrollView(mainScrollPos);

        switch (currentTab)
        {
            case 0:
                DrawTabMobileControls();
                break;
            case 1:
                DrawTabVideoHosting();
                break;
            case 2:
                DrawTabValidation();
                break;
        }

        EditorGUILayout.Space(12);
        DrawLogsSection();

        EditorGUILayout.EndScrollView();
    }

    // ========================================================
    // TAB 1: MOBILE CONTROLS GUI
    // ========================================================
    private void DrawTabMobileControls()
    {
        EditorGUILayout.LabelField("MOBILE CONTROLS", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("Atur posisi dan ukuran tombol Mobile Controls. Klik [PREVIEW SCENE AKTIF] untuk melihat hasil langsung di scene yang terbuka, atau [UPDATE SEMUA SCENE] untuk menerapkan ke seluruh Game Scene terpilih.", MessageType.Info);
        EditorGUILayout.Space(6);

        DrawButtonConfig("BUTTON MAJU", ref majuPosX, ref majuPosY, ref majuWidth, ref majuHeight);
        EditorGUILayout.Space(4);
        DrawButtonConfig("BUTTON MUNDUR", ref mundurPosX, ref mundurPosY, ref mundurWidth, ref mundurHeight);
        EditorGUILayout.Space(4);
        DrawButtonConfig("BUTTON LOMPAT", ref lompatPosX, ref lompatPosY, ref lompatWidth, ref lompatHeight);

        EditorGUILayout.Space(8);
        EditorGUILayout.LabelField("Pengaturan Lanjutan", EditorStyles.boldLabel);
        validateEventTrigger = EditorGUILayout.ToggleLeft("Validasi & Pasang Ulang Event Trigger (PointerDown/Up)", validateEventTrigger);
        syncMobileControlsReferences = EditorGUILayout.ToggleLeft("Sinkronisasi field MobileControls.cs", syncMobileControlsReferences);
        createMissing = EditorGUILayout.ToggleLeft("Buat Button jika belum ada di scene", createMissing);

        EditorGUILayout.Space(8);
        DrawSceneSelectionList();

        EditorGUILayout.Space(10);
        EditorGUILayout.BeginHorizontal();

        GUI.backgroundColor = new Color(0.3f, 0.7f, 1f);
        if (GUILayout.Button("PREVIEW SCENE AKTIF", GUILayout.Height(36)))
        {
            ApplyToActiveScene();
        }

        GUI.backgroundColor = new Color(0.2f, 0.85f, 0.3f);
        if (GUILayout.Button("UPDATE SEMUA SCENE", GUILayout.Height(36)))
        {
            ApplyToAllScenes();
        }
        GUI.backgroundColor = Color.white;

        EditorGUILayout.EndHorizontal();
    }

    private void DrawButtonConfig(string label, ref float posX, ref float posY, ref float width, ref float height)
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField(label, EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Position X:", GUILayout.Width(70));
        posX = EditorGUILayout.FloatField(posX);
        EditorGUILayout.LabelField("Position Y:", GUILayout.Width(70));
        posY = EditorGUILayout.FloatField(posY);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Width:", GUILayout.Width(70));
        width = EditorGUILayout.FloatField(width);
        EditorGUILayout.LabelField("Height:", GUILayout.Width(70));
        height = EditorGUILayout.FloatField(height);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();
    }

    private void DrawSceneSelectionList()
    {
        EditorGUILayout.LabelField("Daftar Scene Target:", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Pilih Semua Game Scene", EditorStyles.miniButton))
        {
            foreach (var sc in sceneList) if (sc.isGameScene) sc.isSelected = true;
        }
        if (GUILayout.Button("Pilih Semua", EditorStyles.miniButton))
        {
            foreach (var sc in sceneList) sc.isSelected = true;
        }
        if (GUILayout.Button("Hapus Pilihan", EditorStyles.miniButton))
        {
            foreach (var sc in sceneList) sc.isSelected = false;
        }
        if (GUILayout.Button("Refresh", EditorStyles.miniButton))
        {
            RefreshSceneList();
        }
        EditorGUILayout.EndHorizontal();

        sceneScrollPos = EditorGUILayout.BeginScrollView(sceneScrollPos, GUILayout.Height(130));
        for (int i = 0; i < sceneList.Count; i++)
        {
            var item = sceneList[i];
            EditorGUILayout.BeginHorizontal();
            item.isSelected = EditorGUILayout.Toggle(item.isSelected, GUILayout.Width(20));

            GUIStyle labelStyle = item.isGameScene ? EditorStyles.boldLabel : EditorStyles.label;
            string prefix = item.isGameScene ? "[GAME] " : "[MENU] ";
            if (item.hasVideo) prefix = "[VIDEO] " + prefix;

            EditorGUILayout.LabelField(prefix + item.sceneName, labelStyle);
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();
    }

    // ========================================================
    // TAB 2: VIDEO HOSTING GUI
    // ========================================================
    private void DrawTabVideoHosting()
    {
        EditorGUILayout.LabelField("VIDEO HOSTING / VIDEO MANAGER", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("Sistem video terstandarisasi menggunakan StreamingAssets. URL video dibentuk otomatis saat runtime sehingga kompatibel dengan Editor, Standalone PC, Android, dan WebGL Hosting tanpa path hardcode.", MessageType.Info);
        EditorGUILayout.Space(6);

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("PILIHAN VIDEO STREAMINGASSETS", EditorStyles.boldLabel);

        if (availableVideoFiles.Length == 0)
        {
            EditorGUILayout.HelpBox("Tidak ada file video di folder Assets/StreamingAssets! Letakkan file .mp4 di folder tersebut.", MessageType.Warning);
        }
        else
        {
            selectedIntroVideoIndex = EditorGUILayout.Popup("Video Intro:", selectedIntroVideoIndex, availableVideoFiles);
            selectedEndingVideoIndex = EditorGUILayout.Popup("Video Ending:", selectedEndingVideoIndex, availableVideoFiles);
        }

        EditorGUILayout.Space(4);
        string currentIntroName = availableVideoFiles.Length > selectedIntroVideoIndex ? availableVideoFiles[selectedIntroVideoIndex] : "";
        string runtimeUrlSample = VideoUrlHelper.GetStreamingAssetsUrl(currentIntroName);

        EditorGUILayout.LabelField("StreamingAssets Path (Otomatis):", EditorStyles.miniBoldLabel);
        EditorGUILayout.SelectableLabel(Application.streamingAssetsPath, EditorStyles.textField, GUILayout.Height(20));

        EditorGUILayout.LabelField("Runtime URL Preview (Local/PC):", EditorStyles.miniBoldLabel);
        EditorGUILayout.SelectableLabel(runtimeUrlSample, EditorStyles.textField, GUILayout.Height(20));

        EditorGUILayout.LabelField("Runtime URL Preview (WebGL Hosting):", EditorStyles.miniBoldLabel);
        EditorGUILayout.SelectableLabel("StreamingAssets/" + (string.IsNullOrEmpty(currentIntroName) ? "video.mp4" : Uri.EscapeDataString(currentIntroName)), EditorStyles.textField, GUILayout.Height(20));

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Buka Folder StreamingAssets", EditorStyles.miniButton))
        {
            string p = Path.Combine(Application.dataPath, "StreamingAssets");
            if (!Directory.Exists(p)) Directory.CreateDirectory(p);
            EditorUtility.RevealInFinder(p);
        }
        if (GUILayout.Button("Refresh Daftar Video", EditorStyles.miniButton))
        {
            RefreshStreamingAssetsVideos();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();

        EditorGUILayout.Space(6);
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("RENDER TEXTURE & COMPONENT ASSIGNMENTS", EditorStyles.boldLabel);

        targetRenderTexture = (RenderTexture)EditorGUILayout.ObjectField("Render Texture:", targetRenderTexture, typeof(RenderTexture), false);
        if (targetRenderTexture == null)
        {
            EditorGUILayout.HelpBox("Render Texture belum diisi! Klik tombol di bawah untuk membuat atau mendeteksi RenderTexture video 1920x1080.", MessageType.Warning);
            if (GUILayout.Button("Buat / Reset Video RenderTexture (1920x1080)"))
            {
                CreateOrAssignVideoRenderTexture();
            }
        }
        else
        {
            EditorGUILayout.LabelField($"Resolusi: {targetRenderTexture.width} x {targetRenderTexture.height} | Format: {targetRenderTexture.format}", EditorStyles.miniLabel);
        }

        EditorGUILayout.Space(4);
        EditorGUILayout.LabelField("Komponen Scene Aktif (" + SceneManager.GetActiveScene().name + "):", EditorStyles.boldLabel);

        activeVideoPlayer = (VideoPlayer)EditorGUILayout.ObjectField("Video Player:", activeVideoPlayer, typeof(VideoPlayer), true);
        activeVideoPanel = (GameObject)EditorGUILayout.ObjectField("Video Panel:", activeVideoPanel, typeof(GameObject), true);
        activeRawImage = (RawImage)EditorGUILayout.ObjectField("Raw Image:", activeRawImage, typeof(RawImage), true);

        if (GUILayout.Button("Cari Ulang Komponen di Scene Aktif", EditorStyles.miniButton))
        {
            RefreshActiveSceneVideoReferences();
        }

        EditorGUILayout.EndVertical();

        EditorGUILayout.Space(10);
        EditorGUILayout.BeginHorizontal();

        GUI.backgroundColor = new Color(0.3f, 0.7f, 1f);
        if (GUILayout.Button("VALIDATE VIDEO SETUP", GUILayout.Height(36)))
        {
            ValidateActiveSceneVideo();
        }

        GUI.backgroundColor = new Color(0.9f, 0.6f, 0.2f);
        if (GUILayout.Button("UPDATE VIDEO SCENE", GUILayout.Height(36)))
        {
            UpdateActiveSceneVideo();
        }

        GUI.backgroundColor = new Color(0.2f, 0.85f, 0.3f);
        if (GUILayout.Button("UPDATE SEMUA VIDEO SCENE", GUILayout.Height(36)))
        {
            UpdateAllVideoScenes();
        }
        GUI.backgroundColor = Color.white;

        EditorGUILayout.EndHorizontal();
    }

    private void CreateOrAssignVideoRenderTexture()
    {
        string folder = "Assets/RenderTexture";
        if (!AssetDatabase.IsValidFolder(folder))
        {
            AssetDatabase.CreateFolder("Assets", "RenderTexture");
        }

        string assetPath = folder + "/VideoRenderTexture.renderTexture";
        RenderTexture rt = AssetDatabase.LoadAssetAtPath<RenderTexture>(assetPath);

        if (rt == null)
        {
            rt = new RenderTexture(1920, 1080, 24, RenderTextureFormat.ARGB32);
            rt.name = "VideoRenderTexture";
            AssetDatabase.CreateAsset(rt, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Log("[VIDEO] Berhasil membuat RenderTexture baru di: " + assetPath);
        }

        targetRenderTexture = rt;
    }

    // ========================================================
    // TAB 3: VALIDATION GUI
    // ========================================================
    private void DrawTabValidation()
    {
        EditorGUILayout.LabelField("VALIDATION TOOL", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("Pemeriksaan menyeluruh (non-destructive) pada semua Game Scene. Memvalidasi Mobile Controls dan Video Setup tanpa mengubah isi scene.", MessageType.Info);
        EditorGUILayout.Space(6);

        GUI.backgroundColor = new Color(0.2f, 0.75f, 0.95f);
        if (GUILayout.Button("CHECK ALL GAME SCENES", GUILayout.Height(40)))
        {
            RunComprehensiveValidation();
        }
        GUI.backgroundColor = Color.white;

        if (hasRunValidation)
        {
            EditorGUILayout.Space(8);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("=== VALIDATION SUMMARY ===", EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"Total Success : {totalSuccess}", EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"Total Warnings: {totalWarnings}", EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"Total Errors  : {totalErrors}", EditorStyles.boldLabel);
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(6);
            EditorGUILayout.LabelField("Detail per Scene:", EditorStyles.boldLabel);

            validationScrollPos = EditorGUILayout.BeginScrollView(validationScrollPos, GUILayout.Height(280));
            foreach (var entry in validationResults)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField("• " + entry.sceneName, EditorStyles.boldLabel);
                foreach (string item in entry.items)
                {
                    EditorGUILayout.LabelField("   " + item);
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space(2);
            }
            EditorGUILayout.EndScrollView();
        }
    }

    // ========================================================
    // LOGS GUI
    // ========================================================
    private void DrawLogsSection()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Process Log", EditorStyles.boldLabel);
        if (GUILayout.Button("Clear Log", EditorStyles.miniButton, GUILayout.Width(70)))
        {
            processLogs.Clear();
        }
        EditorGUILayout.EndHorizontal();

        logScrollPos = EditorGUILayout.BeginScrollView(logScrollPos, GUILayout.Height(100));
        for (int i = processLogs.Count - 1; i >= 0; i--)
        {
            EditorGUILayout.LabelField(processLogs[i], EditorStyles.miniLabel);
        }
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

    private void Log(string message)
    {
        string timestamped = $"[{DateTime.Now:HH:mm:ss}] {message}";
        processLogs.Add(timestamped);
        Debug.Log(message);
    }

    private void LogWarning(string message)
    {
        string timestamped = $"[{DateTime.Now:HH:mm:ss}] [WARN] {message}";
        processLogs.Add(timestamped);
        Debug.LogWarning(message);
    }

    private void LogError(string message)
    {
        string timestamped = $"[{DateTime.Now:HH:mm:ss}] [ERROR] {message}";
        processLogs.Add(timestamped);
        Debug.LogError(message);
    }

    // ========================================================
    // ACTIONS: MOBILE CONTROLS
    // ========================================================
    private void ApplyToActiveScene()
    {
        Scene activeScene = SceneManager.GetActiveScene();
        Log($"[GAME SETUP] Mulai konfigurasi Mobile Controls di scene aktif: {activeScene.name}");

        int updated = ProcessMobileControlsInCurrentScene();
        if (updated > 0)
        {
            EditorSceneManager.MarkSceneDirty(activeScene);
            Log($"[MOBILE] Berhasil memperbarui {updated} tombol di '{activeScene.name}'.");
            EditorUtility.DisplayDialog("Sukses", $"Mobile Controls berhasil diperbarui di '{activeScene.name}'!", "OK");
        }
        else
        {
            LogWarning($"[MOBILE] MobileControls tidak ditemukan atau tidak ada perubahan di '{activeScene.name}'.");
            EditorUtility.DisplayDialog("Perhatian", $"MobileControls tidak ditemukan di '{activeScene.name}'. Pastikan objek Canvas/MobileControls ada di scene.", "OK");
        }
    }

    private void ApplyToAllScenes()
    {
        SavePreferences();

        List<SceneItem> targets = sceneList.FindAll(s => s.isSelected);
        if (targets.Count == 0)
        {
            EditorUtility.DisplayDialog("Perhatian", "Tidak ada scene yang dipilih untuk di-update.", "OK");
            return;
        }

        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            return;
        }

        string originalScenePath = SceneManager.GetActiveScene().path;
        int totalUpdatedScenes = 0;
        int totalFailedScenes = 0;

        Log($"[GAME SETUP] === MEMULAI UPDATE SEMUA SCENE ({targets.Count} scene) ===");

        try
        {
            for (int i = 0; i < targets.Count; i++)
            {
                var target = targets[i];
                float progress = (float)i / targets.Count;
                EditorUtility.DisplayProgressBar("Updating Mobile Controls", $"Memproses ({i + 1}/{targets.Count}): {target.sceneName}", progress);

                if (!File.Exists(target.scenePath))
                {
                    LogError($"[MOBILE] File scene tidak ditemukan: {target.scenePath}");
                    totalFailedScenes++;
                    continue;
                }

                Scene openedScene = EditorSceneManager.OpenScene(target.scenePath, OpenSceneMode.Single);
                int count = ProcessMobileControlsInCurrentScene();

                if (count > 0)
                {
                    EditorSceneManager.SaveScene(openedScene);
                    totalUpdatedScenes++;
                    Log($"[MOBILE] ✓ {target.sceneName}: {count} tombol berhasil diperbarui dan disimpan.");
                }
                else
                {
                    LogWarning($"[MOBILE] ⚠ {target.sceneName}: MobileControls tidak ditemukan.");
                }
            }
        }
        finally
        {
            EditorUtility.ClearProgressBar();

            if (!string.IsNullOrEmpty(originalScenePath) && File.Exists(originalScenePath))
            {
                EditorSceneManager.OpenScene(originalScenePath, OpenSceneMode.Single);
                Log($"[GAME SETUP] Kembali ke scene awal: {Path.GetFileNameWithoutExtension(originalScenePath)}");
            }
        }

        Log($"[GAME SETUP] Selesai! Berhasil: {totalUpdatedScenes} scene. Gagal/Lewat: {totalFailedScenes} scene.");
        EditorUtility.DisplayDialog("Selesai", $"Update selesai!\nBerhasil: {totalUpdatedScenes} scene\nPeriksa Process Log untuk detail.", "OK");
    }

    private int ProcessMobileControlsInCurrentScene()
    {
        GameObject mobileControlsGo = FindMobileControlsRoot();
        if (mobileControlsGo == null)
        {
            return 0;
        }

        int updatedCount = 0;

        RectTransform maju = FindButtonRect(mobileControlsGo, "ButtonMaju", "Maju", "Button_Maju");
        RectTransform mundur = FindButtonRect(mobileControlsGo, "ButtonMundur", "Mundur", "Button_Mundur");
        RectTransform lompat = FindButtonRect(mobileControlsGo, "ButtonLompat", "Lompat", "Button_Lompat");

        if (maju != null)
        {
            ApplyRectTransform(maju, majuPosX, majuPosY, majuWidth, majuHeight);
            if (validateEventTrigger) ValidateButtonEventTrigger(maju.gameObject, "Maju");
            updatedCount++;
            Log($"[MOBILE] {SceneManager.GetActiveScene().name}: ButtonMaju ditemukan & diatur ({majuWidth}x{majuHeight}).");
        }

        if (mundur != null)
        {
            ApplyRectTransform(mundur, mundurPosX, mundurPosY, mundurWidth, mundurHeight);
            if (validateEventTrigger) ValidateButtonEventTrigger(mundur.gameObject, "Mundur");
            updatedCount++;
            Log($"[MOBILE] {SceneManager.GetActiveScene().name}: ButtonMundur ditemukan & diatur ({mundurWidth}x{mundurHeight}).");
        }

        if (lompat != null)
        {
            ApplyRectTransform(lompat, lompatPosX, lompatPosY, lompatWidth, lompatHeight);
            if (validateEventTrigger) ValidateButtonEventTrigger(lompat.gameObject, "Lompat");
            updatedCount++;
            Log($"[MOBILE] {SceneManager.GetActiveScene().name}: ButtonLompat ditemukan & diatur ({lompatWidth}x{lompatHeight}).");
        }

        if (syncMobileControlsReferences)
        {
            SyncMobileControlsScript(mobileControlsGo, maju, mundur, lompat);
        }

        return updatedCount;
    }

    private GameObject FindMobileControlsRoot()
    {
        GameObject[] roots = SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (var r in roots)
        {
            if (r.name.Equals("MobileControls", StringComparison.OrdinalIgnoreCase))
                return r;

            Transform found = FindDeepChild(r.transform, "MobileControls");
            if (found != null) return found.gameObject;
        }
        return null;
    }

    private RectTransform FindButtonRect(GameObject parent, params string[] possibleNames)
    {
        foreach (string n in possibleNames)
        {
            Transform t = FindDeepChild(parent.transform, n);
            if (t != null && t is RectTransform rt) return rt;
        }
        return null;
    }

    private Transform FindDeepChild(Transform parent, string childName)
    {
        if (parent.name.Equals(childName, StringComparison.OrdinalIgnoreCase))
            return parent;

        for (int i = 0; i < parent.childCount; i++)
        {
            Transform found = FindDeepChild(parent.GetChild(i), childName);
            if (found != null) return found;
        }
        return null;
    }

    private void ApplyRectTransform(RectTransform rt, float posX, float posY, float width, float height)
    {
        Undo.RecordObject(rt, "Modify Mobile Button Layout");
        rt.anchoredPosition = new Vector2(posX, posY);
        rt.sizeDelta = new Vector2(width, height);
        EditorUtility.SetDirty(rt);
    }

    private void ValidateButtonEventTrigger(GameObject btnGo, string actionType)
    {
        EventTrigger trigger = btnGo.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = Undo.AddComponent<EventTrigger>(btnGo);
        }

        // Pastikan GraphicRaycaster dan Image raycastTarget aktif
        Image img = btnGo.GetComponent<Image>();
        if (img != null && !img.raycastTarget)
        {
            Undo.RecordObject(img, "Enable Raycast Target");
            img.raycastTarget = true;
            EditorUtility.SetDirty(img);
        }

        Canvas canvas = btnGo.GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            GraphicRaycaster gr = canvas.GetComponent<GraphicRaycaster>();
            if (gr == null)
            {
                gr = Undo.AddComponent<GraphicRaycaster>(canvas.gameObject);
            }
            if (!gr.enabled) gr.enabled = true;
        }
    }

    private void SyncMobileControlsScript(GameObject root, RectTransform maju, RectTransform mundur, RectTransform lompat)
    {
        MonoBehaviour mb = root.GetComponent("MobileControls") as MonoBehaviour;
        if (mb == null) mb = root.GetComponentInChildren(Type.GetType("MobileControls, Assembly-CSharp")) as MonoBehaviour;
        if (mb == null) return;

        SerializedObject so = new SerializedObject(mb);
        SerializedProperty pMaju = so.FindProperty("buttonMaju");
        SerializedProperty pMundur = so.FindProperty("buttonMundur");
        SerializedProperty pLompat = so.FindProperty("buttonLompat");

        if (pMaju != null && maju != null) pMaju.objectReferenceValue = maju.GetComponent<Button>();
        if (pMundur != null && mundur != null) pMundur.objectReferenceValue = mundur.GetComponent<Button>();
        if (pLompat != null && lompat != null) pLompat.objectReferenceValue = lompat.GetComponent<Button>();

        so.ApplyModifiedProperties();
        EditorUtility.SetDirty(mb);
    }

    // ========================================================
    // ACTIONS: VIDEO HOSTING / VIDEO MANAGER
    // ========================================================
    private void ValidateActiveSceneVideo()
    {
        RefreshActiveSceneVideoReferences();
        FindDefaultRenderTexture();

        string sceneName = SceneManager.GetActiveScene().name;
        Log($"[VIDEO] === VALIDASI VIDEO SETUP DI SCENE: {sceneName} ===");

        // 1. StreamingAssets
        string streamingPath = Path.Combine(Application.dataPath, "StreamingAssets");
        if (Directory.Exists(streamingPath))
        {
            Log("[VIDEO] StreamingAssets ditemukan: " + streamingPath);
        }
        else
        {
            LogError("[VIDEO] Folder StreamingAssets TIDAK DITEMUKAN di Assets/StreamingAssets!");
        }

        // 2. Video File
        string chosenVideo = availableVideoFiles.Length > selectedIntroVideoIndex ? availableVideoFiles[selectedIntroVideoIndex] : "";
        if (!string.IsNullOrEmpty(chosenVideo))
        {
            Log("[VIDEO] Video Terpilih: " + chosenVideo);
            string url = VideoUrlHelper.GetStreamingAssetsUrl(chosenVideo);
            Log("[VIDEO] URL: " + url);
        }
        else
        {
            LogWarning("[VIDEO] Belum ada file video yang dipilih dari dropdown.");
        }

        // 3. RenderTexture
        if (targetRenderTexture != null)
        {
            Log($"[VIDEO] RenderTexture berhasil ditemukan: '{targetRenderTexture.name}' ({targetRenderTexture.width}x{targetRenderTexture.height})");
        }
        else
        {
            LogError("[VIDEO] RenderTexture belum dipilih atau tidak ada di project!");
        }

        // 4. VideoPlayer
        if (activeVideoPlayer != null)
        {
            Log($"[VIDEO] VideoPlayer ditemukan pada GameObject: '{activeVideoPlayer.gameObject.name}'");
            if (activeVideoPlayer.source == VideoSource.Url)
            {
                Log("[VIDEO] VideoPlayer source: URL ✓");
            }
            else
            {
                LogWarning("[VIDEO] VideoPlayer source masih berupa VideoClip! Direkomendasikan menggunakan URL untuk WebGL hosting.");
            }

            if (activeVideoPlayer.targetTexture == targetRenderTexture && targetRenderTexture != null)
            {
                Log("[VIDEO] VideoPlayer.targetTexture terhubung dengan RenderTexture ✓");
            }
            else
            {
                LogWarning("[VIDEO] VideoPlayer.targetTexture belum terhubung dengan RenderTexture yang sesuai.");
            }
        }
        else
        {
            LogWarning($"[VIDEO] VideoPlayer tidak ditemukan di scene '{sceneName}'.");
        }

        // 5. VideoPanel / RawImage
        if (activeRawImage != null)
        {
            Log($"[VIDEO] VideoPanel / RawImage ditemukan pada GameObject: '{activeRawImage.gameObject.name}'");
            if (activeRawImage.texture == targetRenderTexture && targetRenderTexture != null)
            {
                Log("[VIDEO] RawImage.texture terhubung dengan RenderTexture ✓");
            }
            else
            {
                LogWarning("[VIDEO] RawImage.texture belum terhubung dengan RenderTexture yang sesuai.");
            }
        }
        else
        {
            LogWarning($"[VIDEO] VideoPanel / RawImage tidak ditemukan di scene '{sceneName}'.");
        }

        Log("[VIDEO] === VALIDASI SELESAI ===");
        EditorUtility.DisplayDialog("Validasi Video", $"Validasi video selesai untuk scene '{sceneName}'. Lihat Process Log untuk detail.", "OK");
    }

    private void UpdateActiveSceneVideo()
    {
        RefreshActiveSceneVideoReferences();
        FindDefaultRenderTexture();

        if (targetRenderTexture == null)
        {
            EditorUtility.DisplayDialog("Error", "Render Texture belum dipilih! Silakan pilih atau buat RenderTexture terlebih dahulu.", "OK");
            return;
        }

        string videoFile = availableVideoFiles.Length > selectedIntroVideoIndex ? availableVideoFiles[selectedIntroVideoIndex] : "";
        string sceneName = SceneManager.GetActiveScene().name;

        // Auto-match video jika nama file sesuai nomor stage
        videoFile = ResolveVideoFileForScene(sceneName, videoFile);

        bool success = ConfigureVideoInCurrentScene(videoFile, targetRenderTexture);
        if (success)
        {
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            Log($"[VIDEO] Video setup di scene '{sceneName}' berhasil diperbarui!");
            EditorUtility.DisplayDialog("Sukses", $"Video setup di scene '{sceneName}' berhasil diperbarui menggunakan video '{videoFile}'!", "OK");
        }
        else
        {
            LogWarning($"[VIDEO] Tidak dapat memperbarui video di scene '{sceneName}' (VideoPlayer atau VideoPanel tidak ditemukan).");
            EditorUtility.DisplayDialog("Perhatian", $"VideoPlayer atau VideoPanel tidak ditemukan di scene '{sceneName}'.", "OK");
        }
    }

    private void UpdateAllVideoScenes()
    {
        FindDefaultRenderTexture();
        if (targetRenderTexture == null)
        {
            EditorUtility.DisplayDialog("Error", "Render Texture belum dipilih! Silakan pilih atau buat RenderTexture terlebih dahulu.", "OK");
            return;
        }

        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;

        string originalScenePath = SceneManager.GetActiveScene().path;
        List<SceneItem> videoScenes = sceneList.FindAll(s => s.hasVideo || s.sceneName.ToLower().Contains("level1") || s.sceneName.ToLower().Contains("boss"));

        Log($"[VIDEO] === MEMULAI UPDATE SEMUA VIDEO SCENE ({videoScenes.Count} scene) ===");

        int successCount = 0;

        try
        {
            for (int i = 0; i < videoScenes.Count; i++)
            {
                var sc = videoScenes[i];
                EditorUtility.DisplayProgressBar("Updating Video Scenes", $"Memproses ({i + 1}/{videoScenes.Count}): {sc.sceneName}", (float)i / videoScenes.Count);

                if (!File.Exists(sc.scenePath)) continue;

                Scene openedScene = EditorSceneManager.OpenScene(sc.scenePath, OpenSceneMode.Single);
                string videoFile = ResolveVideoFileForScene(sc.sceneName, "");

                bool ok = ConfigureVideoInCurrentScene(videoFile, targetRenderTexture);
                if (ok)
                {
                    EditorSceneManager.SaveScene(openedScene);
                    successCount++;
                    Log($"[VIDEO] ✓ {sc.sceneName}: Konfigurasi video berhasil diperbarui ({videoFile}).");
                }
                else
                {
                    LogWarning($"[VIDEO] ⚠ {sc.sceneName}: Tidak ditemukan komponen video.");
                }
            }
        }
        finally
        {
            EditorUtility.ClearProgressBar();
            if (!string.IsNullOrEmpty(originalScenePath) && File.Exists(originalScenePath))
            {
                EditorSceneManager.OpenScene(originalScenePath, OpenSceneMode.Single);
                Log($"[VIDEO] Kembali ke scene awal: {Path.GetFileNameWithoutExtension(originalScenePath)}");
            }
        }

        Log($"[VIDEO] Selesai! Berhasil memperbarui {successCount} video scene.");
        EditorUtility.DisplayDialog("Selesai", $"Update semua video scene selesai!\nBerhasil: {successCount} scene.", "OK");
    }

    private string ResolveVideoFileForScene(string sceneName, string fallback)
    {
        string s = sceneName.ToLower();

        if (s.Contains("finalboss") || s.Contains("level3"))
        {
            foreach (var f in availableVideoFiles)
            {
                if (f.IndexOf("STAGE 6 AKHIR", StringComparison.OrdinalIgnoreCase) >= 0) return f;
            }
        }
        else if (s.Contains("stage1"))
        {
            foreach (var f in availableVideoFiles)
            {
                if (f.IndexOf("STAGE 1 - AWAL", StringComparison.OrdinalIgnoreCase) >= 0) return f;
            }
        }
        else if (s.Contains("stage2"))
        {
            foreach (var f in availableVideoFiles)
            {
                if (f.IndexOf("STAGE 1 - AKHIR", StringComparison.OrdinalIgnoreCase) >= 0) return f;
            }
        }
        else if (s.Contains("stage3"))
        {
            foreach (var f in availableVideoFiles)
            {
                if (f.IndexOf("STAGE 2 AKHIR", StringComparison.OrdinalIgnoreCase) >= 0) return f;
            }
        }
        else if (s.Contains("stage4"))
        {
            foreach (var f in availableVideoFiles)
            {
                if (f.IndexOf("STAGE 3 AKHIR", StringComparison.OrdinalIgnoreCase) >= 0) return f;
            }
        }
        else if (s.Contains("stage5"))
        {
            foreach (var f in availableVideoFiles)
            {
                if (f.IndexOf("STAGE 4 AKHIR", StringComparison.OrdinalIgnoreCase) >= 0) return f;
            }
        }
        else if (s.Contains("stage6"))
        {
            foreach (var f in availableVideoFiles)
            {
                if (f.IndexOf("STAGE 5 AKHIR", StringComparison.OrdinalIgnoreCase) >= 0) return f;
            }
        }

        if (!string.IsNullOrEmpty(fallback)) return fallback;
        return availableVideoFiles.Length > 0 ? availableVideoFiles[0] : "";
    }

    private bool ConfigureVideoInCurrentScene(string videoFileName, RenderTexture rt)
    {
        RefreshActiveSceneVideoReferences();

        if (activeVideoPlayer == null && activeIntroManager == null && activePortalController == null)
        {
            return false;
        }

        // 1. Konfigurasi AudioSource Khusus Video
        AudioSource videoAudioSource = null;
        if (activeVideoPlayer != null)
        {
            videoAudioSource = activeVideoPlayer.GetComponent<AudioSource>();
            if (videoAudioSource == null)
            {
                videoAudioSource = Undo.AddComponent<AudioSource>(activeVideoPlayer.gameObject);
            }

            // Pastikan bukan AudioSource BGM
            string goName = videoAudioSource.gameObject.name.ToLower();
            if (goName.Contains("bgm") || goName.Contains("backgroundmusic") || BGMController.IsBgmSource(videoAudioSource, null))
            {
                videoAudioSource = Undo.AddComponent<AudioSource>(activeVideoPlayer.gameObject);
            }

            Undo.RecordObject(videoAudioSource, "Configure Video AudioSource");
            videoAudioSource.playOnAwake = false;
            videoAudioSource.loop = false;
            videoAudioSource.spatialBlend = 0f;
            videoAudioSource.volume = 1f;
            videoAudioSource.mute = false;
            EditorUtility.SetDirty(videoAudioSource);
        }

        // 2. Konfigurasi VideoPlayer
        if (activeVideoPlayer != null)
        {
            Undo.RecordObject(activeVideoPlayer, "Configure VideoPlayer for StreamingAssets");
            activeVideoPlayer.source = VideoSource.Url;
            activeVideoPlayer.url = ""; // Kosong di Inspector per B4
            activeVideoPlayer.clip = null; // Bersihkan clip agar tidak boros memory build
            activeVideoPlayer.renderMode = VideoRenderMode.RenderTexture;
            activeVideoPlayer.targetTexture = rt;
            activeVideoPlayer.playOnAwake = false;
            activeVideoPlayer.isLooping = false;
            activeVideoPlayer.skipOnDrop = true;
            activeVideoPlayer.playbackSpeed = 1f;

            // Gunakan AudioSource untuk mencegah AudioSampleProvider buffer overflow
            activeVideoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
            activeVideoPlayer.controlledAudioTrackCount = 1;
            activeVideoPlayer.EnableAudioTrack(0, true);
            if (videoAudioSource != null)
            {
                activeVideoPlayer.SetTargetAudioSource(0, videoAudioSource);
            }

            for (ushort i = 1; i < activeVideoPlayer.audioTrackCount; i++)
            {
                activeVideoPlayer.EnableAudioTrack(i, false);
            }

            EditorUtility.SetDirty(activeVideoPlayer);
            Log($"[VIDEO] VideoPlayer '{activeVideoPlayer.gameObject.name}' berhasil dikonfigurasi.");
        }

        // 3. Konfigurasi VideoPanel / RawImage
        if (activeRawImage != null)
        {
            Undo.RecordObject(activeRawImage, "Configure RawImage RenderTexture");
            activeRawImage.texture = rt;
            EditorUtility.SetDirty(activeRawImage);
            Log($"[VIDEO] VideoPanel RawImage '{activeRawImage.gameObject.name}' berhasil dikonfigurasi.");
        }

        // 4. Konfigurasi IntroVideoManager jika ada
        if (activeIntroManager != null)
        {
            Undo.RecordObject(activeIntroManager, "Configure IntroVideoManager");
            activeIntroManager.videoPlayer = activeVideoPlayer;
            activeIntroManager.videoRawImage = activeRawImage;
            activeIntroManager.renderTexture = rt;
            activeIntroManager.videoAudioSource = videoAudioSource;
            if (!string.IsNullOrEmpty(videoFileName))
            {
                activeIntroManager.videoFileName = videoFileName;
            }
            EditorUtility.SetDirty(activeIntroManager);
            Log($"[VIDEO] IntroVideoManager dikonfigurasi dengan video: '{activeIntroManager.videoFileName}'");
        }

        // 5. Konfigurasi PortalVideoController jika ada
        if (activePortalController != null)
        {
            Undo.RecordObject(activePortalController, "Configure PortalVideoController");
            activePortalController.videoPlayer = activeVideoPlayer;
            activePortalController.videoRawImage = activeRawImage;
            activePortalController.renderTexture = rt;
            activePortalController.videoAudioSource = videoAudioSource;
            if (!string.IsNullOrEmpty(videoFileName))
            {
                activePortalController.videoFileName = videoFileName;
            }
            EditorUtility.SetDirty(activePortalController);
            Log($"[VIDEO] PortalVideoController dikonfigurasi dengan video: '{activePortalController.videoFileName}'");
        }

        return true;
    }

    // ========================================================
    // ACTIONS: TAB 3 COMPREHENSIVE VALIDATION
    // ========================================================
    private void RunComprehensiveValidation()
    {
        Log("[GAME SETUP] Mulai validation seluruh Game Scene...");

        hasRunValidation = true;
        validationResults.Clear();
        totalSuccess = 0;
        totalWarnings = 0;
        totalErrors = 0;

        string originalScenePath = SceneManager.GetActiveScene().path;
        List<SceneItem> scenesToCheck = sceneList.FindAll(s => s.isGameScene);

        try
        {
            for (int i = 0; i < scenesToCheck.Count; i++)
            {
                var sc = scenesToCheck[i];
                EditorUtility.DisplayProgressBar("Validating Scenes", $"Memeriksa ({i + 1}/{scenesToCheck.Count}): {sc.sceneName}", (float)i / scenesToCheck.Count);

                if (!File.Exists(sc.scenePath))
                {
                    totalErrors++;
                    continue;
                }

                Scene openedScene = EditorSceneManager.OpenScene(sc.scenePath, OpenSceneMode.Single);
                ValidationEntry entry = new ValidationEntry { sceneName = sc.sceneName };

                // --- 1. VALIDASI MOBILE ---
                GameObject mcGo = FindMobileControlsRoot();
                if (mcGo != null)
                {
                    entry.items.Add("✓ MobileControls");
                    entry.successCount++;

                    RectTransform btnMaju = FindButtonRect(mcGo, "ButtonMaju", "Maju");
                    if (btnMaju != null) { entry.items.Add("✓ ButtonMaju"); entry.successCount++; }
                    else { entry.items.Add("⚠ ButtonMaju tidak ditemukan"); entry.warningCount++; }

                    RectTransform btnMundur = FindButtonRect(mcGo, "ButtonMundur", "Mundur");
                    if (btnMundur != null) { entry.items.Add("✓ ButtonMundur"); entry.successCount++; }
                    else { entry.items.Add("⚠ ButtonMundur tidak ditemukan"); entry.warningCount++; }

                    RectTransform btnLompat = FindButtonRect(mcGo, "ButtonLompat", "Lompat");
                    if (btnLompat != null) { entry.items.Add("✓ ButtonLompat"); entry.successCount++; }
                    else { entry.items.Add("⚠ ButtonLompat tidak ditemukan"); entry.warningCount++; }

                    // Canvas & Raycaster check
                    Canvas cv = mcGo.GetComponentInParent<Canvas>();
                    if (cv != null && cv.GetComponent<GraphicRaycaster>() != null)
                    {
                        entry.items.Add("✓ Canvas & GraphicRaycaster");
                        entry.successCount++;
                    }
                    else
                    {
                        entry.items.Add("⚠ Canvas atau GraphicRaycaster tidak valid");
                        entry.warningCount++;
                    }
                }
                else
                {
                    entry.items.Add("⚠ MobileControls tidak ditemukan");
                    entry.warningCount++;
                }

                // --- 2. VALIDASI VIDEO (jika scene memiliki video) ---
                if (sc.hasVideo)
                {
                    RefreshActiveSceneVideoReferences();
                    if (activeVideoPlayer != null)
                    {
                        entry.items.Add("✓ VideoPlayer");
                        entry.successCount++;
                    }
                    else
                    {
                        entry.items.Add("❌ VideoPlayer tidak ditemukan");
                        entry.errorCount++;
                    }

                    if (activeRawImage != null)
                    {
                        entry.items.Add("✓ VideoPanel");
                        entry.successCount++;
                    }
                    else
                    {
                        entry.items.Add("❌ VideoPanel / RawImage tidak ditemukan");
                        entry.errorCount++;
                    }

                    if (targetRenderTexture != null)
                    {
                        entry.items.Add("✓ RenderTexture");
                        entry.successCount++;
                    }
                    else
                    {
                        entry.items.Add("⚠ RenderTexture belum diset");
                        entry.warningCount++;
                    }

                    // Cek file video di StreamingAssets
                    string resolvedVideo = ResolveVideoFileForScene(sc.sceneName, "");
                    if (VideoUrlHelper.VideoFileExists(resolvedVideo))
                    {
                        entry.items.Add($"✓ Video file ada ({resolvedVideo})");
                        entry.successCount++;
                    }
                    else
                    {
                        entry.items.Add($"⚠ Video file tidak ditemukan di StreamingAssets ({resolvedVideo})");
                        entry.warningCount++;
                    }
                }

                totalSuccess += entry.successCount;
                totalWarnings += entry.warningCount;
                totalErrors += entry.errorCount;
                validationResults.Add(entry);
            }
        }
        finally
        {
            EditorUtility.ClearProgressBar();
            if (!string.IsNullOrEmpty(originalScenePath) && File.Exists(originalScenePath))
            {
                EditorSceneManager.OpenScene(originalScenePath, OpenSceneMode.Single);
            }
        }

        // Print to Unity console in user's desired format
        Log("====================================");
        Log("VALIDATION RESULT");
        Log("====================================");
        foreach (var r in validationResults)
        {
            Log(r.sceneName + ":\n" + string.Join("\n", r.items));
        }
        Log("====================================");
        Log($"Total:\nSuccess: {totalSuccess}\nWarning: {totalWarnings}\nError: {totalErrors}");
        Log("====================================");

        EditorUtility.DisplayDialog("Validation Result", $"Validasi Selesai!\nTotal Success: {totalSuccess}\nTotal Warnings: {totalWarnings}\nTotal Errors: {totalErrors}\n\nLihat Process Log dan Console untuk rincian.", "OK");
    }
}
