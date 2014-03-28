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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.label6 = new System.Windows.Forms.Label();
            this.LocationZ = new System.Windows.Forms.TextBox();
            this.LocationY = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.LocationX = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.ErrorBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.maxErrorControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ErrorControl)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // ErrorBox
            // 
            this.ErrorBox.AutoSize = true;
            this.ErrorBox.Controls.Add(this.maxErrorControl);
            this.ErrorBox.Controls.Add(this.label9);
            this.ErrorBox.Controls.Add(this.ErrorControl);
            this.ErrorBox.Controls.Add(this.label1);
            this.ErrorBox.Location = new System.Drawing.Point(3, 3);
            this.ErrorBox.Name = "ErrorBox";
            this.ErrorBox.Size = new System.Drawing.Size(200, 89);
            this.ErrorBox.TabIndex = 0;
            this.ErrorBox.TabStop = false;
            this.ErrorBox.Text = "Error";
            // 
            // maxErrorControl
            // 
            this.maxErrorControl.Location = new System.Drawing.Point(69, 50);
            this.maxErrorControl.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.maxErrorControl.Name = "maxErrorControl";
            this.maxErrorControl.Size = new System.Drawing.Size(120, 20);
            this.maxErrorControl.TabIndex = 3;
            this.maxErrorControl.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.maxErrorControl.Value = new decimal(new int[] {
            1000,
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
            1000,
            0,
            0,
            0});
            this.ErrorControl.Name = "ErrorControl";
            this.ErrorControl.Size = new System.Drawing.Size(120, 20);
            this.ErrorControl.TabIndex = 1;
            this.ErrorControl.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.ErrorControl.Value = new decimal(new int[] {
            10,
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
            // groupBox1
            // 
            this.groupBox1.AutoSize = true;
            this.groupBox1.Controls.Add(this.button2);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.textBox1);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.listBox1);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.LocationZ);
            this.groupBox1.Controls.Add(this.LocationY);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.LocationX);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(3, 89);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 429);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Location";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(7, 355);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(146, 23);
            this.button2.TabIndex = 13;
            this.button2.Text = "Go To Location";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(56, 182);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 12;
            this.button1.Text = "Search";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(3, 211);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(53, 13);
            this.label8.TabIndex = 11;
            this.label8.Text = "Locations";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(57, 160);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 20);
            this.textBox1.TabIndex = 10;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(7, 160);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(44, 13);
            this.label7.TabIndex = 9;
            this.label7.Text = "Search:";
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(6, 227);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(147, 121);
            this.listBox1.TabIndex = 8;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(7, 144);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(84, 13);
            this.label6.TabIndex = 7;
            this.label6.Text = "Select Location:";
            // 
            // LocationZ
            // 
            this.LocationZ.Enabled = false;
            this.LocationZ.Location = new System.Drawing.Point(33, 98);
            this.LocationZ.Name = "LocationZ";
            this.LocationZ.Size = new System.Drawing.Size(100, 20);
            this.LocationZ.TabIndex = 6;
            // 
            // LocationY
            // 
            this.LocationY.Enabled = false;
            this.LocationY.Location = new System.Drawing.Point(33, 72);
            this.LocationY.Name = "LocationY";
            this.LocationY.Size = new System.Drawing.Size(100, 20);
            this.LocationY.TabIndex = 5;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(7, 98);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(17, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Z:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 44);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(20, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "X: ";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 72);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(17, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Y:";
            // 
            // LocationX
            // 
            this.LocationX.Enabled = false;
            this.LocationX.Location = new System.Drawing.Point(33, 44);
            this.LocationX.Name = "LocationX";
            this.LocationX.Size = new System.Drawing.Size(100, 20);
            this.LocationX.TabIndex = 1;
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
            this.Size = new System.Drawing.Size(206, 521);
            this.Load += new System.EventHandler(this.SettingsControl_Load);
            this.ErrorBox.ResumeLayout(false);
            this.ErrorBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.maxErrorControl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ErrorControl)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox ErrorBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown ErrorControl;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox LocationX;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.NumericUpDown maxErrorControl;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox LocationZ;
        private System.Windows.Forms.TextBox LocationY;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
    }
}
