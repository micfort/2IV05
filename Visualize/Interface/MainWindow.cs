using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using micfort.GHL.Logging;

namespace CG_2IV05.Visualize.Interface
{
    public partial class MainWindow : Form
    {
        private Game game;

        public MainWindow()
        {
            InitializeComponent();
        }


        static void Main(string[] args)
        {
            micfort.GHL.GHLWindowsInit.Init();
            micfort.GHL.Logging.ErrorReporting.Instance.Engine = new TextWriterLoggingEngine(Console.Out);
            MainWindow window = new MainWindow();
            window.ShowDialog();
            
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            this.game = new Game();
            if (Site == null || !Site.DesignMode)
            {
                this.gameControl.initGame(game, settingsControl);
                this.settingsControl.InitLocations(game);
            }
        }
    }
}
