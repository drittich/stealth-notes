
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
			this.components = new System.ComponentModel.Container();
			this.dgvInputs = new System.Windows.Forms.DataGridView();
			this.IsSelected = new System.Windows.Forms.DataGridViewCheckBoxColumn();
			this.FriendlyName = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.IsMuted = new System.Windows.Forms.DataGridViewCheckBoxColumn();
			this.tbMuteInterval = new System.Windows.Forms.TrackBar();
			this.lblUnmuteDuration = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.btnReloadDevices = new System.Windows.Forms.Button();
			this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
			this.chkIgnoreModifierKeys = new System.Windows.Forms.CheckBox();
			((System.ComponentModel.ISupportInitialize)(this.dgvInputs)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.tbMuteInterval)).BeginInit();
			this.SuspendLayout();
			// 
			// dgvInputs
			// 
			this.dgvInputs.AllowUserToAddRows = false;
			this.dgvInputs.AllowUserToDeleteRows = false;
			this.dgvInputs.AllowUserToOrderColumns = true;
			this.dgvInputs.AllowUserToResizeRows = false;
			this.dgvInputs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.dgvInputs.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
			this.dgvInputs.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvInputs.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.IsSelected,
            this.FriendlyName,
            this.IsMuted});
			this.dgvInputs.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnKeystroke;
			this.dgvInputs.Location = new System.Drawing.Point(13, 53);
			this.dgvInputs.MultiSelect = false;
			this.dgvInputs.Name = "dgvInputs";
			this.dgvInputs.RowHeadersVisible = false;
			this.dgvInputs.RowHeadersWidth = 62;
			this.dgvInputs.RowTemplate.Height = 33;
			this.dgvInputs.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.dgvInputs.ShowCellToolTips = false;
			this.dgvInputs.Size = new System.Drawing.Size(723, 253);
			this.dgvInputs.TabIndex = 1;
			this.dgvInputs.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
			this.dgvInputs.SelectionChanged += new System.EventHandler(this.dataGridView1_SelectionChanged);
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
			this.IsMuted.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
			// 
			// tbMuteInterval
			// 
			this.tbMuteInterval.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.tbMuteInterval.LargeChange = 1;
			this.tbMuteInterval.Location = new System.Drawing.Point(5, 312);
			this.tbMuteInterval.Minimum = 1;
			this.tbMuteInterval.Name = "tbMuteInterval";
			this.tbMuteInterval.Size = new System.Drawing.Size(325, 69);
			this.tbMuteInterval.TabIndex = 5;
			this.tbMuteInterval.Value = 1;
			this.tbMuteInterval.Scroll += new System.EventHandler(this.trackBar1_Scroll);
			this.tbMuteInterval.ValueChanged += new System.EventHandler(this.trackBar1_ValueChanged);
			// 
			// lblUnmuteDuration
			// 
			this.lblUnmuteDuration.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.lblUnmuteDuration.AutoSize = true;
			this.lblUnmuteDuration.Location = new System.Drawing.Point(68, 344);
			this.lblUnmuteDuration.Name = "lblUnmuteDuration";
			this.lblUnmuteDuration.Size = new System.Drawing.Size(201, 25);
			this.lblUnmuteDuration.TabIndex = 1;
			this.lblUnmuteDuration.Text = "Unmute after 2 seconds";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(13, 13);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(317, 25);
			this.label1.TabIndex = 9;
			this.label1.Text = "Select the inputs to mute when typing:";
			// 
			// btnReloadDevices
			// 
			this.btnReloadDevices.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnReloadDevices.Font = new System.Drawing.Font("Segoe UI", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			this.btnReloadDevices.ForeColor = System.Drawing.SystemColors.Highlight;
			this.btnReloadDevices.Location = new System.Drawing.Point(328, 6);
			this.btnReloadDevices.Name = "btnReloadDevices";
			this.btnReloadDevices.Size = new System.Drawing.Size(112, 25);
			this.btnReloadDevices.TabIndex = 10;
			this.btnReloadDevices.Text = "button1";
			this.btnReloadDevices.UseVisualStyleBackColor = true;
			this.btnReloadDevices.Click += new System.EventHandler(this.btnReloadDevices_Click);
			// 
			// notifyIcon1
			// 
			this.notifyIcon1.Text = "Stealth Notes";
			this.notifyIcon1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseClick);
			// 
			// chkIgnoreModifierKeys
			// 
			this.chkIgnoreModifierKeys.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.chkIgnoreModifierKeys.AutoSize = true;
			this.chkIgnoreModifierKeys.Location = new System.Drawing.Point(354, 342);
			this.chkIgnoreModifierKeys.Name = "chkIgnoreModifierKeys";
			this.chkIgnoreModifierKeys.Size = new System.Drawing.Size(202, 29);
			this.chkIgnoreModifierKeys.TabIndex = 11;
			this.chkIgnoreModifierKeys.Text = "Ignore modifier keys";
			this.chkIgnoreModifierKeys.UseVisualStyleBackColor = true;
			this.chkIgnoreModifierKeys.Click += new System.EventHandler(this.chkIgnoreAltTab_Click);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(750, 382);
			this.Controls.Add(this.chkIgnoreModifierKeys);
			this.Controls.Add(this.btnReloadDevices);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.lblUnmuteDuration);
			this.Controls.Add(this.tbMuteInterval);
			this.Controls.Add(this.dgvInputs);
			this.MinimumSize = new System.Drawing.Size(538, 360);
			this.Name = "Form1";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Stealth Notes";
			this.Load += new System.EventHandler(this.Form1_Load);
			this.Resize += new System.EventHandler(this.Form1_Resize);
			((System.ComponentModel.ISupportInitialize)(this.dgvInputs)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.tbMuteInterval)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.DataGridView dgvInputs;
		private System.Windows.Forms.TrackBar tbMuteInterval;
		private System.Windows.Forms.Label lblUnmuteDuration;
		private System.Windows.Forms.DataGridViewCheckBoxColumn IsSelected;
		private System.Windows.Forms.DataGridViewTextBoxColumn FriendlyName;
		private System.Windows.Forms.DataGridViewCheckBoxColumn IsMuted;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button btnReloadDevices;
		private System.Windows.Forms.NotifyIcon notifyIcon1;
		private System.Windows.Forms.CheckBox chkIgnoreModifierKeys;
	}
}

