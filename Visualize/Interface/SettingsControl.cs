using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenTK;

namespace CG_2IV05.Visualize.Interface
{
    public partial class SettingsControl : UserControl
    {
        private Game game;
        public SettingsControl(Game game)
        {
            this.game = game;
            InitializeComponent();
        }

        private void SettingsControl_Load(object sender, EventArgs e)
        {
        }

        public void updatePosition(Vector3 cameraPos)
        {
            LocationX.Text = cameraPos.X.ToString();
            LocationY.Text = cameraPos.Y.ToString();
            LocationZ.Text = cameraPos.Z.ToString();
        }

        private void ErrorControl_ValueChanged(object sender, EventArgs e)
        {
            if(game.manager != null)
            {
                game.manager.DistanceModifier = float.Parse(ErrorControl.Text);
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (game.manager != null)
            {
                game.manager.DistanceModifier = float.Parse(maxErrorControl.Text);
            }
        }
    }
}
