using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace OthelloLogic
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    
    public delegate void IsButtonChangedHandler(object sender, ButtonChangedHandler e);

    /// <summary>
    /// Class that makes all the calculation and logic of the game.
    /// </summary>
    public class GameLogics
    {
        private const char k_QuestionMark = '?';
        private const char k_EmptyCell = ' ';
        private static Random s_RandomMove;
        private readonly Player r_FirstPlayer;
        private readonly Player r_SecondPlayer;
        private readonly List<string> r_ComputerValidMoves;
        private OthelloBoard m_BoardGame;
        private bool m_IsItComputer;

        public event IsButtonChangedHandler UpdateingCellHandler;

        public GameLogics(int i_BoardSize)
        {
            m_IsItComputer = false;
            CreateBoardGame(i_BoardSize);
            r_FirstPlayer = new Player("Black", true);
            r_SecondPlayer = new Player("White", false);
            s_RandomMove = new Random();
            r_ComputerValidMoves = new List<string>();
        }

        private List<string> computerValidMoves
        {
            get { return r_ComputerValidMoves; }
        }

        public bool IsItComputer
        {
            get { return m_IsItComputer; }
            set { m_IsItComputer = value; }
        }

        private char questionMark
        {
            get { return k_QuestionMark; }
        }

        public Random RandomMove
        {
            get { return s_RandomMove; }
        }

        public Player FirstPlayer
        {
            get { return r_FirstPlayer; }
        }

        public Player SecondPlayer
        {
            get { return r_SecondPlayer; }
        }

        public char EmptyCell
        {
            get { return k_EmptyCell; }
        }

        public OthelloBoard BoardGame
        {
            get { return m_BoardGame; }
        }

        public void CreateBoardGame(int i_BoardSize)
        {
            m_BoardGame = new OthelloBoard(i_BoardSize);
            InitializationBoardCells();
        }

        /// <summary>
        /// This method is assigned to create the moves the current player can do (the grey cells).
        /// </summary>
        /// <param name="i_IsItPlayerOneTurn">A bool that holds on the player which has the turn. </param>
        public void GenerateValidTurns(bool i_IsItPlayerOneTurn)
        {
            RemoveQuestionMarkAfterMove();
            char currentPlayerTurnSymbol;
            char opponentPlayerSymbol;
            computerValidMoves.Clear();
            const bool v_FoundOpponent = true;
            if (i_IsItPlayerOneTurn)
            {
                currentPlayerTurnSymbol = FirstPlayer.SymbolOfPlayer;
                opponentPlayerSymbol = SecondPlayer.SymbolOfPlayer;
            }
            else
            {
                currentPlayerTurnSymbol = SecondPlayer.SymbolOfPlayer;
                opponentPlayerSymbol = FirstPlayer.SymbolOfPlayer;
            }

            for (int rowIndex = 0; rowIndex < BoardGame.BoardSize; rowIndex++)
            {
                for (int colIndex = 0; colIndex < BoardGame.BoardSize; colIndex++)
                {
                    if (BoardGame.BoardGameMatrix[rowIndex, colIndex] != ' ')
                    {
                        continue;
                    }

                    for (int i = -1; i <= 1; i++)
                    {
                        for (int j = -1; j <= 1; j++)
                        {
                            if (rowIndex + i < 0 || rowIndex + i >= BoardGame.BoardSize ||
                                colIndex + j < 0 || colIndex + j >= BoardGame.BoardSize ||
                                (i == 0 && j == 0))
                            {
                                continue;
                            }

                            if (BoardGame.BoardGameMatrix[rowIndex + i, colIndex + j] == opponentPlayerSymbol)
                            {
                                int rowIndexWhenSearching = rowIndex + i;
                                int colIndexWhenSearching = colIndex + j;
                                while (v_FoundOpponent)
                                {
                                    rowIndexWhenSearching += i;
                                    colIndexWhenSearching += j;

                                    if (rowIndexWhenSearching < 0 || rowIndexWhenSearching >= BoardGame.BoardSize || colIndexWhenSearching < 0 || colIndexWhenSearching >= BoardGame.BoardSize)
                                    {
                                        break;
                                    }

                                    if (BoardGame.BoardGameMatrix[rowIndexWhenSearching, colIndexWhenSearching] == EmptyCell)
                                    {
                                        break;
                                    }

                                    if (BoardGame.BoardGameMatrix[rowIndexWhenSearching, colIndexWhenSearching] ==
                                        currentPlayerTurnSymbol)
                                    {
                                        BoardGame.BoardGameMatrix[rowIndex, colIndex] = questionMark;
                                        if (UpdateingCellHandler != null)
                                        {
                                            StringBuilder coordinate = new StringBuilder();
                                            coordinate.Append(Convert.ToChar(colIndex + 65));
                                            coordinate.Append(Convert.ToString(rowIndex + 1));
                                            UpdateingCellHandler(this, new ButtonChangedHandler(coordinate.ToString()));
                                        }

                                        if (IsItComputer && !i_IsItPlayerOneTurn)
                                        {
                                            computerValidMoves.Add(string.Format("{0}" + "{1}", Convert.ToChar(colIndex + 65), rowIndex + 1));
                                        }

                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public void RemoveQuestionMarkAfterMove()
        {
            for (int i = 0; i < BoardGame.BoardSize; i++)
            {
                for (int j = 0; j < BoardGame.BoardSize; j++)
                {
                    if (BoardGame.BoardGameMatrix[i, j] == questionMark)
                    {
                        BoardGame.BoardGameMatrix[i, j] = EmptyCell;
                        if (UpdateingCellHandler != null)
                        {
                            StringBuilder coordinate = new StringBuilder();
                            coordinate.Append(Convert.ToChar(j + 65));
                            coordinate.Append(Convert.ToString(i + 1));
                            UpdateingCellHandler(this, new ButtonChangedHandler(coordinate.ToString()));
                        }
                    }
                }
            }
        }

        public void CountCoinesPerPlayer()
        {
            FirstPlayer.NumberOfCoins = 0;
            SecondPlayer.NumberOfCoins = 0;
            for (int i = 0; i < BoardGame.BoardSize; i++)
            {
                for (int j = 0; j < BoardGame.BoardSize; j++)
                {
                    if (BoardGame.BoardGameMatrix[i, j] == FirstPlayer.SymbolOfPlayer)
                    {
                        FirstPlayer.NumberOfCoins++;
                    }
                    else if (BoardGame.BoardGameMatrix[i, j] == SecondPlayer.SymbolOfPlayer)
                    {
                        SecondPlayer.NumberOfCoins++;
                    }
                }
            }
        }

        /// <summary>
        /// This method checks the board after player move and flips all the cells which should be changed 
        /// because of the player move. 
        /// </summary>
        /// <param name="i_PlayerMove">A string with the Coordinates of the cell that the player/computer choose.</param>
        /// <param name="i_IsItPlayerOneTurn">A bool that holds on the player which has the turn</param>
        public void FlipSymbolsAfterMoving(string i_PlayerMove, bool i_IsItPlayerOneTurn)
        {
            int chosenQuestionMarkIndexI;
            int chosenQuestionMarkIndexJ;
            GenerateIndexesFromPlayerMoves(i_PlayerMove, out chosenQuestionMarkIndexI, out chosenQuestionMarkIndexJ);
            char currentPlayerTurnSymbol;
            char opponentPlayerSymbol;
            const bool v_SearchingForOpponnet = true;
            if (i_IsItPlayerOneTurn)
            {
                opponentPlayerSymbol = SecondPlayer.SymbolOfPlayer;
                currentPlayerTurnSymbol = FirstPlayer.SymbolOfPlayer;
            }
            else
            {
                opponentPlayerSymbol = FirstPlayer.SymbolOfPlayer;
                currentPlayerTurnSymbol = SecondPlayer.SymbolOfPlayer;
            }

            BoardGame.BoardGameMatrix[chosenQuestionMarkIndexI, chosenQuestionMarkIndexJ] = currentPlayerTurnSymbol;
            if (UpdateingCellHandler != null)
            {
                UpdateingCellHandler(this, new ButtonChangedHandler(i_PlayerMove));
            }

            for (int rowIncrement = -1; rowIncrement <= 1; rowIncrement++)
            {
                for (int colIncrement = -1; colIncrement <= 1; colIncrement++)
                {
                    if (chosenQuestionMarkIndexI + rowIncrement < 0 || chosenQuestionMarkIndexI + rowIncrement >= BoardGame.BoardSize ||
                       chosenQuestionMarkIndexJ + colIncrement < 0 || chosenQuestionMarkIndexJ + colIncrement >= BoardGame.BoardSize ||
                                            (rowIncrement == 0 && colIncrement == 0))
                    {
                        continue;
                    }

                    if (BoardGame.BoardGameMatrix[chosenQuestionMarkIndexI + rowIncrement, chosenQuestionMarkIndexJ + colIncrement] == opponentPlayerSymbol)
                    {
                        int rowForSearch = chosenQuestionMarkIndexI + rowIncrement;
                        int colForSearch = chosenQuestionMarkIndexJ + colIncrement;

                        while (v_SearchingForOpponnet)
                        {
                            rowForSearch += rowIncrement;
                            colForSearch += colIncrement;

                            if (rowForSearch < 0 || rowForSearch >= BoardGame.BoardSize || colForSearch < 0 || colForSearch >= BoardGame.BoardSize)
                            {
                                break;
                            }

                            if (BoardGame.BoardGameMatrix[rowForSearch, colForSearch] == EmptyCell)
                            {
                                break;
                            }

                            if (BoardGame.BoardGameMatrix[rowForSearch, colForSearch] == currentPlayerTurnSymbol)
                            {
                                while (BoardGame.BoardGameMatrix[rowForSearch -= rowIncrement, colForSearch -= colIncrement] == opponentPlayerSymbol)
                                {
                                    BoardGame.BoardGameMatrix[rowForSearch, colForSearch] = currentPlayerTurnSymbol;
                                    if (UpdateingCellHandler != null)
                                    {
                                        StringBuilder coordinate = new StringBuilder();
                                        coordinate.Append(Convert.ToChar(colForSearch + 65));
                                        coordinate.Append(Convert.ToString(rowForSearch + 1));
                                        UpdateingCellHandler(this, new ButtonChangedHandler(coordinate.ToString()));
                                    }
                                }

                                break;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// A method that receives the string wich holds the Coordinates of the cell that the player/computer choose
        /// and changes it to 2 int's.
        /// </summary>
        /// <param name="i_PlayerMove">A string with the Coordinates of the cell that the player/computer choose.</param>
        /// <param name="io_ChosenQuestionMarkIndexI"></param>
        /// <param name="io_ChosenQuestionMarkIndexJ"></param>
        public void GenerateIndexesFromPlayerMoves(string i_PlayerMove, out int io_ChosenQuestionMarkIndexI, out int io_ChosenQuestionMarkIndexJ)
        {
            switch (i_PlayerMove.Length)
            {
                case 2:
                    {
                        io_ChosenQuestionMarkIndexI = i_PlayerMove[1] - '0' - 1;
                        io_ChosenQuestionMarkIndexJ = i_PlayerMove[0] - 65;
                        break;
                    }

                case 3:
                    {
                        StringBuilder number = new StringBuilder();
                        number.Append(i_PlayerMove[1]);
                        number.Append(i_PlayerMove[2]);
                        io_ChosenQuestionMarkIndexI = int.Parse(number.ToString()) - 1;
                        io_ChosenQuestionMarkIndexJ = i_PlayerMove[0] - 65;
                        break;
                    }

                default:
                    {
                        io_ChosenQuestionMarkIndexI = 0;
                        io_ChosenQuestionMarkIndexJ = 0;
                        break;
                    }
            }
        }

        /// <summary>
        /// A method that manages the movments of the player/computer.
        /// </summary>
        /// <param name="i_IsItPlayerOneTurn">A bool that holds on the player which has the turn</param>
        /// <param name="i_PlayerMove">A string with the Coordinates of the cell that the player/computer choose.</param>
        public void GamePlayControl(bool i_IsItPlayerOneTurn, string i_PlayerMove)
        {
            GenerateValidTurns(i_IsItPlayerOneTurn);
            FlipSymbolsAfterMoving(i_PlayerMove, i_IsItPlayerOneTurn);
            i_IsItPlayerOneTurn = !i_IsItPlayerOneTurn;
            GenerateValidTurns(i_IsItPlayerOneTurn);
        }

        public int CountQuestionMarks()
        {
            int questionMarkCounter = 0;
            for (int i = 0; i < BoardGame.BoardSize; i++)
            {
                for (int j = 0; j < BoardGame.BoardSize; j++)
                {
                    if (BoardGame.BoardGameMatrix[i, j] == questionMark)
                    {
                        questionMarkCounter++;
                    }
                }
            }

            return questionMarkCounter;
        }

        public void InitOfGameData()
        {
            FirstPlayer.NumberOfCoins = 2;
            SecondPlayer.NumberOfCoins = 2;
            InitializationBoardCells();
        }

        public string ComputerTurn(bool i_IsItPlayerOneTurn)
        {
            string checkingMoves;
            checkingMoves = aiThatChecksEdges();
            computerValidMoves.Clear();
            return checkingMoves;
        }

        private string aiThatChecksEdges()
        {
            List<string> edgesValidMoves = new List<string>();
            string returnedMove = null;
            bool isItEdge = false;
            foreach (string validMoves in computerValidMoves)
            {
                if (validMoves[0] == 'A' || validMoves[0] == Convert.ToChar(BoardGame.BoardSize + 64))
                {
                    edgesValidMoves.Add(validMoves);
                    isItEdge = true;
                }

                if (validMoves[1] == '1' || validMoves[1] == Convert.ToChar(BoardGame.BoardSize + '0'))
                {
                    edgesValidMoves.Add(validMoves);
                    isItEdge = true;
                }
            }

            if (isItEdge)
            {
                returnedMove = edgesValidMoves[RandomMove.Next(0, edgesValidMoves.Count)];
            }
            else
            {
                if (computerValidMoves.Count != 0)
                {
                    returnedMove = computerValidMoves[RandomMove.Next(0, computerValidMoves.Count)];
                }
            }

            return returnedMove;
        }

       /// <summary>
        /// A method which creates the the cells in the beginning of every game. 
        /// </summary>
        public void InitializationBoardCells()
        {
            for (int i = 0; i < BoardGame.BoardSize; i++)
            {
                for (int j = 0; j < BoardGame.BoardSize; j++)
                {
                    if (i == (BoardGame.BoardSize / 2) - 1 && j == (BoardGame.BoardSize / 2) - 1)
                    {
                        BoardGame.BoardGameMatrix[i, j] = (char)Player.ePlayerPosibleSymbol.PlayerTwo;
                    }
                    else if (i == (BoardGame.BoardSize / 2) && j == (BoardGame.BoardSize / 2))
                    {
                        BoardGame.BoardGameMatrix[i, j] = (char)Player.ePlayerPosibleSymbol.PlayerTwo;
                    }
                    else if (i == (BoardGame.BoardSize / 2) - 1 && j == BoardGame.BoardSize / 2)
                    { 
                        BoardGame.BoardGameMatrix[i, j] = (char)Player.ePlayerPosibleSymbol.PlayerOne;
                    }
                    else if (i == (BoardGame.BoardSize / 2) && (j == ((BoardGame.BoardSize - 1) / 2)))
                    {
                        BoardGame.BoardGameMatrix[i, j] = (char)Player.ePlayerPosibleSymbol.PlayerOne;
                    }
                    else
                    {
                        BoardGame.BoardGameMatrix[i, j] = EmptyCell;
                    }

                    if (UpdateingCellHandler != null)
                    {
                        StringBuilder coordinate = new StringBuilder();
                        coordinate.Append(Convert.ToChar(j + 65));
                        coordinate.Append(Convert.ToString(i + 1));
                        UpdateingCellHandler(this, new ButtonChangedHandler(coordinate.ToString()));
                    }
                }
            }
        }
    }
}
