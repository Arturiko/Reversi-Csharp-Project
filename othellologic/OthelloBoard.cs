using System;
using System.Collections.Generic;
using System.Text;

namespace OthelloLogic
{
    public class OthelloBoard
    {
        private readonly int r_BoardSize;
        private readonly char[,] r_BoardgameMatrix;

        public OthelloBoard(int i_BoardSize)
        {
            r_BoardSize = i_BoardSize;
            r_BoardgameMatrix = new char[BoardSize, BoardSize];
        }

        public int BoardSize
        {
            get { return r_BoardSize; }
        }

        public char[,] BoardGameMatrix
        {
            get { return r_BoardgameMatrix; }
        }
    }
}
