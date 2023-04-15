using System;
using System.Threading.Tasks;
using Reversi.Persistence;

namespace Reversi.Model
{
    public enum TableSize { Small, Medium, Large }

    public class ReversiGameModel
    {
        #region Size constants

        private const int small = 10;
        private const int medium = 20;
        private const int large = 30;

        #endregion

        #region Fields

        private  IReversiDataAccess _dataAccess;
        private ReversiTable _table;
        private TableSize _tableSize;

        #endregion

        #region Properties

        public Player CurrentPlayer { get { return _table.CurrentPlayer;  } }

        public int TimeWhite { get { return _table.TimeWhite; } }

        public int TimeBlack { get { return _table.TimeBlack; } }

        public ReversiTable Table { get { return _table; } }

        public bool IsGameOver { get { return _table.ScoreWhite == 0 || _table.ScoreBlack == 0 || 
                                 _table.IsFilled; } }

        public TableSize TableSize { get { return _tableSize; } set { _tableSize = value;  } }

        #endregion

        #region Events

        public event EventHandler<ReversiEventArgs> GameOver;

        public event EventHandler<EventArgs> TimeAdvanced;

        public event EventHandler<EventArgs> FieldsChanged;

        public event EventHandler<ReversiEventArgs> PlayerPassed;

        public event EventHandler<EventArgs> GameCreated;

        #endregion

        #region Constructor

        public ReversiGameModel(IReversiDataAccess dataAcces)
        {
            _dataAccess = dataAcces;
            _tableSize = TableSize.Medium;
            _table = new ReversiTable();
        }

        #endregion

        #region Public game methods

        public void NewGame()
        {
            switch (_tableSize)
            {
                case TableSize.Small:
                    _table = new ReversiTable(small);
                    break;
                case TableSize.Medium:
                    _table = new ReversiTable(medium);
                    break;
                case TableSize.Large:
                    _table = new ReversiTable(large);
                    break;
            }

            OnGameCreated();
        }

        public void AdvanceTime()
        {
            if(IsGameOver)
            {
                return;
            }

            _table.IncreaseTime();

            OnTimeAdvanced();
        }


        public void Step(int x, int y)
        {
            if(IsGameOver)
            {
                return;
            }

            bool moved = _table.MakeMove(x, y);

            if (moved)
            {
                OnFieldsChanged();

                if (IsGameOver)
                {
                    if (_table.ScoreWhite > _table.ScoreBlack)
                    {
                        OnGameOver(Player.PlayerWhite, _table.ScoreWhite, _table.ScoreBlack);
                    }
                    else if (_table.ScoreWhite < _table.ScoreBlack)
                    {
                        OnGameOver(Player.PlayerBlack, _table.ScoreWhite, _table.ScoreBlack);
                    }
                    else
                    {
                        OnGameOver(Player.NoPlayer, _table.ScoreWhite, _table.ScoreBlack);
                    }
                }

                CheckBoard();
            }
        }

        public async Task LoadGameAsync(string path)
        {
            if (_dataAccess == null)
            {
                throw new InvalidOperationException("No data acces provided.");
            }

            _table = await _dataAccess.LoadAsync(path);

            switch (_table.Size)
            {
                case 10:
                    _tableSize = TableSize.Small;
                    break;
                case 20:
                    _tableSize = TableSize.Medium;
                    break;
                case 30:
                    _tableSize = TableSize.Large;
                    break;
            }

            OnGameCreated();
        }

        public async Task SaveGameAsync(string path)
        {
            if(_dataAccess == null)
            {
                throw new InvalidOperationException("No data acces provided.");
            }

            await _dataAccess.SaveAsync(path, _table);
        }

        #endregion

        #region Private game methods

        private void CheckBoard()
        {
            if(!_table.HasAnyValidMove())
            {
                _table.CurrentPlayer = (CurrentPlayer == Player.PlayerWhite) ? 
                                            Player.PlayerBlack : Player.PlayerWhite;
                if (!_table.HasAnyValidMove())
                {
                    if (_table.ScoreWhite > _table.ScoreBlack)
                    {
                        OnGameOver(Player.PlayerWhite, _table.ScoreWhite, _table.ScoreBlack);
                    }
                    else if (_table.ScoreWhite < _table.ScoreBlack)
                    {
                        OnGameOver(Player.PlayerBlack, _table.ScoreWhite, _table.ScoreBlack);
                    }
                    else
                    {
                        OnGameOver(Player.NoPlayer, _table.ScoreWhite, _table.ScoreBlack);
                    }
                }
                else
                {
                    OnPlayerPassed((CurrentPlayer == Player.PlayerWhite) ?
                                            Player.PlayerBlack : Player.PlayerWhite);
                }
            }
        }

        #endregion

        #region Private event methods

        private void OnGameOver(Player winner, int scoreWhite, int scoreBlack)
        {
            if(GameOver != null)
            {
                GameOver(this, new ReversiEventArgs(winner, scoreWhite, scoreBlack));
            }
        }

        private void OnTimeAdvanced()
        {
            if(TimeAdvanced != null)
            {
                TimeAdvanced(this, new EventArgs());
            }
        }

        private void OnFieldsChanged()
        {
            if (FieldsChanged != null)
            {
                FieldsChanged(this, new EventArgs());
            }
        }

        private void OnPlayerPassed(Player p, int x = 0, int y = 0)
        {
            if (PlayerPassed != null)
            {
                PlayerPassed(this, new ReversiEventArgs(p, x, y));
            }
        }

        private void OnGameCreated()
        {
            if (GameCreated != null)
            {
                GameCreated(this, new EventArgs());
            }
        }

        #endregion
    }
}
