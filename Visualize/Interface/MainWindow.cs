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

        private void MainWindow_Load(object sender, EventArgs e)
        {
            try
            {
                ErrorReporting.Instance.ReportInfo(this, "starting game");
                this.game = new Game();
                if (Site == null || !Site.DesignMode)
                {
                    this.settingsControl.game = game;
                    ErrorReporting.Instance.ReportInfo(this, "init game control");
                    this.gameControl.initGame(game, settingsControl);
                    ErrorReporting.Instance.ReportInfo(this, "init settings control");
                    this.settingsControl.InitLocations();
                    ErrorReporting.Instance.ReportInfo(this, "finished init controls");
                }
            }
            catch (Exception exception)
            {
                ErrorReporting.Instance.ReportFatalT(this, "oeps", exception);
            }
        }
    }
}
