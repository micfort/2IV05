using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CG_2IV05.Common.BAG;
using OpenTK;
using micfort.GHL.Math2;

namespace CG_2IV05.Visualize.Interface
{
    public partial class SettingsControl : UserControl
    {
        public Game game { get; set; }
        private BoroughList boroughs;
       

        public SettingsControl()
        {
            InitializeComponent();
        }

        public void InitLocations()
        {
            boroughs = new BoroughList();
            boroughs.loadBoroughLocations(game.getDataCenter());
            boroughs.getProvinces().ForEach(x => this.provinceLB.Items.Add(x));
            this.provinceLB.SelectedIndex = 0;
        }

        public void updateSettingsControl(Vector3 cameraPos)
        {
            LocationX.Text = (cameraPos.X + game.Tree.centerData.Value.X).ToString();
            LocationY.Text = (cameraPos.Y + game.Tree.centerData.Value.Y).ToString();
            LocationZ.Text = cameraPos.Z.ToString();

	        String nearestBorough = boroughs.findNearestBorough(cameraPos);
            currentLocationTB.Text = nearestBorough;

            Game.ViewMode curMode = game.getCurrentViewMode();
            switch (curMode)
            {
                case Game.ViewMode.Roaming:
                    radioButtonRoaming.Checked = true;
                    break;
                case Game.ViewMode.Walking:
                    radioButtonWalking.Checked = true;
                    break;
                case Game.ViewMode.TopDown:
                    radioButtonTopDown.Checked = true;
                    break;
            }
        }

        private void ErrorControl_ValueChanged(object sender, EventArgs e)
        {
            if(game.manager != null)
            {
                game.manager.DistanceModifier = Convert.ToSingle(ErrorControl.Value);
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (game.manager != null)
            {
				game.manager.MaxDistanceError = Convert.ToSingle(maxErrorControl.Value);
            }
        }

        

        private void ProvinceLB_SelectedIndexChanged(object sender, EventArgs e)
        {
            reloadBoroughList();
            boroughSearchField.Clear();
        }

        private void reloadBoroughList()
        {
            if(provinceLB.SelectedIndex == 0)
            {
                fillBoroughList(boroughs.getAllBoroughs());
            }
            else
            {
                String province = (string)provinceLB.SelectedItem;
                fillBoroughList(boroughs.getBoroughsByProvince(province));
            }
        }

        private void boroughSearchField_TextChanged(object sender, EventArgs e)
        {
            if (boroughSearchField.Text.Equals(""))
            {
                reloadBoroughList();
            }
            else if (provinceLB.SelectedIndex == 0)
            {
                fillBoroughList(boroughs.searchBoroughs(boroughSearchField.Text));
            }
            else if (provinceLB.SelectedIndex > 0)
            {
                String province = (string)provinceLB.SelectedItem;
                fillBoroughList(boroughs.searchBoroughs(boroughSearchField.Text, province));
            }
        }

        public void fillBoroughList(IEnumerable<Borough> boroughs)
        {
            this.boroughLB.Items.Clear();
            foreach (var borough in boroughs)
            {
                boroughLB.Items.Add(borough);
            }
            if (boroughLB.Items.Count > 0)
            {
                boroughLB.SelectedIndex = 0;
            }
        }

        private void radio_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            if (rb != null)
            {
                if (rb.Checked)
                {
                    if (rb.Text.Equals(radioButtonRoaming.Text))
                    {
                        game.SetViewMode(Game.ViewMode.Roaming);
                    }
                    else if (rb.Text.Equals(radioButtonWalking.Text))
                    {
                        game.SetViewMode(Game.ViewMode.Walking);
                    }
                    else if (rb.Text.Equals(radioButtonTopDown.Text))
                    {
                        game.SetViewMode(Game.ViewMode.TopDown);
                    }
                }
            }          
        }

        private void locationButton_Click(object sender, EventArgs e)
        {
            goToSelectedLocation();
        }

        private void boroughSearchField_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                goToSelectedLocation();
            }
        }

        private void goToSelectedLocation()
        {
            if (boroughLB.SelectedItem != null)
            {
                String newLocation = ((Borough)boroughLB.SelectedItem).Name;
                HyperPoint<float> goToPoint = boroughs.GoToLocation(newLocation);
                game.GoToPoint(goToPoint);
            }
        }
    }
}
