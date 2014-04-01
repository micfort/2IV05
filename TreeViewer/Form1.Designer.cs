namespace TreeViewer
{
	partial class Form1
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.btnOpen = new System.Windows.Forms.Button();
			this.tbInfo = new System.Windows.Forms.TextBox();
			this.btnParent = new System.Windows.Forms.Button();
			this.btnChild = new System.Windows.Forms.Button();
			this.cbChilds = new System.Windows.Forms.ComboBox();
			this.tbPosition = new System.Windows.Forms.TextBox();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.panel1 = new System.Windows.Forms.Panel();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.label5 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.tbErrorPerMeter = new System.Windows.Forms.TextBox();
			this.tbMaxError = new System.Windows.Forms.TextBox();
			this.btnUpdateLoadedItems = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.tbPosY = new System.Windows.Forms.TextBox();
			this.tbPosZ = new System.Windows.Forms.TextBox();
			this.tbPosX = new System.Windows.Forms.TextBox();
			this.btnChild0 = new System.Windows.Forms.Button();
			this.btnChild1 = new System.Windows.Forms.Button();
			this.btnSaveImage = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.panel1.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnOpen
			// 
			this.btnOpen.Location = new System.Drawing.Point(12, 12);
			this.btnOpen.Name = "btnOpen";
			this.btnOpen.Size = new System.Drawing.Size(75, 23);
			this.btnOpen.TabIndex = 0;
			this.btnOpen.Text = "Open";
			this.btnOpen.UseVisualStyleBackColor = true;
			this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
			// 
			// tbInfo
			// 
			this.tbInfo.Enabled = false;
			this.tbInfo.Location = new System.Drawing.Point(12, 152);
			this.tbInfo.Multiline = true;
			this.tbInfo.Name = "tbInfo";
			this.tbInfo.Size = new System.Drawing.Size(202, 192);
			this.tbInfo.TabIndex = 1;
			// 
			// btnParent
			// 
			this.btnParent.Location = new System.Drawing.Point(12, 67);
			this.btnParent.Name = "btnParent";
			this.btnParent.Size = new System.Drawing.Size(75, 23);
			this.btnParent.TabIndex = 2;
			this.btnParent.Text = "Go to Parent";
			this.btnParent.UseVisualStyleBackColor = true;
			this.btnParent.Click += new System.EventHandler(this.btnParent_Click);
			// 
			// btnChild
			// 
			this.btnChild.Location = new System.Drawing.Point(139, 123);
			this.btnChild.Name = "btnChild";
			this.btnChild.Size = new System.Drawing.Size(75, 23);
			this.btnChild.TabIndex = 4;
			this.btnChild.Text = "Go To child";
			this.btnChild.UseVisualStyleBackColor = true;
			this.btnChild.Click += new System.EventHandler(this.btnChild_Click);
			// 
			// cbChilds
			// 
			this.cbChilds.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbChilds.FormattingEnabled = true;
			this.cbChilds.Location = new System.Drawing.Point(12, 125);
			this.cbChilds.Name = "cbChilds";
			this.cbChilds.Size = new System.Drawing.Size(121, 21);
			this.cbChilds.TabIndex = 5;
			// 
			// tbPosition
			// 
			this.tbPosition.Enabled = false;
			this.tbPosition.Location = new System.Drawing.Point(12, 41);
			this.tbPosition.Name = "tbPosition";
			this.tbPosition.Size = new System.Drawing.Size(202, 20);
			this.tbPosition.TabIndex = 6;
			// 
			// pictureBox1
			// 
			this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pictureBox1.Location = new System.Drawing.Point(0, 0);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(653, 526);
			this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.pictureBox1.TabIndex = 7;
			this.pictureBox1.TabStop = false;
			// 
			// panel1
			// 
			this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panel1.AutoScroll = true;
			this.panel1.Controls.Add(this.pictureBox1);
			this.panel1.Location = new System.Drawing.Point(220, 12);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(653, 526);
			this.panel1.TabIndex = 8;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.label5);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.tbErrorPerMeter);
			this.groupBox1.Controls.Add(this.tbMaxError);
			this.groupBox1.Controls.Add(this.btnUpdateLoadedItems);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this.tbPosY);
			this.groupBox1.Controls.Add(this.tbPosZ);
			this.groupBox1.Controls.Add(this.tbPosX);
			this.groupBox1.Location = new System.Drawing.Point(14, 350);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(200, 189);
			this.groupBox1.TabIndex = 9;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "groupBox1";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(6, 154);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(52, 13);
			this.label5.TabIndex = 10;
			this.label5.Text = "Max Error";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(6, 131);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(76, 13);
			this.label4.TabIndex = 9;
			this.label4.Text = "Error per meter";
			// 
			// tbErrorPerMeter
			// 
			this.tbErrorPerMeter.Location = new System.Drawing.Point(94, 128);
			this.tbErrorPerMeter.Name = "tbErrorPerMeter";
			this.tbErrorPerMeter.Size = new System.Drawing.Size(100, 20);
			this.tbErrorPerMeter.TabIndex = 8;
			this.tbErrorPerMeter.Text = "10";
			// 
			// tbMaxError
			// 
			this.tbMaxError.Location = new System.Drawing.Point(94, 151);
			this.tbMaxError.Name = "tbMaxError";
			this.tbMaxError.Size = new System.Drawing.Size(100, 20);
			this.tbMaxError.TabIndex = 7;
			this.tbMaxError.Text = "10000";
			// 
			// btnUpdateLoadedItems
			// 
			this.btnUpdateLoadedItems.Location = new System.Drawing.Point(119, 101);
			this.btnUpdateLoadedItems.Name = "btnUpdateLoadedItems";
			this.btnUpdateLoadedItems.Size = new System.Drawing.Size(75, 23);
			this.btnUpdateLoadedItems.TabIndex = 6;
			this.btnUpdateLoadedItems.Text = "Update loaded items";
			this.btnUpdateLoadedItems.UseVisualStyleBackColor = true;
			this.btnUpdateLoadedItems.Click += new System.EventHandler(this.btnUpdateLoadedItems_Click);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(5, 78);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(17, 13);
			this.label3.TabIndex = 5;
			this.label3.Text = "Z:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(6, 52);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(17, 13);
			this.label2.TabIndex = 4;
			this.label2.Text = "Y:";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(6, 25);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(17, 13);
			this.label1.TabIndex = 3;
			this.label1.Text = "X:";
			// 
			// tbPosY
			// 
			this.tbPosY.Location = new System.Drawing.Point(47, 49);
			this.tbPosY.Name = "tbPosY";
			this.tbPosY.Size = new System.Drawing.Size(147, 20);
			this.tbPosY.TabIndex = 2;
			this.tbPosY.Text = "0";
			// 
			// tbPosZ
			// 
			this.tbPosZ.Location = new System.Drawing.Point(46, 75);
			this.tbPosZ.Name = "tbPosZ";
			this.tbPosZ.Size = new System.Drawing.Size(148, 20);
			this.tbPosZ.TabIndex = 1;
			this.tbPosZ.Text = "10";
			// 
			// tbPosX
			// 
			this.tbPosX.Location = new System.Drawing.Point(46, 22);
			this.tbPosX.Name = "tbPosX";
			this.tbPosX.Size = new System.Drawing.Size(148, 20);
			this.tbPosX.TabIndex = 0;
			this.tbPosX.Text = "0";
			// 
			// btnChild0
			// 
			this.btnChild0.Location = new System.Drawing.Point(12, 96);
			this.btnChild0.Name = "btnChild0";
			this.btnChild0.Size = new System.Drawing.Size(100, 23);
			this.btnChild0.TabIndex = 11;
			this.btnChild0.Text = "Go to child 0";
			this.btnChild0.UseVisualStyleBackColor = true;
			this.btnChild0.Click += new System.EventHandler(this.btnChild0_Click);
			// 
			// btnChild1
			// 
			this.btnChild1.Location = new System.Drawing.Point(118, 96);
			this.btnChild1.Name = "btnChild1";
			this.btnChild1.Size = new System.Drawing.Size(96, 23);
			this.btnChild1.TabIndex = 12;
			this.btnChild1.Text = "Go to child 1";
			this.btnChild1.UseVisualStyleBackColor = true;
			this.btnChild1.Click += new System.EventHandler(this.btnChild1_Click);
			// 
			// btnSaveImage
			// 
			this.btnSaveImage.Location = new System.Drawing.Point(133, 12);
			this.btnSaveImage.Name = "btnSaveImage";
			this.btnSaveImage.Size = new System.Drawing.Size(75, 23);
			this.btnSaveImage.TabIndex = 13;
			this.btnSaveImage.Text = "Save image";
			this.btnSaveImage.UseVisualStyleBackColor = true;
			this.btnSaveImage.Click += new System.EventHandler(this.btnSaveImage_Click);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(885, 550);
			this.Controls.Add(this.btnSaveImage);
			this.Controls.Add(this.btnChild1);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.btnChild0);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.tbPosition);
			this.Controls.Add(this.cbChilds);
			this.Controls.Add(this.btnChild);
			this.Controls.Add(this.btnParent);
			this.Controls.Add(this.tbInfo);
			this.Controls.Add(this.btnOpen);
			this.Name = "Form1";
			this.Text = "Form1";
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.panel1.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnOpen;
		private System.Windows.Forms.TextBox tbInfo;
		private System.Windows.Forms.Button btnParent;
		private System.Windows.Forms.Button btnChild;
		private System.Windows.Forms.ComboBox cbChilds;
		private System.Windows.Forms.TextBox tbPosition;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Button btnUpdateLoadedItems;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox tbPosY;
		private System.Windows.Forms.TextBox tbPosZ;
		private System.Windows.Forms.TextBox tbPosX;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox tbErrorPerMeter;
		private System.Windows.Forms.TextBox tbMaxError;
		private System.Windows.Forms.Button btnChild0;
		private System.Windows.Forms.Button btnChild1;
		private System.Windows.Forms.Button btnSaveImage;
	}
}

