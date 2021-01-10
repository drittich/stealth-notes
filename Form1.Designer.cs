﻿
namespace StealthNotes
{
	partial class Form1
	{
		/// <summary>
		///  Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		///  Clean up any resources being used.
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
		///  Required method for Designer support - do not modify
		///  the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.btnUnmuteAll = new System.Windows.Forms.Button();
			this.dataGridView1 = new System.Windows.Forms.DataGridView();
			this.IsSelected = new System.Windows.Forms.DataGridViewCheckBoxColumn();
			this.FriendlyName = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.IsMuted = new System.Windows.Forms.DataGridViewCheckBoxColumn();
			this.btnMuteAll = new System.Windows.Forms.Button();
			this.btnReload = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
			this.SuspendLayout();
			// 
			// btnUnmuteAll
			// 
			this.btnUnmuteAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnUnmuteAll.Location = new System.Drawing.Point(185, 294);
			this.btnUnmuteAll.Name = "btnUnmuteAll";
			this.btnUnmuteAll.Size = new System.Drawing.Size(147, 44);
			this.btnUnmuteAll.TabIndex = 2;
			this.btnUnmuteAll.Text = "Unmute All";
			this.btnUnmuteAll.UseVisualStyleBackColor = true;
			this.btnUnmuteAll.Click += new System.EventHandler(this.btnUnmuteAll_Click);
			// 
			// dataGridView1
			// 
			this.dataGridView1.AllowUserToAddRows = false;
			this.dataGridView1.AllowUserToDeleteRows = false;
			this.dataGridView1.AllowUserToOrderColumns = true;
			this.dataGridView1.AllowUserToResizeRows = false;
			this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
			this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.IsSelected,
            this.FriendlyName,
            this.IsMuted});
			this.dataGridView1.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnKeystroke;
			this.dataGridView1.Location = new System.Drawing.Point(13, 13);
			this.dataGridView1.MultiSelect = false;
			this.dataGridView1.Name = "dataGridView1";
			this.dataGridView1.RowHeadersVisible = false;
			this.dataGridView1.RowHeadersWidth = 62;
			this.dataGridView1.RowTemplate.Height = 33;
			this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.dataGridView1.ShowCellToolTips = false;
			this.dataGridView1.Size = new System.Drawing.Size(774, 259);
			this.dataGridView1.TabIndex = 3;
			this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
			this.dataGridView1.SelectionChanged += new System.EventHandler(this.dataGridView1_SelectionChanged);
			// 
			// IsSelected
			// 
			this.IsSelected.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
			this.IsSelected.HeaderText = "";
			this.IsSelected.MinimumWidth = 8;
			this.IsSelected.Name = "IsSelected";
			this.IsSelected.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.IsSelected.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
			this.IsSelected.Width = 29;
			// 
			// FriendlyName
			// 
			this.FriendlyName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.FriendlyName.HeaderText = "Input";
			this.FriendlyName.MinimumWidth = 8;
			this.FriendlyName.Name = "FriendlyName";
			this.FriendlyName.ReadOnly = true;
			// 
			// IsMuted
			// 
			this.IsMuted.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
			this.IsMuted.HeaderText = "Muted";
			this.IsMuted.MinimumWidth = 8;
			this.IsMuted.Name = "IsMuted";
			this.IsMuted.ReadOnly = true;
			this.IsMuted.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
			// 
			// btnMuteAll
			// 
			this.btnMuteAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnMuteAll.Location = new System.Drawing.Point(16, 294);
			this.btnMuteAll.Name = "btnMuteAll";
			this.btnMuteAll.Size = new System.Drawing.Size(147, 44);
			this.btnMuteAll.TabIndex = 4;
			this.btnMuteAll.Text = "Mute All";
			this.btnMuteAll.UseVisualStyleBackColor = true;
			this.btnMuteAll.Click += new System.EventHandler(this.btnMuteAll_Click);
			// 
			// btnReload
			// 
			this.btnReload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnReload.Location = new System.Drawing.Point(354, 294);
			this.btnReload.Name = "btnReload";
			this.btnReload.Size = new System.Drawing.Size(147, 44);
			this.btnReload.TabIndex = 5;
			this.btnReload.Text = "Reload";
			this.btnReload.UseVisualStyleBackColor = true;
			this.btnReload.Click += new System.EventHandler(this.btnReload_Click);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(801, 356);
			this.Controls.Add(this.btnReload);
			this.Controls.Add(this.btnMuteAll);
			this.Controls.Add(this.dataGridView1);
			this.Controls.Add(this.btnUnmuteAll);
			this.MinimumSize = new System.Drawing.Size(538, 360);
			this.Name = "Form1";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Stealth Notes";
			this.Load += new System.EventHandler(this.Form1_Load);
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.Button btnUnmuteAll;
		private System.Windows.Forms.DataGridView dataGridView1;
		private System.Windows.Forms.Button btnMuteAll;
		private System.Windows.Forms.Button btnReload;
		private System.Windows.Forms.DataGridViewCheckBoxColumn IsSelected;
		private System.Windows.Forms.DataGridViewTextBoxColumn FriendlyName;
		private System.Windows.Forms.DataGridViewCheckBoxColumn IsMuted;
	}
}

