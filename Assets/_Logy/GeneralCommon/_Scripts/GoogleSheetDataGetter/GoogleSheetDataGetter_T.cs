using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Networking;

namespace Logy.UnityCommon
{
    public abstract class GoogleSheetDataGetter<T> : ScriptableObject
    {
        [SerializeField]
        private bool _loadGoogleSheetOnInitialize;

        [SerializeField]
        private string _googleSheetURL;

        [SerializeField]
        private byte _pageNumber = 1;

        [SerializeField]
        private TextAsset _csvFile;

        [SerializeField]
        private T[] _dataArray;

        public void Initialize()
        {
            if (_loadGoogleSheetOnInitialize)
                _GetGoogleSheetDatas();
        }

        protected void _GetGoogleSheetDatas()
        {
            if (string.IsNullOrEmpty(_googleSheetURL))
                throw new ArgumentException("Google Sheet URL is null or empty.");

            UnityWebRequest _request = UnityWebRequest.Get(ConvertURL());
            var _asyncOperation = _request.SendWebRequest();

            _asyncOperation.completed += (operation) =>
            {
                if (_request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"Failed to load Google Sheet: {_request.error}");
                    return;
                }

                var _jsonText = _request.downloadHandler.text;
                var _json = "{\"Items\":" + _jsonText + "}";
                _dataArray = JsonUtility.FromJson<Wrapper<T>>(_json).Items;

                // 在 Callback 模式中必須手動 Dispose
                _request.Dispose();
            };
        }

        [Serializable]
        private class Wrapper<T> { public T[] Items; }

        private string ConvertURL()
        {
            // Extract spreadsheet ID between "/d/" and the next "/"
            const string _prefix = "/d/";
            var _startIndex = _googleSheetURL.IndexOf(_prefix) + _prefix.Length;
            var _endIndex = _googleSheetURL.IndexOf('/', _startIndex);
            var _spreadsheetId = _googleSheetURL.Substring(_startIndex, _endIndex - _startIndex);
            var _convertURL = $"https://opensheet.elk.sh/{_spreadsheetId}/{_pageNumber}";

            Debug.Log($"{nameof(ConvertURL)} : {_convertURL}");

            return _convertURL;
        }

        protected void _GetCsvDatas()
        {
            if (_csvFile == null)
                throw new ArgumentException("CSV file is null.");

            var _lines = _csvFile.text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            if (_lines.Length < 2)
            {
                Debug.LogError("CSV file has no data rows.");
                return;
            }

            var _headers = _lines[0].Split(',');
            var _type = typeof(T);
            var _fieldMap = new Dictionary<int, FieldInfo>();

            for (var i = 0; i < _headers.Length; i++)
            {
                var _field = _type.GetField(_headers[i].Trim(), BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

                if (_field != null)
                    _fieldMap[i] = _field;
                else
                    Debug.LogError($"Field '{_headers[i].Trim()}' not found in {_type.Name}.");
            }

            var _dataList = new List<T>();

            for (var i = 1; i < _lines.Length; i++)
            {
                var _values = _lines[i].Split(',');
                var _instance = (T)Activator.CreateInstance(_type);

                foreach (var _kvp in _fieldMap)
                {
                    if (_kvp.Key >= _values.Length)
                        continue;

                    var _value = _values[_kvp.Key].Trim();
                    _kvp.Value.SetValue(_instance, Convert.ChangeType(_value, _kvp.Value.FieldType));
                }

                _dataList.Add(_instance);
            }

            _dataArray = _dataList.ToArray();
        }
    }
}