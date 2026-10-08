using UnityEngine;

/// <summary>
/// Jembatan kompatibilitas ke GameStatisticsManager.
/// Seluruh pemanggilan lama diarahkan ke GameStatisticsManager agar konsisten di satu sumber data.
/// </summary>
public class UpdatePencapaian : MonoBehaviour
{
    // Panggil fungsi ini setiap kali Player mengambil koin
    public static void TambahKoin(int jumlahKoin)
    {
        GameStatisticsManager.TambahKoin(jumlahKoin);
    }

    // Panggil fungsi ini saat Player menjawab soal (Benar atau Salah)
    public static void CatatJawaban(bool benar)
    {
        if (benar)
        {
            GameStatisticsManager.CatatJawabanBenar();
        }
        else
        {
            GameStatisticsManager.CatatJawabanSalah();
        }
    }

    // Panggil fungsi ini saat Player menyentuh bendera Finish / Portal Stage
    public static void SelesaikanStage(int nomorStage)
    {
        GameStatisticsManager.CatatStageSelesai(nomorStage);
    }

    // Panggil fungsi ini saat Player mengalahkan bos naga (Stage 1/Bos)
    public static void CatatKalahkanBos()
    {
        GameStatisticsManager.CatatBossKalah();
    }
}