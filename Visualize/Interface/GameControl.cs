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
    public partial class GameControl : GLControl
    {
        private Game game;
        private SettingsControl settings;

        public GameControl()
        {
            InitializeComponent();
        }

        public void initGame(Game game, SettingsControl settings)
        {
            this.game = game;
            this.settings = settings;

            Application.Idle += GameControl_Idle;
            this.Resize += game.game_Resize;
            this.HandleDestroyed += GameControl_unload;
            this.Paint += GameControl_render;
            this.KeyDown += game.OnKeyDown;
            this.KeyUp += game.OnKeyUp;
            this.MouseDown += game.OnMouseDown;
            this.MouseUp += game.OnMouseUp;
            this.MouseMove += game.OnMouseMove;
            game.game_Load();
        }

        private void GameControl_Idle(object sender, EventArgs e)
        {
            game.processPressedKeys();
            settings.updateSettingsControl(game.CameraPos);
            this.Invalidate();
        }

        private void GameControl_render(object sender, PaintEventArgs e)
        {
            game.game_RenderFrame();
            this.SwapBuffers();
        }

        private void GameControl_unload(object sender, EventArgs e)
        {
            game.game_Unload();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Escape)
                Application.Exit();

            base.OnKeyDown(e);
        }
    }
}
