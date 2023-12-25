namespace CS_Editor
{
    partial class CharSelDlg
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
            okBtn = new Button();
            cancelBtn = new Button();
            cbNames = new ComboBox();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 18);
            label1.Name = "label1";
            label1.Size = new Size(130, 15);
            label1.TabIndex = 0;
            label1.Text = "&Select Character Name:";
            // 
            // okBtn
            // 
            okBtn.Location = new Point(12, 72);
            okBtn.Name = "okBtn";
            okBtn.Size = new Size(75, 23);
            okBtn.TabIndex = 2;
            okBtn.Text = "&OK";
            okBtn.UseVisualStyleBackColor = true;
            okBtn.Click += OkBtn_Click;
            // 
            // cancelBtn
            // 
            cancelBtn.DialogResult = DialogResult.Cancel;
            cancelBtn.Location = new Point(93, 72);
            cancelBtn.Name = "cancelBtn";
            cancelBtn.Size = new Size(75, 23);
            cancelBtn.TabIndex = 3;
            cancelBtn.Text = "&Cancel";
            cancelBtn.UseVisualStyleBackColor = true;
            // 
            // cbNames
            // 
            cbNames.DropDownStyle = ComboBoxStyle.DropDownList;
            cbNames.FormattingEnabled = true;
            cbNames.Items.AddRange(new object[] { "", "Evans", "Faythe", "Jed", "Mathias", "Garret", "Cobra", "Knurl", "Harbinger", "Romeo" });
            cbNames.Location = new Point(148, 15);
            cbNames.Name = "cbNames";
            cbNames.Size = new Size(196, 23);
            cbNames.TabIndex = 1;
            // 
            // CharSelDlg
            // 
            AcceptButton = okBtn;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = cancelBtn;
            ClientSize = new Size(356, 122);
            Controls.Add(cbNames);
            Controls.Add(cancelBtn);
            Controls.Add(okBtn);
            Controls.Add(label1);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            Name = "CharSelDlg";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Character Selection";
            Load += CharSelDlg_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Button okBtn;
        private Button cancelBtn;
        private ComboBox cbNames;
    }
}