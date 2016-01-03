using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace OthelloUi
{
    public class GameSettingsForm : Form
    {
        private readonly Button r_BoardSizeButton = new Button();
        private readonly Button r_ComputerVsPlayerButton = new Button();
        private readonly Button r_PlayerVsPlayerButton = new Button();
        private int m_BoardSize = 6;
        private bool m_IsAgainstComputer;

        public GameSettingsForm()
        {
            initControls();
        }

        public int BoardSize
        {
            get { return m_BoardSize; }
        }

        public bool IsAgainstComputer
        {
            get { return m_IsAgainstComputer; }
        }

        private void onPlayerVSPlayerButtonPressed(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            m_IsAgainstComputer = false;
        }

        private void onPlayerVSComputerButtonPressed(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            m_IsAgainstComputer = true;
        }
        
        private void onButtonChangeSizeClick(object sender, EventArgs e)
        {
            m_BoardSize += 2;
            if (m_BoardSize <= 12)
            {
                r_BoardSizeButton.Text = string.Format("Board Size: {0} x {0} (click to increase)", m_BoardSize);
            }
            else
            {
                m_BoardSize = 6;
                r_BoardSizeButton.Text = string.Format("Board Size: {0} x {0} (click to increase)", m_BoardSize);
            }
        }

        private void initControls()
        {
            const int k_ButtonSizeHeight = 40;
            //// 
            //// Change Size Button
            //// 
            r_BoardSizeButton.Text = string.Format("Board Size: {0} x {0} (click to increase)", m_BoardSize);
            r_BoardSizeButton.Location = new Point(5, 10);
            r_BoardSizeButton.Font = new Font("Georgia", 10);
            r_BoardSizeButton.Height = k_ButtonSizeHeight;
            r_BoardSizeButton.Width = ClientSize.Width - (10 * 2);
            r_BoardSizeButton.MouseEnter += onMouseEnter;
            r_BoardSizeButton.MouseLeave += onMouseLeave;
            r_BoardSizeButton.Click += onButtonChangeSizeClick;
            this.Controls.Add(r_BoardSizeButton);
            //// 
            //// Computer VS Player button
            //// 
            r_ComputerVsPlayerButton.Text = string.Format("Play against the {0} computer", Environment.NewLine);
            r_ComputerVsPlayerButton.Location = new Point(r_BoardSizeButton.Left, r_BoardSizeButton.Top + 70);
            r_ComputerVsPlayerButton.Font = new Font("Georgia", 10);
            r_ComputerVsPlayerButton.Height = k_ButtonSizeHeight;
            r_ComputerVsPlayerButton.Width = r_BoardSizeButton.Width / 2;
            r_ComputerVsPlayerButton.MouseEnter += onMouseEnter;
            r_ComputerVsPlayerButton.MouseLeave += onMouseLeave;
            r_ComputerVsPlayerButton.Click += onPlayerVSComputerButtonPressed;
            
            this.Controls.Add(r_ComputerVsPlayerButton);
            //// 
            //// Player VS Player button
            //// 
            r_PlayerVsPlayerButton.Text = "Play against your friend";
            r_PlayerVsPlayerButton.Font = new Font("Georgia", 10);
            r_PlayerVsPlayerButton.Height = k_ButtonSizeHeight;
            r_PlayerVsPlayerButton.Width = r_BoardSizeButton.Width / 2;
            r_PlayerVsPlayerButton.Location = new Point(r_BoardSizeButton.Width - r_ComputerVsPlayerButton.Width + 5, r_BoardSizeButton.Top + 70);
            r_PlayerVsPlayerButton.MouseEnter += onMouseEnter;
            r_PlayerVsPlayerButton.MouseLeave += onMouseLeave;
            r_PlayerVsPlayerButton.Click += onPlayerVSPlayerButtonPressed;
            this.Controls.Add(r_PlayerVsPlayerButton);
            //// 
            //// Settings Form
            //// 

            this.AutoSize = true;
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Othello - Game Settings";
        }

        private void onMouseLeave(object sender, EventArgs e)
        {
            (sender as Button).BackColor = DefaultBackColor;
        }

        private void onMouseEnter(object sender, EventArgs e)
        {
            (sender as Button).BackColor = Color.Thistle;
        }
    }
}