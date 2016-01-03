using System;
using System.Collections.Generic;
using System.Text;

namespace OthelloLogic
{
    public class Player
    {
        public enum ePlayerPosibleSymbol
        {
            PlayerOne = 'X',
            PlayerTwo = 'O',
        }

        private string m_PlayerName;
        private int m_PlayerScore;
        private char m_SymbolOfPlayer;
        private int m_NumberOfCoins;

        public Player(string i_PlayerName, bool IsItFirstOrSecondPlayer)
        {
            m_PlayerScore = 0;
            m_PlayerName = i_PlayerName;
            m_NumberOfCoins = 2;
            if (IsItFirstOrSecondPlayer)
            {
                m_SymbolOfPlayer = (char)ePlayerPosibleSymbol.PlayerOne;
            }
            else
            {
                m_SymbolOfPlayer = (char)ePlayerPosibleSymbol.PlayerTwo;
            }
        }

        public char SymbolOfPlayer
        {
            get { return m_SymbolOfPlayer; }
            set { m_SymbolOfPlayer = value; }
        }

        public int NumberOfCoins
        {
            get { return m_NumberOfCoins; }
            set { m_NumberOfCoins = value; }
        }

        public string PlayerName
        {
            get { return m_PlayerName; }
            set { m_PlayerName = value; }
        }

        public int PlayerScore
        {
            get { return m_PlayerScore; }
            set { m_PlayerScore = value; }
        }
    }
}
