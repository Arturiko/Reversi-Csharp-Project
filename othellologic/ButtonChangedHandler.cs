using System;
using System.Collections.Generic;
using System.Text;

namespace OthelloLogic
{
    public class ButtonChangedHandler : EventArgs
    {
        private readonly string r_ButtonIndex;
        
        public ButtonChangedHandler(string i_ButtonIndex)
        {
            r_ButtonIndex = i_ButtonIndex;
        }

        public string ButtonIndex
        {
            get { return r_ButtonIndex; }
        }
    }
}
