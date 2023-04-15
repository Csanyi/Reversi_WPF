using System;
using System.Threading.Tasks;
using Reversi.Model;
using Reversi.Persistence;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Reversi.Test
{
    [TestClass]
    public class ReversiGameModelTest
    {
        private ReversiGameModel _model;
        private ReversiTable _mockedTable;
        private Mock<IReversiDataAccess> _mock;

        [TestInitialize]
        public void Initialize()
        {
            _mockedTable = new ReversiTable();
            _mockedTable.SetValue(1, 2, Player.PlayerWhite);
            _mockedTable.SetValue(10, 12, Player.PlayerWhite);
            _mockedTable.SetValue(8, 4, Player.PlayerWhite);
            _mockedTable.SetValue(16, 2, Player.PlayerBlack);
            _mockedTable.SetValue(11, 15, Player.PlayerBlack);
            _mockedTable.SetValue(19, 19, Player.PlayerBlack);


            _mock = new Mock<IReversiDataAccess>();
            _mock.Setup(mock => mock.LoadAsync(It.IsAny<String>()))
                .Returns(() => Task.FromResult(_mockedTable));

            _model = new ReversiGameModel(_mock.Object);
        }

        [TestMethod]
        public void ReversiGameModelNewGameMediumTest()
        {
            _model.NewGame();

            Assert.AreEqual(2, _model.Table.ScoreWhite);
            Assert.AreEqual(2, _model.Table.ScoreBlack);
            Assert.AreEqual(Player.PlayerBlack, _model.Table.CurrentPlayer);
            Assert.AreEqual(TableSize.Medium, _model.TableSize);
            Assert.AreEqual(0, _model.TimeWhite);
            Assert.AreEqual(0, _model.TimeBlack);

            int emptyFields = 0;
            for (int i = 0; i < _model.Table.Size; i++)
                for (int j = 0; j < _model.Table.Size; j++)
                    if (_model.Table.IsEmpty(i, j))
                        emptyFields++;

            Assert.AreEqual(396, emptyFields);
        }

        [TestMethod]
        public void ReversiGameModelNewGameSmallTest()
        {
            _model.TableSize = TableSize.Small;
            _model.NewGame();

            Assert.AreEqual(2, _model.Table.ScoreWhite);
            Assert.AreEqual(2, _model.Table.ScoreBlack);
            Assert.AreEqual(Player.PlayerBlack, _model.Table.CurrentPlayer);
            Assert.AreEqual(TableSize.Small, _model.TableSize);
            Assert.AreEqual(0, _model.TimeWhite);
            Assert.AreEqual(0, _model.TimeBlack);

            int emptyFields = 0;
            for (int i = 0; i < _model.Table.Size; i++)
                for (int j = 0; j < _model.Table.Size; j++)
                    if (_model.Table.IsEmpty(i, j))
                        emptyFields++;

            Assert.AreEqual(96, emptyFields);
        }

        [TestMethod]
        public void ReversiGameModelNewGameLargeTest()
        {
            _model.TableSize = TableSize.Large;
            _model.NewGame();

            Assert.AreEqual(2, _model.Table.ScoreWhite);
            Assert.AreEqual(2, _model.Table.ScoreBlack);
            Assert.AreEqual(Player.PlayerBlack, _model.Table.CurrentPlayer);
            Assert.AreEqual(TableSize.Large, _model.TableSize);
            Assert.AreEqual(0, _model.TimeWhite);
            Assert.AreEqual(0, _model.TimeBlack);

            int emptyFields = 0;
            for (int i = 0; i < _model.Table.Size; i++)
                for (int j = 0; j < _model.Table.Size; j++)
                    if (_model.Table.IsEmpty(i, j))
                        emptyFields++;

            Assert.AreEqual(896, emptyFields);
        }

        [TestMethod]
        public void ReversiGameModelStepTest()
        {
            Assert.AreEqual(2, _model.Table.ScoreWhite);
            Assert.AreEqual(2, _model.Table.ScoreBlack);

            _model.NewGame();

            int x = 8, y = 9;
            _model.Step(x, y);  // szabályos lépés

            Assert.AreEqual(Player.PlayerBlack, _model.Table[x, y]);
            Assert.AreEqual(Player.PlayerWhite, _model.Table.CurrentPlayer);
            Assert.AreEqual(0, _model.TimeWhite);
            Assert.AreEqual(0, _model.TimeBlack);
            Assert.AreEqual(1, _model.Table.ScoreWhite);
            Assert.AreEqual(4, _model.Table.ScoreBlack);

            x = 0; y = 0;
            _model.Step(x, y); // nem üt 

            Assert.AreEqual(Player.NoPlayer, _model.Table[x, y]);
            Assert.AreEqual(Player.PlayerWhite, _model.Table.CurrentPlayer);
            Assert.AreEqual(0, _model.TimeWhite);
            Assert.AreEqual(0, _model.TimeBlack);
            Assert.AreEqual(1, _model.Table.ScoreWhite);
            Assert.AreEqual(4, _model.Table.ScoreBlack);

            x = 8; y = 9;
            _model.Step(x, y); // nem üresre lép

            Assert.AreEqual(Player.PlayerBlack, _model.Table[x, y]);
            Assert.AreEqual(Player.PlayerWhite, _model.Table.CurrentPlayer);
            Assert.AreEqual(0, _model.TimeWhite);
            Assert.AreEqual(0, _model.TimeBlack);
            Assert.AreEqual(1, _model.Table.ScoreWhite);
            Assert.AreEqual(4, _model.Table.ScoreBlack);

            x = 7; y = 9;
            _model.Step(x, y); // nem üt

            Assert.AreEqual(Player.NoPlayer, _model.Table[x, y]);
            Assert.AreEqual(Player.PlayerWhite, _model.Table.CurrentPlayer);
            Assert.AreEqual(0, _model.TimeWhite);
            Assert.AreEqual(0, _model.TimeBlack);
            Assert.AreEqual(1, _model.Table.ScoreWhite);
            Assert.AreEqual(4, _model.Table.ScoreBlack);
        }

        [TestMethod]
        public void ReversiGameModelAdvanceTimeTest()
        {
            _model.NewGame();

            int timeWhite = _model.TimeBlack;
            int timeBlack = _model.TimeBlack;

            while (timeBlack < 100)
            {
                _model.AdvanceTime();

                timeBlack++;

                Assert.AreEqual(timeBlack, _model.TimeBlack);
                Assert.AreEqual(0, _model.TimeWhite);
            }

            _model.Table.CurrentPlayer = Player.PlayerWhite;

            while (timeBlack < 100)
            {
                _model.AdvanceTime();

                timeWhite++;

                Assert.AreEqual(100, _model.TimeBlack);
                Assert.AreEqual(timeWhite, _model.TimeWhite);
            }
        }

        [TestMethod]
        public async Task ReversiGameModelLoadTest()
        {
            _model.NewGame();

            await _model.LoadGameAsync(String.Empty);

            for (int i = 0; i < _model.Table.Size; i++)
                for (int j = 0; j < _model.Table.Size; j++)
                {
                    Assert.AreEqual(_mockedTable.GetValue(i, j), _model.Table.GetValue(i, j));
                }

            _mock.Verify(dataAccess => dataAccess.LoadAsync(String.Empty), Times.Once());
        }
    }
}

