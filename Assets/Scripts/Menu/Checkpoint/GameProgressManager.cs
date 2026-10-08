using UnityEngine;

/// <summary>
/// GameProgressManager: Wrapper ke CheckpointManager untuk memastikan SATU sumber kebenaran (Single Source of Truth).
/// </summary>
public static class GameProgressManager
{
    public const int TOTAL_STAGE = CheckpointManager.TOTAL_STAGE;
    public const int TOTAL_LEVEL = CheckpointManager.TOTAL_LEVEL;
    public const int TOTAL_SUB_LEVEL = CheckpointManager.TOTAL_SUBLEVEL;

    public static void SelesaikanSubLevel(int stage, int level, int subLevel)
    {
        if (CheckpointManager.Instance != null)
        {
            CheckpointManager.Instance.SelesaikanSubLevel(stage, level, subLevel);
        }
        else
        {
            CheckpointManager.CatatProgressSceneSelesai(CheckpointManager.GetNamaScene(stage, level, subLevel));
        }
    }

    public static bool SubLevelSelesai(int stage, int level, int subLevel)
    {
        return CheckpointManager.IsSubLevelSelesaiStatic(stage, level, subLevel);
    }

    public static bool LevelSelesai(int stage, int level)
    {
        return CheckpointManager.IsLevelSelesaiStatic(stage, level);
    }

    public static int HitungBintangStage(int stage)
    {
        if (CheckpointManager.Instance != null)
            return CheckpointManager.Instance.HitungBintangStage(stage);

        int jumlahBintang = 0;
        for (int lvl = 1; lvl <= TOTAL_LEVEL; lvl++)
        {
            if (LevelSelesai(stage, lvl))
                jumlahBintang++;
        }
        return jumlahBintang;
    }

    public static bool StageSelesai(int stage)
    {
        return HitungBintangStage(stage) >= TOTAL_LEVEL;
    }

    public static bool StageTerbuka(int stage)
    {
        if (stage <= 1) return true;
        return StageSelesai(stage - 1);
    }

    public static bool LevelTerbuka(int stage, int level)
    {
        if (!StageTerbuka(stage)) return false;
        if (level <= 1) return true;
        return LevelSelesai(stage, level - 1);
    }

    public static void ResetProgress()
    {
        CheckpointManager.ResetProgressKhususCheckpoint();
    }
}