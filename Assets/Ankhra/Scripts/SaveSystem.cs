using UnityEngine;
using System.IO;
public class SaveSystem : MonoBehaviour
{
    static public SaveFile SaveFile_instance;

    [Header("存檔可視化")]
    [SerializeField] bool SFSync;
    public SaveFile SFShowCase;

    static public string saveFilePath = "SaveFile" + 0;

    private void Awake()
    {
        Debug.Log("自動載入");
        LoadSF();
    }

    private void Update()
    {
        if (SFSync)
        {
            SFShowCase = SaveFile_instance;

        }
        SaveSF();
    }
    static public void SaveSF()
    {
        string fullPath = Application.persistentDataPath + saveFilePath;
        File.WriteAllText(fullPath, JsonUtility.ToJson(SaveFile_instance));
    }

    static public void LoadSF()
    {
        string fullPath = Application.persistentDataPath + saveFilePath;

        if (string.IsNullOrEmpty(fullPath) || !File.Exists(fullPath))
        {
            Debug.Log("載入失敗：路徑是空的或找不到檔案，嘗試生成一個新的檔案。");
            ResetSF();
        }

        SaveFile sSF = JsonUtility.FromJson<SaveFile>(File.ReadAllText(fullPath));
        SaveFile_instance = sSF;
    }

    static public void ResetSF()
    {
        string fullPath = Application.persistentDataPath + saveFilePath;

        SaveFile sSF = new SaveFile();
        File.WriteAllText(fullPath, JsonUtility.ToJson(sSF));

        SaveFile_instance = sSF;
    }
}

[System.Serializable]
public class SaveFile
{
    // Character A
    public bool isKnow_characterName_A;
    public bool isKnow_characterNickName_A;
    public bool isKnow_birthday_A;
    public bool isKnow_starSign_A;
    public bool isKnow_likeFood_A;
    public bool isKnow_hateFood_A;
    public bool isKnow_hobby1_A;
    public bool isKnow_hobby2_A;

    // Character B
    public bool isKnow_characterName_B;
    public bool isKnow_characterNickName_B;
    public bool isKnow_birthday_B;
    public bool isKnow_starSign_B;
    public bool isKnow_likeFood_B;
    public bool isKnow_hateFood_B;
    public bool isKnow_hobby1_B;
    public bool isKnow_hobby2_B;

    // Character C
    public bool isKnow_characterName_C;
    public bool isKnow_characterNickName_C;
    public bool isKnow_birthday_C;
    public bool isKnow_starSign_C;
    public bool isKnow_likeFood_C;
    public bool isKnow_hateFood_C;
    public bool isKnow_hobby1_C;
    public bool isKnow_hobby2_C;
}