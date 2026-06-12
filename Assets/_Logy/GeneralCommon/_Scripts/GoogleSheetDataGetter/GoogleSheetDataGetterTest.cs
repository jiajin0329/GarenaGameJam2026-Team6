using UnityEngine;

namespace Logy.UnityCommon
{
    [CreateAssetMenu(fileName = nameof(GoogleSheetDataGetterTest), menuName = "ScriptableObject/" + nameof(GoogleSheetDataGetterTest))]
    public class GoogleSheetDataGetterTest : GoogleSheetDataGetter<Test>
    {
        [ContextMenu(nameof(GetGoogleSheetDatas))]
        private void GetGoogleSheetDatas() => _GetGoogleSheetDatas();

        [ContextMenu(nameof(GetCsvDatas))]
        private void GetCsvDatas() => _GetCsvDatas();
    }
}