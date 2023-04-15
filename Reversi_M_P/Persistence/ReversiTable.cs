using System;

namespace Reversi.Persistence
{
    public class ReversiTable
    {
        #region Fields

        private int _tableSize;
        private Player[,] _fieldValues;
        private int _timeWhite;
        private int _timeBlack;
        private int _scoreWhite;
        private int _scoreBlack;
        private Player _currentPlayer;
        private readonly string[] directions = { "topLeft", "topCenter", "topRight", "rightCenter",
                                        "bottomRight", "bottomCenter", "bottomLeft", "leftCenter" };

        #endregion

        #region Properties

        public bool IsFilled
        {
            get
            {
                foreach (Player p in _fieldValues)
                    if (p == Player.NoPlayer)
                        return false;
                return true;
            }
        }

        public int Size { get { return _tableSize; } }

        public Player this[int x, int y] { get { return GetValue(x, y); } }

        public int TimeWhite
        {
            get { return _timeWhite; }
            set { _timeWhite = value; }
        }

        public int TimeBlack 
        { 
            get { return _timeBlack;  }
            set { _timeBlack = value;  }   
        }

        public Player CurrentPlayer
        {
            get { return _currentPlayer; }
            set { _currentPlayer = value; }
        }

        public int ScoreWhite
        {
            get { return _scoreWhite; }
            set { _scoreWhite = value; }
        }

        public int ScoreBlack
        {
            get { return _scoreBlack; }
            set { _scoreBlack = value; }
        }

        #endregion

        #region Constructors

        public ReversiTable() : this(20) { }

        public ReversiTable(int x)
        {
            if (x <= 2)
                throw new ArgumentOutOfRangeException("Invalid table size.", "x");
            if (x % 2 == 1)
                throw new ArgumentException("The table size must be even", "x");

            _tableSize = x;
            _fieldValues = new Player[x, x];
            _timeBlack = 0;
            _timeWhite = 0;
            _scoreWhite = 2;
            _scoreBlack = 2;
            _currentPlayer = Player.PlayerBlack;

            for (int i = 0; i < x; ++i)
            {
                for (int j = 0; j < x; ++j)
                {
                    _fieldValues[i, j] = Player.NoPlayer;
                }
            }

            _fieldValues[_tableSize / 2, _tableSize / 2] = Player.PlayerWhite;
            _fieldValues[(_tableSize / 2) - 1, (_tableSize / 2) - 1] = Player.PlayerWhite;
            _fieldValues[(_tableSize / 2) - 1, _tableSize / 2] = Player.PlayerBlack;
            _fieldValues[_tableSize / 2, (_tableSize / 2) - 1] = Player.PlayerBlack;
        }

        #endregion

        #region Public methods

        public bool IsEmpty(int x, int y)
        {
            if (x < 0 || x >= _fieldValues.GetLength(0))
                throw new ArgumentOutOfRangeException("x", "The X coordinate is out of range.");
            if (y < 0 || y >= _fieldValues.GetLength(1))
                throw new ArgumentOutOfRangeException("y", "The Y coordinate is out of range.");

            return _fieldValues[x, y] == Player.NoPlayer;
        }

        public Player GetValue(int x, int y)
        {
            if (x < 0 || x >= _fieldValues.GetLength(0))
                throw new ArgumentOutOfRangeException("x", "The X coordinate is out of range.");
            if (y < 0 || y >= _fieldValues.GetLength(1))
                throw new ArgumentOutOfRangeException("y", "The Y coordinate is out of range.");

            return _fieldValues[x, y];
        }

        public void SetValue(int x, int y, Player p)
        {
            if (x < 0 || x >= _fieldValues.GetLength(0))
                throw new ArgumentOutOfRangeException("x", "The X coordinate is out of range.");
            if (y < 0 || y >= _fieldValues.GetLength(1))
                throw new ArgumentOutOfRangeException("y", "The Y coordinate is out of range.");
        
            _fieldValues[x, y] = p;
        }

        public void IncreaseTime()
        {
            if(_currentPlayer == Player.PlayerWhite)
            {
                ++_timeWhite;
            }
            else if(_currentPlayer == Player.PlayerBlack)
            {
                ++_timeBlack;
            }
            else 
            {
                throw new Exception("Cannot increase NoPlayer time!");
            }
        }

