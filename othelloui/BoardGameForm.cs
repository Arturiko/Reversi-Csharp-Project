using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Text;
using System.Windows.Forms;
using OthelloLogic;

namespace OthelloUi
{
    internal class BoardGameForm : Form
    {
        private const int k_ButtonSize = 37;
        private readonly GameSettingsForm r_GameSettings;
        private readonly int r_BoardSize;
        private readonly GameCellButton[,] r_BoardChipButton;
        private readonly GameLogics r_GameLogics;
        private readonly bool r_IsAgainstComputer;
        private bool m_IsItFirstPlayerTurn;

        public BoardGameForm()
        {
            r_GameSettings = new GameSettingsForm();

            if (r_GameSettings.ShowDialog() == DialogResult.OK)
            {
                r_BoardSize = r_GameSettings.BoardSize;
                r_GameLogics = new GameLogics(r_GameSettings.BoardSize);
                r_BoardChipButton = new GameCellButton[r_GameSettings.BoardSize, r_GameSettings.BoardSize];
                r_IsAgainstComputer = r_GameSettings.IsAgainstComputer;
               }
            else
            {
                Environment.Exit(0);
            }
        }

        private void startTheGame()
        {
            r_GameLogics.UpdateingCellHandler += changeGameBoardAfterMoves;
            m_IsItFirstPlayerTurn = true;
            initGameBoardBySize();
            r_GameLogics.IsItComputer = r_GameSettings.IsAgainstComputer;
            r_GameLogics.InitializationBoardCells();
            r_GameLogics.GenerateValidTurns(m_IsItFirstPlayerTurn);
        }
        
        protected override void OnLoad(EventArgs e)
        {
            this.AutoSize = true;
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.BackColor = Color.Gainsboro;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Text = "Othello";
            startTheGame();
        }

        public GameCellButton[,] BoardChipButton
        {
            get { return r_BoardChipButton; }
        }

        private void initGameBoardBySize()
        {
            int leftSize = 5;
            int topSize = 5;

            for (int i = 0; i < r_BoardSize; i++)
            {
                for (int j = 0; j < r_BoardSize; j++)
                {
                    GameCellButton newButton = new GameCellButton(j, i);
                    r_BoardChipButton[i, j] = newButton;
                    newButton.Size = new Size(k_ButtonSize, k_ButtonSize);
                    newButton.Location = new Point(leftSize, topSize);
                    newButton.BackColor = Color.Gainsboro;
                    newButton.Enabled = false;
                    newButton.Click += onGameCellButtonClicked;
                    Controls.Add(newButton);
                    leftSize += newButton.Width + 6;
                }

                leftSize = 5;
                topSize += 17 + 30;
            }
        }

        private void UIplayGameControl(string i_PlayerMove)
        {
            r_GameLogics.GamePlayControl(m_IsItFirstPlayerTurn, i_PlayerMove.ToString());
            m_IsItFirstPlayerTurn = !m_IsItFirstPlayerTurn;
            checkIfPlayerHasDoneOrGameOver();

            if (r_IsAgainstComputer && !m_IsItFirstPlayerTurn)
            {
                r_GameLogics.GenerateValidTurns(m_IsItFirstPlayerTurn);
                string computerMove = r_GameLogics.ComputerTurn(m_IsItFirstPlayerTurn);
                int x, y;
                r_GameLogics.GenerateIndexesFromPlayerMoves(computerMove, out x, out y);
                BoardChipButton[x, y].PerformClick();
            }
        }

        private void checkIfPlayerHasDoneOrGameOver()
        {
            int countValidMoves = r_GameLogics.CountQuestionMarks();

            if (countValidMoves == 0)
            {
                m_IsItFirstPlayerTurn = !m_IsItFirstPlayerTurn;
                r_GameLogics.GenerateValidTurns(m_IsItFirstPlayerTurn);
                countValidMoves = r_GameLogics.CountQuestionMarks();

                if (countValidMoves == 0)
                {
                    gameOver();
                }
                else
                {
                    MessageBox.Show("Player Have No Valid Moves. Player Changed");
                }
            }
        }

