using UnityEngine;

namespace Logy.UnityCommon
{
    [CreateAssetMenu(fileName = nameof(GoogleSheetDataGetterQuestions), menuName = "ScriptableObject/" + nameof(GoogleSheetDataGetterQuestions))]
    public class GoogleSheetDataGetterQuestions : GoogleSheetDataGetter<Questions>
    {
        [ContextMenu(nameof(GetGoogleSheetDatas))]
        private void GetGoogleSheetDatas() => _GetGoogleSheetDatas();

        [ContextMenu(nameof(GetCsvDatas))]
        private void GetCsvDatas() => _GetCsvDatas();
    }
}