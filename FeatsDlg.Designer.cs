namespace CS_Editor
{
    partial class FeatsDlg
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
            label1 = new Label();
            lvFeats = new ListView();
            chFeatName = new ColumnHeader();
            chFeatCategory = new ColumnHeader();
            chFeatEffect = new ColumnHeader();
            btnRemove = new Button();
            label2 = new Label();
            lvAvailable = new ListView();
            chName = new ColumnHeader();
            chRequirement = new ColumnHeader();
            chEffect = new ColumnHeader();
            btnAdd = new Button();
            tbDetails = new TextBox();
            label3 = new Label();
            tbAssetName = new TextBox();
            btnAddName = new Button();
            okBtn = new Button();
            cancelBtn = new Button();
            SuspendLayout();
            //
            // label1
            //
            label1.AutoSize = true;
            label1.Location = new Point(12, 12);
            label1.Name = "label1";
            label1.Size = new Size(93, 15);
            label1.TabIndex = 0;
            label1.Text = "Character &feats:";
            //
            // lvFeats
            //
            lvFeats.Columns.AddRange(new ColumnHeader[] { chFeatName, chFeatCategory, chFeatEffect });
            lvFeats.FullRowSelect = true;
            lvFeats.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            lvFeats.HideSelection = false;
            lvFeats.Location = new Point(12, 30);
            lvFeats.MultiSelect = false;
            lvFeats.Name = "lvFeats";
            lvFeats.ShowItemToolTips = true;
            lvFeats.Size = new Size(760, 150);
            lvFeats.TabIndex = 1;
            lvFeats.UseCompatibleStateImageBehavior = false;
            lvFeats.View = View.Details;
            lvFeats.SelectedIndexChanged += LvFeats_SelectedIndexChanged;
            //
            // chFeatName
            //
            chFeatName.Text = "Feat";
            chFeatName.Width = 170;
            //
            // chFeatCategory
            //
            chFeatCategory.Text = "Category";
            chFeatCategory.Width = 190;
            //
            // chFeatEffect
            //
            chFeatEffect.Text = "Effect";
            chFeatEffect.Width = 375;
            //
            // btnRemove
            //
            btnRemove.Location = new Point(778, 30);
            btnRemove.Name = "btnRemove";
            btnRemove.Size = new Size(75, 23);
            btnRemove.TabIndex = 2;
            btnRemove.Text = "&Remove";
            btnRemove.UseVisualStyleBackColor = true;
            btnRemove.Click += BtnRemove_Click;
            //
            // label2
            //
            label2.AutoSize = true;
            label2.Location = new Point(12, 190);
            label2.Name = "label2";
            label2.Size = new Size(229, 15);
            label2.TabIndex = 3;
            label2.Text = "A&ll feats (double click or Add to add feat):";
            //
            // lvAvailable
            //
            lvAvailable.Columns.AddRange(new ColumnHeader[] { chName, chRequirement, chEffect });
            lvAvailable.FullRowSelect = true;
            lvAvailable.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            lvAvailable.HideSelection = false;
            lvAvailable.Location = new Point(12, 208);
            lvAvailable.MultiSelect = false;
            lvAvailable.Name = "lvAvailable";
            lvAvailable.ShowItemToolTips = true;
            lvAvailable.Size = new Size(760, 300);
            lvAvailable.TabIndex = 4;
            lvAvailable.UseCompatibleStateImageBehavior = false;
            lvAvailable.View = View.Details;
            lvAvailable.SelectedIndexChanged += LvAvailable_SelectedIndexChanged;
            lvAvailable.DoubleClick += LvAvailable_DoubleClick;
            //
            // chName
            //
            chName.Text = "Feat";
            chName.Width = 170;
            //
            // chRequirement
            //
            chRequirement.Text = "Requirement";
            chRequirement.Width = 150;
            //
            // chEffect
            //
            chEffect.Text = "Effect";
            chEffect.Width = 415;
            //
            // btnAdd
            //
            btnAdd.Location = new Point(778, 208);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(75, 23);
            btnAdd.TabIndex = 5;
            btnAdd.Text = "&Add";
            btnAdd.UseVisualStyleBackColor = true;
            btnAdd.Click += BtnAdd_Click;
            //
            // tbDetails
            //
            tbDetails.Location = new Point(12, 514);
            tbDetails.Multiline = true;
            tbDetails.Name = "tbDetails";
            tbDetails.ReadOnly = true;
            tbDetails.Size = new Size(760, 64);
            tbDetails.TabIndex = 6;
            tbDetails.TabStop = false;
            //
            // label3
            //
            label3.AutoSize = true;
            label3.Location = new Point(12, 591);
            label3.Name = "label3";
            label3.Size = new Size(166, 15);
            label3.TabIndex = 7;
            label3.Text = "Other feat, by asset &name (F_xxx):";
            //
            // tbAssetName
            //
            tbAssetName.Location = new Point(210, 588);
            tbAssetName.Name = "tbAssetName";
            tbAssetName.Size = new Size(562, 23);
            tbAssetName.TabIndex = 8;
            //
            // btnAddName
            //
            btnAddName.Location = new Point(778, 587);
            btnAddName.Name = "btnAddName";
            btnAddName.Size = new Size(75, 23);
            btnAddName.TabIndex = 9;
            btnAddName.Text = "Add Na&me";
            btnAddName.UseVisualStyleBackColor = true;
            btnAddName.Click += BtnAddName_Click;
            //
            // okBtn
            //
            okBtn.DialogResult = DialogResult.OK;
            okBtn.Location = new Point(12, 626);
            okBtn.Name = "okBtn";
            okBtn.Size = new Size(75, 23);
            okBtn.TabIndex = 10;
            okBtn.Text = "&OK";
            okBtn.UseVisualStyleBackColor = true;
            //
            // cancelBtn
            //
            cancelBtn.DialogResult = DialogResult.Cancel;
            cancelBtn.Location = new Point(93, 626);
            cancelBtn.Name = "cancelBtn";
            cancelBtn.Size = new Size(75, 23);
            cancelBtn.TabIndex = 11;
            cancelBtn.Text = "&Cancel";
            cancelBtn.UseVisualStyleBackColor = true;
            //
            // FeatsDlg
            //
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = cancelBtn;
            ClientSize = new Size(865, 661);
            Controls.Add(cancelBtn);
            Controls.Add(okBtn);
            Controls.Add(btnAddName);
            Controls.Add(tbAssetName);
            Controls.Add(label3);
            Controls.Add(tbDetails);
            Controls.Add(btnAdd);
            Controls.Add(lvAvailable);
            Controls.Add(label2);
            Controls.Add(btnRemove);
            Controls.Add(lvFeats);
            Controls.Add(label1);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FeatsDlg";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Character Feats";
            Load += FeatsDlg_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private ListView lvFeats;
        private ColumnHeader chFeatName;
        private ColumnHeader chFeatCategory;
        private ColumnHeader chFeatEffect;
        private Button btnRemove;
        private Label label2;
        private ListView lvAvailable;
        private ColumnHeader chName;
        private ColumnHeader chRequirement;
        private ColumnHeader chEffect;
        private Button btnAdd;
        private TextBox tbDetails;
        private Label label3;
        private TextBox tbAssetName;
        private Button btnAddName;
        private Button okBtn;
        private Button cancelBtn;
    }
}
