using System;
using System.Collections.ObjectModel;
using Reversi.Model;
using Reversi.Persistence;

namespace Reversi.ViewModel
{
    class ReversiViewModel : ViewModelBase
    {
        #region Fields

        private ReversiGameModel _model;
        private bool isPaused;
        private bool isGoing;

        #endregion

        #region Properties

        public DelegateCommand NewGameCommand { get; private set; }

        public DelegateCommand PauseGameCommand { get; private set; }

        public DelegateCommand RestartGameCommand { get; private set; }

        public DelegateCommand LoadGameCommand { get; private set; }

        public DelegateCommand SaveGameCommand { get; private set; }

        public DelegateCommand ExitCommand { get; private set; }

        public ObservableCollection<ReversiField> Fields { get; set; }

        public string TimeWhite { get { return TimeSpan.FromSeconds(_model.TimeWhite).ToString(); } }

        public string TimeBlack { get { return TimeSpan.FromSeconds(_model.TimeBlack).ToString(); } }

        public int Size { get { return _model.Table.Size; } }

        public TableSize TableSize 
        { 
            get { return _model.TableSize; } 
            set 
            {
                if(value != _model.TableSize)
                {
                    _model.TableSize = value;
                    OnPropertyChanged();
                }        
            }
        }

        public bool IsPaused
        {
            get { return isPaused; }
            set
            {
                if(isPaused != value)
                {
                    isPaused = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsGoing
        {
            get { return isGoing; }
            set
            {
                if (isGoing != value)
                {
                    isGoing = value;
                    OnPropertyChanged();
                }
            }
        }

        public int ColorWhite { get { return _model.CurrentPlayer == Player.PlayerWhite ? 1 : 0; } }

        public int ColorBlack { get { return _model.CurrentPlayer == Player.PlayerBlack ? 1 : 0; } }

        #endregion

        #region Events

        public event EventHandler NewGame;

        public event EventHandler PauseGame;

        public event EventHandler RestartGame;

        public event EventHandler LoadGame;

        public event EventHandler SaveGame;

        public event EventHandler ExitGame;

        #endregion

        #region Constructors

        public ReversiViewModel(ReversiGameModel model)
        {
            _model = model;
            _model.GameOver += new EventHandler<ReversiEventArgs>(Model_GameOver);
            _model.TimeAdvanced += new EventHandler<EventArgs>(Model_TimeAdvanced);
            _model.FieldsChanged += new EventHandler<EventArgs>(Model_FieldsChanged);
            _model.PlayerPassed += new EventHandler<ReversiEventArgs>(Model_PlayerPassed);
            _model.GameCreated += new EventHandler<EventArgs>(Model_GameCreated);

            NewGameCommand = new DelegateCommand(param => NewGameSetSize(Convert.ToInt32(param)));
            PauseGameCommand = new DelegateCommand(param => Paused());
            RestartGameCommand = new DelegateCommand(param => Restarted());
            LoadGameCommand = new DelegateCommand(param => OnLoadGame());
            SaveGameCommand = new DelegateCommand(param => OnSaveGame());
            ExitCommand = new DelegateCommand(param => OnExitGame());

            Fields = new ObservableCollection<ReversiField>();
            GenerateTable();
            RefreshTable();
        }

        #endregion

        #region Private methods

        private void GenerateTable()
        {
            Fields.Clear();

            for (int i = 0; i < _model.Table.Size; ++i)
            {
                for (int j = 0; j < _model.Table.Size; ++j)
                {
                    Fields.Add(new ReversiField
                    {
                        X = i,
                        Y = j,
                        Number = i * _model.Table.Size + j,
                        IsActive = true,
                        StepCommand = new DelegateCommand(param => StepGame(Convert.ToInt32(param)))
                    });
                }
            }

            OnPropertyChanged("Size");

            IsPaused = false;
            IsGoing = true;
        }

        private void RefreshTable()
        {
            foreach (ReversiField field in Fields)
            {
                field.IsWhite = _model.Table[field.X, field.Y] == Player.PlayerWhite;
                field.IsBlack = _model.Table[field.X, field.Y] == Player.PlayerBlack;
                field.IsEmpty = _model.Table[field.X, field.Y] == Player.NoPlayer;
                if (!field.IsEmpty)
                {
                    field.IsActive = false;
                }
            }

            OnPropertyChanged("ColorWhite");
            OnPropertyChanged("ColorBlack");
        }

        private void StepGame(int index)
        {
            ReversiField field = Fields[index];

            _model.Step(field.X, field.Y);
        }

        private void NewGameSetSize(int size)
        {
            switch (size)
            {
                case 10:
                    TableSize = TableSize.Small;
                    break;
                case 20:
                    TableSize = TableSize.Medium;
                    break;
                case 30:
                    TableSize = TableSize.Large;
                    break;
                default:
                    break;
            }
            
            OnNewGame();
        }

        private void Paused()
        {
            IsPaused = true;
            IsGoing = false;

            foreach (ReversiField field in Fields)
            {
                field.IsActive = false;
            }

            OnPauseGame();
        }

        private void Restarted()
        {
            IsPaused = false;
            IsGoing = true;

            foreach (ReversiField field in Fields)
            {
                if (field.IsEmpty)
                {
                    field.IsActive = true;
                }
            }

            OnRestartGame();
        }

        #endregion

        #region Game event handlers

        private void Model_GameOver(object sender, ReversiEventArgs e)
        {
            IsPaused = true;
            IsGoing = false;
        }

        private void Model_PlayerPassed(object sender, ReversiEventArgs e)
        {
            OnPropertyChanged("ColorWhite");
            OnPropertyChanged("ColorBlack");

        }

        private void Model_TimeAdvanced(object sender, EventArgs e)
        {
            OnPropertyChanged("TimeWhite");
            OnPropertyChanged("TimeBlack");
        }

        private void Model_FieldsChanged(object sender, EventArgs e)
        {
            RefreshTable();
        }

        private void Model_GameCreated(object sender, EventArgs e)
        {
            GenerateTable();
            RefreshTable();
        }

        #endregion

        #region Event methods

        private void OnNewGame()
        {
            if (NewGame != null)
            {
                NewGame(this, EventArgs.Empty);
            }
        }

        private void OnPauseGame()
        {
            if (PauseGame != null)
            {
                PauseGame(this, EventArgs.Empty);
            }
        }

        private void OnRestartGame()
        {
            if (RestartGame != null)
            {
                RestartGame(this, EventArgs.Empty);
            }
        }

        private void OnLoadGame()
        {
            if (LoadGame != null)
            {
                LoadGame(this, EventArgs.Empty);
            }
        }

        private void OnSaveGame()
        {
            if (SaveGame != null)
            {
                SaveGame(this, EventArgs.Empty);
            }
        }

        private void OnExitGame()
        {
            if (ExitGame != null)
            {
                ExitGame(this, EventArgs.Empty);
            }
        }

        #endregion
    }
}
