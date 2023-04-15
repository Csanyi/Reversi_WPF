using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;
using Reversi.Model;
using Reversi.Persistence;
using Reversi.View;
using Reversi.ViewModel;
using Microsoft.Win32;

namespace Reversi
{
    public partial class App : Application
    {
        #region Fields

        private ReversiGameModel _model;
        private ReversiViewModel _viewModel;
        private MainWindow _view;
        private DispatcherTimer _timer;

        #endregion

        #region Constructors

        public App()
        {
            Startup += App_Startup;
        }

        #endregion

        #region Application event handlers

        private void App_Startup(object sender, StartupEventArgs e)
        {
            _model = new ReversiGameModel(new ReversiFileDataAccess());
            _model.GameOver += new EventHandler<ReversiEventArgs>(Model_GameOver);
            _model.PlayerPassed += new EventHandler<ReversiEventArgs>(Model_PlayerPassed);
            _model.NewGame();

            _viewModel = new ReversiViewModel(_model);
            _viewModel.NewGame += new EventHandler(ViewModel_NewGame);
            _viewModel.PauseGame += new EventHandler(ViewModel_PauseGame);
            _viewModel.RestartGame += new EventHandler(ViewModel_RestartGame);
            _viewModel.LoadGame += new EventHandler(ViewModel_LoadGame);
            _viewModel.SaveGame += new EventHandler(ViewModel_SaveGame);
            _viewModel.ExitGame += new EventHandler(ViewModel_ExitGame);

            _view = new MainWindow();
            _view.DataContext = _viewModel;
            _view.Closing += new System.ComponentModel.CancelEventHandler(View_Closing);
            _view.Show();

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += new EventHandler(Timer_Tick);
            _timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            _model.AdvanceTime();
        }

        #endregion

        #region View event handlers

        private void View_Closing(object sender, CancelEventArgs e)
        {
            bool restartTimer = _timer.IsEnabled;

            _timer.Stop();

            if (MessageBox.Show("Are you sure you want to exit?", "Reversi", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
            {
                e.Cancel = true;

                if (restartTimer)
                {
                    _timer.Start();
                }   
            }
        }

        #endregion

        #region ViewModel event handlers

        private void ViewModel_NewGame(object sender, EventArgs e)
        {
            _model.NewGame();
            _timer.Start();
        }


        private void ViewModel_PauseGame(object sender, EventArgs e)
        {
            _timer.Stop();

        }

        private void ViewModel_RestartGame(object sender, EventArgs e)
        {
            _timer.Start();
        }

        private async void ViewModel_LoadGame(object sender, EventArgs e)
        {
            bool restartTimer = _timer.IsEnabled;

            _timer.Stop();

            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Title = "Reversi table load";
                openFileDialog.Filter = "Reversi table|*.rtl";
                if (openFileDialog.ShowDialog() == true)
                {
                    await _model.LoadGameAsync(openFileDialog.FileName);

                    _timer.Start();
                }
            }
            catch (ReversiDataException)
            {
                MessageBox.Show("Game load failed!", "Reversi", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            if (restartTimer)
            {
                _timer.Start();
            }
        }

        private async void ViewModel_SaveGame(object sender, EventArgs e)
        {
            bool restartTimer = _timer.IsEnabled;

            _timer.Stop();

            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "Reversi table save";
                saveFileDialog.Filter = "Reverst table|*.rtl";
                if (saveFileDialog.ShowDialog() == true)
                {
                    try
                    {
                        await _model.SaveGameAsync(saveFileDialog.FileName);
                    }
                    catch (ReversiDataException)
                    {
                        MessageBox.Show("Game save failed!" + Environment.NewLine + "The path is incorrect or the directory cannot be written.", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch
            {
                MessageBox.Show("Game save failed!", "Reversi", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            if (restartTimer)
            {
                _timer.Start();
            }
        }

        private void ViewModel_ExitGame(object sender, EventArgs e)
        {
            _view.Close();
        }

        #endregion

        #region Model event handlers

        private void Model_GameOver(object sender, ReversiEventArgs e)
        {
            _timer.Stop();

            if (e.Player == Player.PlayerWhite)
            {
                MessageBox.Show("White player won! (" + e.ScoreWhite + " vs " + e.ScoreBlack + ")",
                    "Reversi", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }
            else if (e.Player == Player.PlayerBlack)
            {
                MessageBox.Show("Black player won! (" + e.ScoreWhite + " vs " + e.ScoreBlack + ")",
                    "Reversi", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }
            else
            {
                MessageBox.Show("Draw!", "Reversi", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }

            _model.NewGame();
            _timer.Start();
        }

        private void Model_PlayerPassed(object sender, ReversiEventArgs e)
        {
            _timer.Stop();

            if (e.Player == Player.PlayerWhite)
            {
                MessageBox.Show("There is no valid move, black player's turn.", "Reversi",
                   MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (e.Player == Player.PlayerBlack)
            {
                MessageBox.Show("There is no valid move, white player's turn", "Reversi",
                   MessageBoxButton.OK, MessageBoxImage.Information);
            }

            _timer.Start();
        }

        #endregion
    }
}
