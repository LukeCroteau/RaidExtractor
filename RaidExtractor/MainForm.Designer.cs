namespace RaidExtractor
{
    partial class MainForm
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
            this.SaveButton = new System.Windows.Forms.Button();
            this.SaveJSONDialog = new System.Windows.Forms.SaveFileDialog();
            this.UploadButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // SaveButton
            // 
            this.SaveButton.Location = new System.Drawing.Point(12, 12);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(75, 23);
            this.SaveButton.TabIndex = 0;
            this.SaveButton.Text = "Save JSON";
            this.SaveButton.UseVisualStyleBackColor = true;
            this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // SaveJSONDialog
            // 
            this.SaveJSONDialog.DefaultExt = "json";
            this.SaveJSONDialog.FileName = "artifacts.json";
            this.SaveJSONDialog.Filter = "JSON files|*.json";
            this.SaveJSONDialog.Title = "Save JSON";
            // 
            // UploadButton
            // 
            this.UploadButton.Location = new System.Drawing.Point(93, 12);
            this.UploadButton.Name = "UploadButton";
            this.UploadButton.Size = new System.Drawing.Size(75, 23);
            this.UploadButton.TabIndex = 1;
            this.UploadButton.Text = "Upload JSON";
            this.UploadButton.UseVisualStyleBackColor = true;
            this.UploadButton.Visible = false;
            this.UploadButton.Click += new System.EventHandler(this.UploadButton_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(208, 48);
            this.Controls.Add(this.UploadButton);
            this.Controls.Add(this.SaveButton);
            this.Name = "MainForm";
            this.Text = "Raid Extractor";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button SaveButton;
        private System.Windows.Forms.SaveFileDialog SaveJSONDialog;
        private System.Windows.Forms.Button UploadButton;
    }
}