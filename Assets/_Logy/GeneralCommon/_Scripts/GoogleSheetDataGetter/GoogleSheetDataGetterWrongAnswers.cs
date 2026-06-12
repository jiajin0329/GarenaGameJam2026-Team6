using UnityEngine;

namespace Logy.UnityCommon
{
    [CreateAssetMenu(fileName = nameof(GoogleSheetDataGetterWrongAnswers), menuName = "ScriptableObject/" + nameof(GoogleSheetDataGetterWrongAnswers))]
    public class GoogleSheetDataGetterWrongAnswers : GoogleSheetDataGetter<WrongAnswers>
    {
        [ContextMenu(nameof(GetGoogleSheetDatas))]
        private void GetGoogleSheetDatas() => _GetGoogleSheetDatas();

        [ContextMenu(nameof(GetCsvDatas))]
        private void GetCsvDatas() => _GetCsvDatas();
    }
}