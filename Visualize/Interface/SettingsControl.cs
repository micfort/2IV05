using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using CG_2IV05.Common.BAG;
using OpenTK;
using SharpCompress.Reader;
using micfort.GHL.Math2;

namespace CG_2IV05.Visualize.Interface
{
    public partial class SettingsControl : UserControl
    {
        private Game game;
        private const string BOROUGH_FILENAME = "Boroughs.xml";

        private Dictionary<String, List<Borough>> boroughs;

        public SettingsControl()
        {
            InitializeComponent();
        }

        public void InitLocations(Game game)
        {
            this.game = game;
            boroughs = new Dictionary<string, List<Borough>>();
            loadBoroughLocations();
            boroughs.Keys.ToList().ForEach(x => this.provinceLB.Items.Add(x));
            this.provinceLB.SelectedIndex = 0;
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

        private void loadBoroughLocations()
        {
            Console.Out.WriteLine("Reading Boroughs file: {0}", BOROUGH_FILENAME);
			using (Stream stream = File.OpenRead(BOROUGH_FILENAME))
			{
				XmlReader reader = XmlReader.Create(stream);
				while (reader.Read())
				{
					if (reader.Name == "Borough")
					{
						Borough borough = Borough.loadBorough(reader);
						if (boroughs.ContainsKey(borough.Province))
						{
							boroughs[borough.Province].Add(borough);
						}
						else
						{
							boroughs.Add(borough.Province, new List<Borough>() { borough });
						}
					}
				}
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
                fillBoroughList(boroughs.Values.SelectMany(x => x));
            }
            else
            {
                String province = (string)provinceLB.SelectedItem;
                fillBoroughList(boroughs[province]);
            }
        }

        private void boroughSearchField_TextChanged(object sender, EventArgs e)
        {
            filterBoroughList();
        }

        private void filterBoroughList()
        {
            if (boroughSearchField.Text.Equals(""))
            {
                reloadBoroughList();
            }
            else if (provinceLB.SelectedIndex == 0)
            {
                IEnumerable<Borough> boroughList = new List<Borough>();
                foreach (List<Borough> province in boroughs.Values.ToList())
                {
                    var result = province.Where(b => b.Name.ToLower().Contains(boroughSearchField.Text.ToLower()));
                    boroughList = boroughList.Concat(result);
                }
                fillBoroughList(boroughList);
            }
            else if (provinceLB.SelectedIndex > 0)
            {
                String province = (string) provinceLB.SelectedItem;
                var boroughList = boroughs[province].Where(b => b.Name.ToLower().Contains(boroughSearchField.Text.ToLower()));
                fillBoroughList(boroughList);
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
