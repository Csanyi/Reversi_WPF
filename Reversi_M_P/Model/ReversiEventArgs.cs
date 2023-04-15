using System;
using Reversi.Persistence;

namespace Reversi.Model
{
    public class ReversiEventArgs : EventArgs
    {
        private Player _player;
        private int _scoreWhite;
        private int _scoreBlack;

        public Player Player { get { return _player; } }
        public int ScoreWhite { get { return _scoreWhite; } }
        public int ScoreBlack { get { return _scoreBlack; } }

        public ReversiEventArgs(Player player, int scoreWhite, int scoreBlack)
        {
            _player = player;
            _scoreWhite = scoreWhite;
            _scoreBlack = scoreBlack;
        }
    }
}
