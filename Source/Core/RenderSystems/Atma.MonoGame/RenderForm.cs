using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Atma.Graphics;

namespace Atma.MonoGame
{
    public partial class RenderForm : Form
    {
        private RenderWindow _renderWindow;
        
        public RenderForm(RenderWindow window)
        {
            _renderWindow = window;
            InitializeComponent();
        }

        protected override void WndProc(ref Message m)
        {
            if (!MGWin32Platform.WndProc(_renderWindow, ref m))
            {
                base.WndProc(ref m);
            }
        }
    }
}
