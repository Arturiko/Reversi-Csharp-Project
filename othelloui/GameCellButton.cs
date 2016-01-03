using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace OthelloUi
{
    public class GameCellButton : Button
    {
        private readonly int r_i;
        private readonly int r_j;

        internal GameCellButton()
        {
        }

        internal GameCellButton(int i_I, int i_J)
        {
            r_i = i_I;
            r_j = i_J;
        }

        public int I
        {
            get { return r_i; }
        }

        public int J
        {
            get { return r_j; }
        }
    }
}
