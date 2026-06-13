namespace GarenaGameJam2026Team6
{
    public class EndService
    {
        private EndModel _model;
        private Affinity[] _affinityArrary;

        private int _biggestAffinityIndex;
        private float _biggestAffinity;

        public EndService(EndModel _model, Affinity[] _affinityArrary)
        {
            this._model = _model;
            this._affinityArrary = _affinityArrary;
        }

        public bool CanJudeEnd()
        {
            return _model.finishCount >= 3;
        }

        public bool CanAllCharacterEnd()
        {
            return _affinityArrary[0].current > 9 && _affinityArrary[1].current > 9 && _affinityArrary[2].current > 9;
        }

        public bool CanCharacterAEnd()
        {
            FindBiggestAffinity();

            return _biggestAffinityIndex == 0 && _biggestAffinity > 8f;
        }

        private void FindBiggestAffinity()
        {
            for (int i = 0; i < _affinityArrary.Length; i++)
            {
                if (_affinityArrary[i].current > _biggestAffinity)
                {
                    _biggestAffinity = _affinityArrary[i].current;
                    _biggestAffinityIndex = i;
                }
            }
        }

        public bool CanCharacterBEnd()
        {
            FindBiggestAffinity();

            return _biggestAffinityIndex == 1 && _biggestAffinity > 8f;
        }

        public bool CanCharacterCEnd()
        {
            FindBiggestAffinity();

            return _biggestAffinityIndex == 2 && _biggestAffinity > 8f;
        }
    }
}
