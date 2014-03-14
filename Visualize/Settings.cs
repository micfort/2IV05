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
		private NodeManager _manager;
		public NodeManager manager
		{
			get { return _manager; }
			set
			{
				_manager = value;
				numericUpDown1.Value = Convert.ToDecimal(_manager.DistanceModifier);
			}
		}

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
