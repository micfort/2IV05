using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CG_2IV05.Common.BAG;
using OpenTK;

namespace CG_2IV05.Visualize.Interface
{
    public partial class SettingsControl : UserControl
    {
        private Game game;
        private BoroughList boroughs;
       

        public SettingsControl()
        {
            InitializeComponent();
        }

        public void InitLocations(Game game)
        {
            this.game = game;
            boroughs = new BoroughList();
            boroughs.loadBoroughLocations(game.getDataCenter());
            boroughs.getProvinces().ForEach(x => this.provinceLB.Items.Add(x));
            this.provinceLB.SelectedIndex = 0;
        }

        public void updatePosition(Vector3 cameraPos)
        {
            LocationX.Text = cameraPos.X.ToString();
            LocationY.Text = cameraPos.Y.ToString();
            LocationZ.Text = cameraPos.Z.ToString();
            
            String nearestBorough = boroughs.findNearestBorough(cameraPos);
            currentLocationTB.Text = nearestBorough;
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
    }
}
