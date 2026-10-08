using UnityEditor;
using UnityEngine;

public class QuestionBankValidatorWindow : EditorWindow
{
    private Vector2 scrollPos;
    private QuestionBankValidator.ValidationReport lastReport;

    [MenuItem("Tools/Validasi Bank Soal")]
    public static void ShowWindow()
    {
        var window = GetWindow<QuestionBankValidatorWindow>("Validasi Bank Soal");
        window.minSize = new Vector2(450, 400);
        window.Jalankan();
    }

    private void Jalankan()
    {
        lastReport = QuestionBankValidator.JalankanValidasi(true);
    }

    private void OnGUI()
    {
        EditorGUILayout.Space(10);
        GUILayout.Label("VALIDASI BANK SOAL OTOMATIS", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("Alat ini memverifikasi jumlah soal unik per stage (target minimal: 100 soal/stage) serta validitas konten soal.", MessageType.Info);

        EditorGUILayout.Space(5);
        if (GUILayout.Button("Jalankan Validasi Sekarang", GUILayout.Height(32)))
        {
            Jalankan();
        }

        EditorGUILayout.Space(10);

        if (lastReport == null)
        {
            EditorGUILayout.HelpBox("Tekan tombol di atas untuk memulai validasi.", MessageType.None);
            return;
        }

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        EditorGUILayout.LabelField("HASIL VALIDASI PER STAGE:", EditorStyles.boldLabel);
        for (int s = 1; s <= 6; s++)
        {
            int count = lastReport.SoalPerStage.ContainsKey(s) ? lastReport.SoalPerStage[s] : 0;
            string status = count >= 100 ? "PASS (>=100)" : $"WARNING ({count} < 100)";
            Color col = count >= 100 ? Color.green : Color.yellow;

            EditorGUILayout.BeginHorizontal("box");
            EditorGUILayout.LabelField($"Stage {s}", GUILayout.Width(80));
            EditorGUILayout.LabelField($"{count} soal", GUILayout.Width(80));

            GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
            style.normal.textColor = col;
            EditorGUILayout.LabelField(status, style);
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.Space(10);
        if (lastReport.IsPass)
        {
            EditorGUILayout.HelpBox("[QUESTION VALIDATION] PASS - Semua stage memenuhi target!", MessageType.Info);
        }
        else
        {
            EditorGUILayout.HelpBox($"[QUESTION VALIDATION] Terdapat {lastReport.Warnings.Count} catatan/warning.", MessageType.Warning);
            foreach (var w in lastReport.Warnings)
            {
                EditorGUILayout.LabelField($"• {w}", EditorStyles.wordWrappedLabel);
            }
        }

        if (lastReport.Errors.Count > 0)
        {
            EditorGUILayout.Space(5);
            EditorGUILayout.HelpBox($"Ditemukan {lastReport.Errors.Count} error pada data soal!", MessageType.Error);
            foreach (var e in lastReport.Errors)
            {
                EditorGUILayout.LabelField($"✖ {e}", EditorStyles.wordWrappedLabel);
            }
        }

        EditorGUILayout.EndScrollView();
    }
}
