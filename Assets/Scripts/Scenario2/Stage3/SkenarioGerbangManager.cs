using UnityEngine;

public class SkenarioGerbangManager : MonoBehaviour
{
    public static SkenarioGerbangManager Instance { get; private set; }

    [Header("--- REFERENSI ---")]
    public DialogManager dialogManager;

    private bool sudahDijalankan = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
{
    MulaiDialogAwalStage();
}

    void MulaiDialogAwalStage()
    {
        BarisDialog[] dialogAwal = new BarisDialog[]
        {
            new BarisDialog { 
                namaPembicara = "Pedagang", 
                kalimat = "Ksatria, area pasar ini masih dilanda kutukan kerugian. Maju terus ke depan dan periksa setiap kios yang ada." 
            },
            new BarisDialog { 
                namaPembicara = "Pedagang", 
                kalimat = "Bantu para pedagang menstabilkan perhitungan mereka. Setelah itu, menuju Gerbang Ujung di ujung pasar." 
            },
            new BarisDialog { 
                namaPembicara = "Player", 
                kalimat = "Baik! Aku akan maju ke depan, cek seluruh area pasar, lalu menuju gerbang keluar." 
            }
        };

        if (dialogManager != null)
        {
            dialogManager.TampilkanDialog(dialogAwal, null); // setelah selesai, player bebas jalan
        }
    }

    public void MulaiSkenarioGerbang()
    {
        if (sudahDijalankan) return;
        sudahDijalankan = true;

        if (GerbangUjungManager.Instance == null)
        {
            Debug.LogError("GerbangUjungManager tidak ditemukan di Scene!");
            return;
        }

        BarisDialog[] percakapanGerbang = new BarisDialog[]
        {
            new BarisDialog { namaPembicara = "Player", kalimat = "Syukurlah, perhitungan di kios-kios pasar sudah mulai stabil." },
            new BarisDialog { namaPembicara = "Pedagang", kalimat = "Terima kasih, Ksatria! Tapi sumber kutukan ini belum sepenuhnya hancur. Kamu harus segera ke Rumah Pohon Iklim." },
            new BarisDialog { namaPembicara = "Player", kalimat = "Rumah Pohon Iklim? Di mana letaknya, Pak?" },
            new BarisDialog { namaPembicara = "Pedagang", kalimat = "Tinggal melewati gerbang utama pasar ini. Tapi hati-hati, sihir Raja Anomali telah membuat Rumah Pohon itu membeku!" },
            new BarisDialog { namaPembicara = "Player", kalimat = "Gawat! Aku harus segera ke sana sebelum seluruh pohon tertutup es!" },
            new BarisDialog { namaPembicara = "Pedagang", kalimat = "Tunggu dulu, gerbang ini masih disegel oleh sihir kutukan kerugian. Gabungkan semua ilmu perhitunganmu untuk memecahkan sandinya. Semoga berhasil!" }
        };

        if (dialogManager != null)
        {
            dialogManager.TampilkanDialog(percakapanGerbang, () =>
            {
                Debug.Log("Dialog selesai → Membuka kuis gerbang");
                GerbangUjungManager.Instance.BukaGerbangUjung();
            });
        }
        else
        {
            Debug.LogError("DialogManager kosong di SkenarioGerbangManager!");
            GerbangUjungManager.Instance.BukaGerbangUjung();
        }
    }
}