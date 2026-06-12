using UnityEngine;

namespace Logy.UnityCommon.ProgressV01
{
    public class ProgressManager : MonoBehaviour
    {
        [SerializeField]
        private Progress[] _progressArray;

        private void Awake()
        {
            for (int i = 0; i < _progressArray.Length; i++)
            {
                _progressArray[i].Initialize();
            }
        }
    }
}