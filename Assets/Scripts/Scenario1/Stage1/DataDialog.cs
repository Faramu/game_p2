using UnityEngine;

[System.Serializable]
public class BarisDialog
{
    [Tooltip("Siapa yang bicara?")]
    public string namaPembicara;
    
    [TextArea(3, 5)]
    [Tooltip("Apa yang dibicarakan?")]
    public string kalimat;
}