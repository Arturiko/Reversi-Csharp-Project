using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace OthelloUi
{
    public class program
    {
        public static void Main()
        {
            BoardGameForm form = new BoardGameForm();
            form.ShowDialog();
        }
    }
}
