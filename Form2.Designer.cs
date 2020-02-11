namespace Binairo
{
    partial class Form2
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rb12 = new System.Windows.Forms.RadioButton();
            this.rb10 = new System.Windows.Forms.RadioButton();
            this.rb8 = new System.Windows.Forms.RadioButton();
            this.bOK = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rb12);
            this.groupBox1.Controls.Add(this.rb10);
            this.groupBox1.Controls.Add(this.rb8);
            this.groupBox1.Location = new System.Drawing.Point(16, 15);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Size = new System.Drawing.Size(188, 123);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "groupBox1";
            // 
            // rb12
            // 
            this.rb12.AutoSize = true;
            this.rb12.Location = new System.Drawing.Point(25, 80);
            this.rb12.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.rb12.Name = "rb12";
            this.rb12.Size = new System.Drawing.Size(71, 21);
            this.rb12.TabIndex = 3;
            this.rb12.Text = "12 x12";
            this.rb12.UseVisualStyleBackColor = true;
            // 
            // rb10
            // 
            this.rb10.AutoSize = true;
            this.rb10.Location = new System.Drawing.Point(25, 52);
            this.rb10.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.rb10.Name = "rb10";
            this.rb10.Size = new System.Drawing.Size(71, 21);
            this.rb10.TabIndex = 2;
            this.rb10.Text = "10 x10";
            this.rb10.UseVisualStyleBackColor = true;
            // 
            // rb8
            // 
            this.rb8.AutoSize = true;
            this.rb8.Checked = true;
            this.rb8.Location = new System.Drawing.Point(25, 23);
            this.rb8.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.rb8.Name = "rb8";
            this.rb8.Size = new System.Drawing.Size(59, 21);
            this.rb8.TabIndex = 1;
            this.rb8.TabStop = true;
            this.rb8.Text = "8 x 8";
            this.rb8.UseVisualStyleBackColor = true;
            // 
            // bOK
            // 
            this.bOK.Location = new System.Drawing.Point(276, 137);
            this.bOK.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.bOK.Name = "bOK";
            this.bOK.Size = new System.Drawing.Size(100, 28);
            this.bOK.TabIndex = 1;
            this.bOK.Text = "OK";
            this.bOK.UseVisualStyleBackColor = true;
            this.bOK.Click += new System.EventHandler(this.bOK_Click);
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(392, 180);
            this.Controls.Add(this.bOK);
            this.Controls.Add(this.groupBox1);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "Form2";
            this.Text = "Dimension de la nouvelle grille";
            this.Load += new System.EventHandler(this.Form2_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rb12;
        private System.Windows.Forms.RadioButton rb10;
        private System.Windows.Forms.RadioButton rb8;
        private System.Windows.Forms.Button bOK;
    }
}