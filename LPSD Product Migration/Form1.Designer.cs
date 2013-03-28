namespace LPSD_Migration
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
            this.startButton = new System.Windows.Forms.Button();
            this.outputLabel = new System.Windows.Forms.Label();
            this.pullCategoryButton = new System.Windows.Forms.Button();
            this.outputRichTextBox = new System.Windows.Forms.RichTextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // startButton
            // 
            this.startButton.Location = new System.Drawing.Point(254, 12);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(184, 23);
            this.startButton.TabIndex = 0;
            this.startButton.Text = "Start Pulling Product Data";
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            // 
            // outputLabel
            // 
            this.outputLabel.AutoSize = true;
            this.outputLabel.Location = new System.Drawing.Point(12, 292);
            this.outputLabel.Name = "outputLabel";
            this.outputLabel.Size = new System.Drawing.Size(39, 13);
            this.outputLabel.TabIndex = 2;
            this.outputLabel.Text = "Output";
            // 
            // pullCategoryButton
            // 
            this.pullCategoryButton.Location = new System.Drawing.Point(254, 295);
            this.pullCategoryButton.Name = "pullCategoryButton";
            this.pullCategoryButton.Size = new System.Drawing.Size(184, 23);
            this.pullCategoryButton.TabIndex = 3;
            this.pullCategoryButton.Text = "Start Pulling Category Data";
            this.pullCategoryButton.UseVisualStyleBackColor = true;
            this.pullCategoryButton.Click += new System.EventHandler(this.pullCategoryButton_Click);
            // 
            // outputRichTextBox
            // 
            this.outputRichTextBox.Location = new System.Drawing.Point(12, 41);
            this.outputRichTextBox.Name = "outputRichTextBox";
            this.outputRichTextBox.Size = new System.Drawing.Size(668, 248);
            this.outputRichTextBox.TabIndex = 4;
            this.outputRichTextBox.Text = "";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(132, 327);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(100, 50);
            this.pictureBox1.TabIndex = 5;
            this.pictureBox1.TabStop = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(692, 383);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.outputRichTextBox);
            this.Controls.Add(this.pullCategoryButton);
            this.Controls.Add(this.outputLabel);
            this.Controls.Add(this.startButton);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.Label outputLabel;
        private System.Windows.Forms.Button pullCategoryButton;
        private System.Windows.Forms.RichTextBox outputRichTextBox;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}

