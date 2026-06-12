namespace GarenaGameJam2026Team6
{
    public class BeatService
    {
        private BeatModel _model;
        private int _beatCount;

        public BeatService(BeatModel _model)
        {
            this._model = _model;
        }

        public bool CanBeat()
        {
            if (_model.timer < _model.beatInterval)
                return false;

            _model.ClearTimer();
            _beatCount++;

            if (_beatCount == _model.oneTimeBeatAmount)
                return false;

            return true;
        }

        public bool CanFirstBeat()
        {
            if (_beatCount != 1)
                return false;

            return true;
        }

        public bool CanOneTimeBeat()
        {
            if (_beatCount != _model.oneTimeBeatAmount)
                return false;

            _beatCount = 0;

            return true;
        }
    }
}

