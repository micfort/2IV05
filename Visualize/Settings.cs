using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CG_2IV05.Visualize
{
	public partial class Settings : Form
	{
		public NodeManager manager { get; set; }

		public Settings()
		{
			InitializeComponent();
		}

		private void numericUpDown1_ValueChanged(object sender, EventArgs e)
		{
			if(manager != null)
			{
				manager.DistanceModifier = Convert.ToSingle(numericUpDown1.Value);
			}
		}
	}
}