        private void gameOver()
        {
            StringBuilder gameWinnerMsg = new StringBuilder();
            r_GameLogics.CountCoinesPerPlayer();
            if (r_GameLogics.FirstPlayer.NumberOfCoins > r_GameLogics.SecondPlayer.NumberOfCoins)
            {
                gameWinnerMsg.AppendLine(string.Format("{0} wins!!", r_GameLogics.FirstPlayer.PlayerName));
                r_GameLogics.FirstPlayer.PlayerScore++;
            }
            else if (r_GameLogics.FirstPlayer.NumberOfCoins < r_GameLogics.SecondPlayer.NumberOfCoins)
            {
                gameWinnerMsg.AppendLine(string.Format("{0} wins!!", r_GameLogics.SecondPlayer.PlayerName));
                r_GameLogics.SecondPlayer.PlayerScore++;
            }
            else
            {
                gameWinnerMsg.AppendLine(string.Format("it's a Draw!!"));
                r_GameLogics.FirstPlayer.PlayerScore++;
                r_GameLogics.SecondPlayer.PlayerScore++;
            }

            gameWinnerMsg.AppendFormat(
@"({0}/{1}) ({2}/{3})
Would you like another round?",
                              r_GameLogics.FirstPlayer.NumberOfCoins,
                              r_GameLogics.SecondPlayer.NumberOfCoins,
                              r_GameLogics.FirstPlayer.PlayerScore,
                              r_GameLogics.SecondPlayer.PlayerScore);
            if (MessageBox.Show(
                gameWinnerMsg.ToString(),
                "Game Over",
                MessageBoxButtons.YesNo,
                        MessageBoxIcon.Information) == DialogResult.Yes)
            {
                initGameBoard();
            }
            else
            {
                Environment.Exit(0);
            }
        }

        private void initGameBoard()
        {
            r_GameLogics.InitOfGameData();
            r_GameLogics.GenerateValidTurns(true);
            Controls.Clear();
            startTheGame();
        }

        private void onGameCellButtonClicked(object sender, EventArgs e)
        {
            GameCellButton currentButton = sender as GameCellButton;
            StringBuilder playerMove = new StringBuilder();

            playerMove.Append(Convert.ToChar(currentButton.I + 65));
            playerMove.Append((currentButton.J + 1).ToString());
            UIplayGameControl(playerMove.ToString());
        }

        private void changeGameBoardAfterMoves(object sender, ButtonChangedHandler e)
        {
            int i;
            int j;
            r_GameLogics.GenerateIndexesFromPlayerMoves(e.ButtonIndex, out i, out j);
            switch (r_GameLogics.BoardGame.BoardGameMatrix[i, j])
            {
                case 'X':
                    {
                        BoardChipButton[i, j].BackColor = Color.Black;
                        BoardChipButton[i, j].Text = "O";
                        BoardChipButton[i, j].ForeColor = Color.White;
                        BoardChipButton[i, j].Enabled = true;
                        BoardChipButton[i, j].Click -= onGameCellButtonClicked;
                        break;
                    }

                case 'O':
                    {
                        BoardChipButton[i, j].BackColor = Color.White;
                        BoardChipButton[i, j].Text = "O";
                        BoardChipButton[i, j].ForeColor = Color.Black;
                        BoardChipButton[i, j].Enabled = true;
                        BoardChipButton[i, j].Click -= onGameCellButtonClicked;
                        break;
                    }

                case '?':
                    {
                        BoardChipButton[i, j].BackColor = Color.DimGray;
                        BoardChipButton[i, j].Enabled = true;

                        break;
                    }

                default:
                    {
                        BoardChipButton[i, j].BackColor = DefaultBackColor;
                        BoardChipButton[i, j].Enabled = false;
                        break;
                    }
            }
        }
    }
}
