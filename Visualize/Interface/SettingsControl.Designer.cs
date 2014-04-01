using OpenTK;

namespace CG_2IV05.Visualize.Interface
{
    partial class SettingsControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ErrorBox = new System.Windows.Forms.GroupBox();
            this.maxErrorControl = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.ErrorControl = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.labelSearch = new System.Windows.Forms.Label();
            this.boroughSearchField = new System.Windows.Forms.TextBox();
            this.labelProvince = new System.Windows.Forms.Label();
            this.locationButton = new System.Windows.Forms.Button();
            this.boroughLB = new System.Windows.Forms.ListBox();
            this.labelBorough = new System.Windows.Forms.Label();
            this.provinceLB = new System.Windows.Forms.ListBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.labelX = new System.Windows.Forms.Label();
            this.labelY = new System.Windows.Forms.Label();
            this.labelZ = new System.Windows.Forms.Label();
            this.LocationX = new System.Windows.Forms.TextBox();
            this.LocationY = new System.Windows.Forms.TextBox();
            this.LocationZ = new System.Windows.Forms.TextBox();
            this.currentLocationTB = new System.Windows.Forms.TextBox();
            this.labelCurrent = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioButtonTopDown = new System.Windows.Forms.RadioButton();
            this.radioButtonWalking = new System.Windows.Forms.RadioButton();
            this.radioButtonRoaming = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.ErrorBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.maxErrorControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ErrorControl)).BeginInit();
            this.groupBox4.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // ErrorBox
            // 
            this.ErrorBox.Controls.Add(this.maxErrorControl);
            this.ErrorBox.Controls.Add(this.label9);
            this.ErrorBox.Controls.Add(this.ErrorControl);
            this.ErrorBox.Controls.Add(this.label1);
            this.ErrorBox.Location = new System.Drawing.Point(10, 3);
            this.ErrorBox.Name = "ErrorBox";
            this.ErrorBox.Size = new System.Drawing.Size(286, 89);
            this.ErrorBox.TabIndex = 0;
            this.ErrorBox.TabStop = false;
            this.ErrorBox.Text = "Error";
            // 
            // maxErrorControl
            // 
            this.maxErrorControl.Location = new System.Drawing.Point(69, 50);
            this.maxErrorControl.Maximum = new decimal(new int[] {
            1215752192,
            23,
            0,
            0});
            this.maxErrorControl.Name = "maxErrorControl";
            this.maxErrorControl.Size = new System.Drawing.Size(100, 20);
            this.maxErrorControl.TabIndex = 3;
            this.maxErrorControl.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.maxErrorControl.Value = new decimal(new int[] {
            10000000,
            0,
            0,
            0});
            this.maxErrorControl.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(7, 50);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(52, 13);
            this.label9.TabIndex = 2;
            this.label9.Text = "Max Error";
            // 
            // ErrorControl
            // 
            this.ErrorControl.Location = new System.Drawing.Point(69, 20);
            this.ErrorControl.Maximum = new decimal(new int[] {
            -727379968,
            232,
            0,
            0});
            this.ErrorControl.Name = "ErrorControl";
            this.ErrorControl.Size = new System.Drawing.Size(100, 20);
            this.ErrorControl.TabIndex = 1;
            this.ErrorControl.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.ErrorControl.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.ErrorControl.ValueChanged += new System.EventHandler(this.ErrorControl_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Error";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.labelSearch);
            this.groupBox4.Controls.Add(this.boroughSearchField);
            this.groupBox4.Controls.Add(this.labelProvince);
            this.groupBox4.Controls.Add(this.locationButton);
            this.groupBox4.Controls.Add(this.boroughLB);
            this.groupBox4.Controls.Add(this.labelBorough);
            this.groupBox4.Controls.Add(this.provinceLB);
            this.groupBox4.Location = new System.Drawing.Point(6, 199);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(293, 328);
            this.groupBox4.TabIndex = 21;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Select Location";
            // 
            // labelSearch
            // 
            this.labelSearch.AutoSize = true;
            this.labelSearch.Location = new System.Drawing.Point(6, 23);
            this.labelSearch.Name = "labelSearch";
            this.labelSearch.Size = new System.Drawing.Size(44, 13);
            this.labelSearch.TabIndex = 9;
            this.labelSearch.Text = "Search:";
            // 
            // boroughSearchField
            // 
            this.boroughSearchField.Location = new System.Drawing.Point(9, 39);
            this.boroughSearchField.Name = "boroughSearchField";
            this.boroughSearchField.Size = new System.Drawing.Size(127, 20);
            this.boroughSearchField.TabIndex = 10;
            this.boroughSearchField.TextChanged += new System.EventHandler(this.boroughSearchField_TextChanged);
            // 
            // labelProvince
            // 
            this.labelProvince.AutoSize = true;
            this.labelProvince.Location = new System.Drawing.Point(9, 66);
            this.labelProvince.Name = "labelProvince";
            this.labelProvince.Size = new System.Drawing.Size(57, 13);
            this.labelProvince.TabIndex = 15;
            this.labelProvince.Text = "Provinces:";
            // 
            // locationButton
            // 
            this.locationButton.Location = new System.Drawing.Point(142, 274);
            this.locationButton.Name = "locationButton";
            this.locationButton.Size = new System.Drawing.Size(141, 23);
            this.locationButton.TabIndex = 13;
            this.locationButton.Text = "Go To Location";
            this.locationButton.UseVisualStyleBackColor = true;
            // 
            // boroughLB
            // 
            this.boroughLB.FormattingEnabled = true;
            this.boroughLB.Location = new System.Drawing.Point(142, 82);
            this.boroughLB.Name = "boroughLB";
            this.boroughLB.Size = new System.Drawing.Size(141, 186);
            this.boroughLB.TabIndex = 14;
            // 
            // labelBorough
            // 
            this.labelBorough.AutoSize = true;
            this.labelBorough.Location = new System.Drawing.Point(142, 66);
            this.labelBorough.Name = "labelBorough";
            this.labelBorough.Size = new System.Drawing.Size(55, 13);
            this.labelBorough.TabIndex = 16;
            this.labelBorough.Text = "Boroughs:";
            // 
            // provinceLB
            // 
            this.provinceLB.FormattingEnabled = true;
            this.provinceLB.Items.AddRange(new object[] {
            "All Provinces"});
            this.provinceLB.Location = new System.Drawing.Point(9, 82);
            this.provinceLB.Name = "provinceLB";
            this.provinceLB.Size = new System.Drawing.Size(127, 186);
            this.provinceLB.TabIndex = 8;
            this.provinceLB.SelectedIndexChanged += new System.EventHandler(this.ProvinceLB_SelectedIndexChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.AutoSize = true;
            this.groupBox1.Controls.Add(this.groupBox4);
            this.groupBox1.Controls.Add(this.groupBox3);
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(10, 94);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(305, 547);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Location";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.labelX);
            this.groupBox3.Controls.Add(this.labelY);
            this.groupBox3.Controls.Add(this.labelZ);
            this.groupBox3.Controls.Add(this.LocationX);
            this.groupBox3.Controls.Add(this.LocationY);
            this.groupBox3.Controls.Add(this.LocationZ);
            this.groupBox3.Controls.Add(this.currentLocationTB);
            this.groupBox3.Controls.Add(this.labelCurrent);
            this.groupBox3.Location = new System.Drawing.Point(6, 20);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(154, 173);
            this.groupBox3.TabIndex = 20;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Location";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 60);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(102, 13);
            this.label3.TabIndex = 19;
            this.label3.Text = "Camera Coordinates";
            // 
            // labelX
            // 
            this.labelX.AutoSize = true;
            this.labelX.Location = new System.Drawing.Point(7, 80);
            this.labelX.Name = "labelX";
            this.labelX.Size = new System.Drawing.Size(20, 13);
            this.labelX.TabIndex = 3;
            this.labelX.Text = "X: ";
            // 
            // labelY
            // 
            this.labelY.AutoSize = true;
            this.labelY.Location = new System.Drawing.Point(7, 108);
            this.labelY.Name = "labelY";
            this.labelY.Size = new System.Drawing.Size(17, 13);
            this.labelY.TabIndex = 2;
            this.labelY.Text = "Y:";
            // 
            // labelZ
            // 
            this.labelZ.AutoSize = true;
            this.labelZ.Location = new System.Drawing.Point(7, 134);
            this.labelZ.Name = "labelZ";
            this.labelZ.Size = new System.Drawing.Size(17, 13);
            this.labelZ.TabIndex = 4;
            this.labelZ.Text = "Z:";
            // 
            // LocationX
            // 
            this.LocationX.Enabled = false;
            this.LocationX.Location = new System.Drawing.Point(33, 80);
            this.LocationX.Name = "LocationX";
            this.LocationX.Size = new System.Drawing.Size(100, 20);
            this.LocationX.TabIndex = 1;
            this.LocationX.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // LocationY
            // 
            this.LocationY.Enabled = false;
            this.LocationY.Location = new System.Drawing.Point(33, 108);
            this.LocationY.Name = "LocationY";
            this.LocationY.Size = new System.Drawing.Size(100, 20);
            this.LocationY.TabIndex = 5;
            this.LocationY.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // LocationZ
            // 
            this.LocationZ.Enabled = false;
            this.LocationZ.Location = new System.Drawing.Point(33, 134);
            this.LocationZ.Name = "LocationZ";
            this.LocationZ.Size = new System.Drawing.Size(100, 20);
            this.LocationZ.TabIndex = 6;
            this.LocationZ.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // currentLocationTB
            // 
            this.currentLocationTB.Enabled = false;
            this.currentLocationTB.Location = new System.Drawing.Point(10, 32);
            this.currentLocationTB.Name = "currentLocationTB";
            this.currentLocationTB.Size = new System.Drawing.Size(123, 20);
            this.currentLocationTB.TabIndex = 18;
            this.currentLocationTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // labelCurrent
            // 
            this.labelCurrent.AutoSize = true;
            this.labelCurrent.Location = new System.Drawing.Point(7, 16);
            this.labelCurrent.Name = "labelCurrent";
            this.labelCurrent.Size = new System.Drawing.Size(85, 13);
            this.labelCurrent.TabIndex = 17;
            this.labelCurrent.Text = "Current Location";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.radioButtonTopDown);
            this.groupBox2.Controls.Add(this.radioButtonWalking);
            this.groupBox2.Controls.Add(this.radioButtonRoaming);
            this.groupBox2.Location = new System.Drawing.Point(166, 20);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(110, 173);
            this.groupBox2.TabIndex = 19;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "View Mode";
            // 
            // radioButtonTopDown
            // 
            this.radioButtonTopDown.AutoSize = true;
            this.radioButtonTopDown.Location = new System.Drawing.Point(6, 70);
            this.radioButtonTopDown.Name = "radioButtonTopDown";
            this.radioButtonTopDown.Size = new System.Drawing.Size(75, 17);
            this.radioButtonTopDown.TabIndex = 2;
            this.radioButtonTopDown.Text = "Top Down";
            this.radioButtonTopDown.UseVisualStyleBackColor = true;
            this.radioButtonTopDown.CheckedChanged += new System.EventHandler(this.radio_CheckedChanged);
            // 
            // radioButtonWalking
            // 
            this.radioButtonWalking.AutoSize = true;
            this.radioButtonWalking.Location = new System.Drawing.Point(6, 47);
            this.radioButtonWalking.Name = "radioButtonWalking";
            this.radioButtonWalking.Size = new System.Drawing.Size(64, 17);
            this.radioButtonWalking.TabIndex = 1;
            this.radioButtonWalking.Text = "Walking";
            this.radioButtonWalking.UseVisualStyleBackColor = true;
            this.radioButtonWalking.CheckedChanged += new System.EventHandler(this.radio_CheckedChanged);
            // 
            // radioButtonRoaming
            // 
            this.radioButtonRoaming.AutoSize = true;
            this.radioButtonRoaming.Checked = true;
            this.radioButtonRoaming.Location = new System.Drawing.Point(6, 24);
            this.radioButtonRoaming.Name = "radioButtonRoaming";
            this.radioButtonRoaming.Size = new System.Drawing.Size(67, 17);
            this.radioButtonRoaming.TabIndex = 0;
            this.radioButtonRoaming.TabStop = true;
            this.radioButtonRoaming.Text = "Roaming";
            this.radioButtonRoaming.UseVisualStyleBackColor = true;
            this.radioButtonRoaming.CheckedChanged += new System.EventHandler(this.radio_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(81, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Current Position";
            // 
            // SettingsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.ErrorBox);
            this.Name = "SettingsControl";
            this.Size = new System.Drawing.Size(318, 644);
            this.ErrorBox.ResumeLayout(false);
            this.ErrorBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.maxErrorControl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ErrorControl)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox ErrorBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown ErrorControl;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox boroughSearchField;
        private System.Windows.Forms.Label labelSearch;
        private System.Windows.Forms.ListBox provinceLB;
        private System.Windows.Forms.TextBox LocationX;
        private System.Windows.Forms.Button locationButton;
        private System.Windows.Forms.NumericUpDown maxErrorControl;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox LocationZ;
        private System.Windows.Forms.TextBox LocationY;
        private System.Windows.Forms.Label labelZ;
        private System.Windows.Forms.Label labelX;
        private System.Windows.Forms.Label labelY;
        private System.Windows.Forms.ListBox boroughLB;
        private System.Windows.Forms.Label labelBorough;
        private System.Windows.Forms.Label labelProvince;
        private System.Windows.Forms.TextBox currentLocationTB;
        private System.Windows.Forms.Label labelCurrent;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton radioButtonTopDown;
        private System.Windows.Forms.RadioButton radioButtonWalking;
        private System.Windows.Forms.RadioButton radioButtonRoaming;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox4;
    }
}