        public bool MakeMove(int x, int y)
        {
            if (x < 0 || x >= _fieldValues.GetLength(0))
            {
                throw new ArgumentOutOfRangeException("x", "The X coordinate is out of range.");
            }
            if (y < 0 || y >= _fieldValues.GetLength(1))
            {
                throw new ArgumentOutOfRangeException("y", "The Y coordinate is out of range.");
            }
            if (!IsValidMove(x, y))
            {
                return false;
            }
                
            _fieldValues[x, y] = _currentPlayer;
            int changeX = 0;
            int changeY = 0;
            int startX = x;
            int startY = y;

            if (_currentPlayer == Player.PlayerWhite)
            {
                ++_scoreWhite;
            }
            else if (_currentPlayer == Player.PlayerBlack)
            {
                ++_scoreBlack;  
            }
            else
            {
                throw new Exception("Cannot change NoPlayer score!");
            }

            for (int i = 0; i < directions.Length; ++i)
            {
                x = startX;
                y = startY;
                SetDirectionValues(ref changeX, ref changeY, directions[i]);
                int n = CountOtherPlayer(x, y, directions[i]);

                if(_currentPlayer == Player.PlayerWhite)
                {
                    _scoreWhite += n;
                    _scoreBlack -= n;
                }
                else if (_currentPlayer == Player.PlayerBlack)
                {
                    _scoreBlack += n;
                    _scoreWhite -= n;
                }
                else
                {
                    throw new Exception("Cannot change NoPlayer score!");
                }

                for (int j = 0; j < n; ++j)
                {
                    x += changeX;
                    y += changeY;

                    if (_fieldValues[x, y] == Player.PlayerWhite)
                    {
                        _fieldValues[x, y] = Player.PlayerBlack;
                    }
                    else if (_fieldValues[x, y] == Player.PlayerBlack)
                    {
                        _fieldValues[x, y] = Player.PlayerWhite;
                    }
                }
            }

            _currentPlayer = (_currentPlayer == Player.PlayerWhite) ? Player.PlayerBlack : Player.PlayerWhite;

            return true;
        }

        public bool HasAnyValidMove()
        {
            for (int i = 0; i < _tableSize; ++i)
            {
                for (int j = 0; j < _tableSize; ++j)
                {
                    if (IsValidMove(i, j))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        #endregion

        #region Private methods

        private bool IsValidMove(int x, int y)
        {
            if (_fieldValues[x, y] != Player.NoPlayer)
            {
                return false;
            }
            else
            {
                for (int i = 0; i < directions.Length; ++i)
                {
                    if (CountOtherPlayer(x, y, directions[i]) > 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private int CountOtherPlayer(int x, int y, string direction)
        {
            int changeX = 0;
            int changeY = 0;

            SetDirectionValues(ref changeX, ref changeY, direction);

            int ctr = 0;
            x += changeX;
            y += changeY;

            while (x >= 0 && y >= 0 && x < _tableSize && y < _tableSize &&
                _fieldValues[x, y] != _currentPlayer && _fieldValues[x, y] != Player.NoPlayer)
            {
                ++ctr;
                x += changeX;
                y += changeY;
            }

            if(x < 0 || y < 0 || x >= _tableSize || y >= _tableSize)
            {
                ctr = 0;
            }
            else if (x >= 0 && y >= 0 && x < _tableSize && y < _tableSize && _fieldValues[x, y] != _currentPlayer)
            {
                ctr = 0;
            }
            
            return ctr;
        }

        private void SetDirectionValues(ref int x, ref int y, string direction)
        {
            switch (direction)
            {
                case "topLeft":
                    x = -1;
                    y = -1;
                    break;
                case "topCenter":
                    x = -1;
                    y = 0;
                    break;
                case "topRight":
                    x = -1;
                    y = 1;
                    break;
                case "rightCenter":
                    x = 0;
                    y = 1;
                    break;
                case "bottomRight":
                    x = 1;
                    y = 1;
                    break;
                case "bottomCenter":
                    x = 1;
                    y = 0;
                    break;
                case "bottomLeft":
                    x = 1;
                    y = -1;
                    break;
                case "leftCenter":
                    x = 0;
                    y = -1;
                    break;
                default:
                    throw new ArgumentException("Invalid direction!");
            }
        }

        #endregion
    }
}
