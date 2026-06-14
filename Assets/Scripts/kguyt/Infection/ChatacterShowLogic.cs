using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatacterShowLogic : MonoBehaviour
{
    public Image characterShowImage;

    public Sprite ChacterA_Sprite;
    public SO_CharacterData CharacterData_A;
    public Sprite ChacterB_Sprite;
    public SO_CharacterData CharacterData_B;
    public Sprite ChacterC_Sprite;
    public SO_CharacterData CharacterData_C;

    public Animator switchCharacterAnimator;

    public int currentCharacterIndex = 1;

    public TextMeshProUGUI[] characterInfomations;

    int wha;
    public void seeFunc()
    {

        wha = wha + 1 > 2 ? 0 : wha + 1;
        SetCharacterImage(wha);
    }
    public void SetCharacterImage(int characterIndex)
    {
        LoadCharacterInfomation(characterIndex);

        if (currentCharacterIndex == characterIndex)
        {
            return;
        }

        if (currentCharacterIndex > characterIndex)
        {
            SetCharacterByValue(characterIndex, true);
        }
        else
        {
            SetCharacterByValue(characterIndex, false);
        }



    }
    public void SetCharacterByValue(int characterIndex, bool isRev = false)
    {
        if (characterIndex == 0)
        {
            //characterShowImage.sprite = ChacterA_Sprite;
            SetNewCharacter(ChacterA_Sprite, isRev);
        }
        else

        if (characterIndex == 1)
        {
            SetNewCharacter(ChacterB_Sprite, isRev);
            //characterShowImage.sprite = ChacterB_Sprite;
        }
        else
        if (characterIndex == 2)
        {
            SetNewCharacter(ChacterC_Sprite, isRev);
            //characterShowImage.sprite = ChacterC_Sprite;
        }
    }

    public void SetNewCharacter(Sprite sprtie, bool isRev = false)
    {
        StartCoroutine(SetNewCharacterCoroutine(sprtie));
    }

    public IEnumerator SetNewCharacterCoroutine(Sprite sprtie, bool isRev = false)
    {

        if (isRev == true)
        {
            switchCharacterAnimator.SetTrigger("ActiveRev");

        }
        else
        {
            switchCharacterAnimator.SetTrigger("Active");
        }
        yield return new WaitForSeconds(0.25f);
        characterShowImage.sprite = sprtie;
        characterShowImage.SetNativeSize();
        yield return null;
    }

    public void LoadCharacterInfomation(int characterIndedx)
    {
        if (characterIndedx == 0)
        {
            LoadCharacterInformationA();
        }
        else
        if (characterIndedx == 1)
        {
            LoadCharacterInformationB();
        }
        else
        if (characterIndedx == 2)
        {
            LoadCharacterInformationC();
        }
    }

    #region AISHIT
    public void LoadCharacterInformationA()
    {
        characterInfomations[0].text = SaveSystem.SaveFile_instance.isKnow_characterName_A
            ? "姓名：" + CharacterData_A._characterName
            : "姓名：???";

        characterInfomations[1].text = SaveSystem.SaveFile_instance.isKnow_characterNickName_A
            ? "暱稱：" + CharacterData_A._characterNickName
            : "暱稱：???";

        characterInfomations[2].text = SaveSystem.SaveFile_instance.isKnow_birthday_A
            ? "生日：" + CharacterData_A._birthday
            : "生日：???";

        characterInfomations[3].text = SaveSystem.SaveFile_instance.isKnow_starSign_A
            ? "星座：" + CharacterData_A._starSign
            : "星座：???";

        characterInfomations[4].text = SaveSystem.SaveFile_instance.isKnow_likeFood_A
            ? "喜歡：" + CharacterData_A._likeFood
            : "喜歡：???";

        characterInfomations[5].text = SaveSystem.SaveFile_instance.isKnow_hateFood_A
            ? "討厭：" + CharacterData_A._hateFood
            : "討厭：???";

        characterInfomations[6].text = SaveSystem.SaveFile_instance.isKnow_hobby1_A
            ? "興趣：" + CharacterData_A._hobby1
            : "興趣：???";

        characterInfomations[7].text = SaveSystem.SaveFile_instance.isKnow_hobby2_A
            ? "愛好：" + CharacterData_A._hobby2
            : "愛好：???";
    }

    public void LoadCharacterInformationB()
    {
        characterInfomations[0].text = SaveSystem.SaveFile_instance.isKnow_characterName_B
            ? "姓名：" + CharacterData_B._characterName
            : "姓名：???";

        characterInfomations[1].text = SaveSystem.SaveFile_instance.isKnow_characterNickName_B
            ? "暱稱：" + CharacterData_B._characterNickName
            : "暱稱：???";

        characterInfomations[2].text = SaveSystem.SaveFile_instance.isKnow_birthday_B
            ? "生日：" + CharacterData_B._birthday
            : "生日：???";

        characterInfomations[3].text = SaveSystem.SaveFile_instance.isKnow_starSign_B
            ? "星座：" + CharacterData_B._starSign
            : "星座：???";

        characterInfomations[4].text = SaveSystem.SaveFile_instance.isKnow_likeFood_B
            ? "喜歡：" + CharacterData_B._likeFood
            : "喜歡：???";

        characterInfomations[5].text = SaveSystem.SaveFile_instance.isKnow_hateFood_B
            ? "討厭：" + CharacterData_B._hateFood
            : "討厭：???";

        characterInfomations[6].text = SaveSystem.SaveFile_instance.isKnow_hobby1_B
            ? "興趣：" + CharacterData_B._hobby1
            : "興趣：???";

        characterInfomations[7].text = SaveSystem.SaveFile_instance.isKnow_hobby2_B
            ? "愛好：" + CharacterData_B._hobby2
            : "愛好：???";
    }

    public void LoadCharacterInformationC()
    {
        characterInfomations[0].text = SaveSystem.SaveFile_instance.isKnow_characterName_C
            ? "姓名：" + CharacterData_C._characterName
            : "姓名：???";

        characterInfomations[1].text = SaveSystem.SaveFile_instance.isKnow_characterNickName_C
            ? "暱稱：" + CharacterData_C._characterNickName
            : "暱稱：???";

        characterInfomations[2].text = SaveSystem.SaveFile_instance.isKnow_birthday_C
            ? "生日：" + CharacterData_C._birthday
            : "生日：???";

        characterInfomations[3].text = SaveSystem.SaveFile_instance.isKnow_starSign_C
            ? "星座：" + CharacterData_C._starSign
            : "星座：???";

        characterInfomations[4].text = SaveSystem.SaveFile_instance.isKnow_likeFood_C
            ? "喜歡：" + CharacterData_C._likeFood
            : "喜歡：???";

        characterInfomations[5].text = SaveSystem.SaveFile_instance.isKnow_hateFood_C
            ? "討厭：" + CharacterData_C._hateFood
            : "討厭：???";

        characterInfomations[6].text = SaveSystem.SaveFile_instance.isKnow_hobby1_C
            ? "興趣：" + CharacterData_C._hobby1
            : "興趣：???";

        characterInfomations[7].text = SaveSystem.SaveFile_instance.isKnow_hobby2_C
            ? "愛好：" + CharacterData_C._hobby2
            : "愛好：???";
    }
    #endregion
}
