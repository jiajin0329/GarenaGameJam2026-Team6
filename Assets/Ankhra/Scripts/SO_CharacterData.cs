using UnityEngine;

[CreateAssetMenu(fileName = "SO_CharacterData", menuName = "Scriptable Objects/SO_CharacterData")]
public class SO_CharacterData : ScriptableObject
{
    //
    [Header("Comment")]
    [TextArea(2,13)]
    public string comment = "玩家是否解鎖角色資訊相關資料位於 SaveSystem.SaveFile_instance.isKnow_characterName_A/B/C......";
    
    [Header("Character Show Data")]
    public string _characterName;
    public string _characterNickName;
    public string _birthday;
    public string _starSign;
    public string _likeFood;
    public string _hateFood;
    public string _hobby1;
    public string _hobby2;
}
