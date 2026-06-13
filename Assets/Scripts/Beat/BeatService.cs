namespace GarenaGameJam2026Team6
{
    public class BeatService
    {
        private BeatModel _model;

        public BeatService(BeatModel _model)
        {
            this._model = _model;
        }

        public bool CanBeat()
        {
            if (_model.timer < _model.beatInterval)
                return false;

            return true;
        }

        public bool CanOneTimeBeat()
        {
            if (_model.beatCount == _model.oneTimeBeatAmount)
                return true;

            return false;
        }
    }
}

