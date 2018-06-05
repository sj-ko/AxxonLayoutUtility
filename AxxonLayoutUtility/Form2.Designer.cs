namespace AxxonLayoutUtility
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form2));
            this.button_displayAll = new System.Windows.Forms.Button();
            this.button_Minimize = new System.Windows.Forms.Button();
            this.button_Exit = new System.Windows.Forms.Button();
            this.comboBox_layout = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button_displayAll
            // 
            this.button_displayAll.BackColor = System.Drawing.Color.DarkOrange;
            this.button_displayAll.Location = new System.Drawing.Point(13, 26);
            this.button_displayAll.Name = "button_displayAll";
            this.button_displayAll.Size = new System.Drawing.Size(108, 29);
            this.button_displayAll.TabIndex = 2;
            this.button_displayAll.Text = "일괄 시작";
            this.button_displayAll.UseVisualStyleBackColor = false;
            this.button_displayAll.TextChanged += new System.EventHandler(this.button_displayAll_TextChanged);
            this.button_displayAll.Click += new System.EventHandler(this.button_displayAll_Click);
            // 
            // button_Minimize
            // 
            this.button_Minimize.Location = new System.Drawing.Point(94, 0);
            this.button_Minimize.Name = "button_Minimize";
            this.button_Minimize.Size = new System.Drawing.Size(18, 20);
            this.button_Minimize.TabIndex = 3;
            this.button_Minimize.Text = "_";
            this.button_Minimize.UseVisualStyleBackColor = true;
            this.button_Minimize.Click += new System.EventHandler(this.button_Minimize_Click);
            // 
            // button_Exit
            // 
            this.button_Exit.Location = new System.Drawing.Point(112, 0);
            this.button_Exit.Name = "button_Exit";
            this.button_Exit.Size = new System.Drawing.Size(18, 20);
            this.button_Exit.TabIndex = 4;
            this.button_Exit.Text = "X";
            this.button_Exit.UseVisualStyleBackColor = true;
            this.button_Exit.Click += new System.EventHandler(this.button_Exit_Click);
            // 
            // comboBox_layout
            // 
            this.comboBox_layout.FormattingEnabled = true;
            this.comboBox_layout.Location = new System.Drawing.Point(58, 67);
            this.comboBox_layout.Name = "comboBox_layout";
            this.comboBox_layout.Size = new System.Drawing.Size(64, 20);
            this.comboBox_layout.TabIndex = 5;
            this.comboBox_layout.SelectedIndexChanged += new System.EventHandler(this.comboBox_layout_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 70);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 12);
            this.label2.TabIndex = 7;
            this.label2.Text = "이동";
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.ClientSize = new System.Drawing.Size(133, 95);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.comboBox_layout);
            this.Controls.Add(this.button_Exit);
            this.Controls.Add(this.button_Minimize);
            this.Controls.Add(this.button_displayAll);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form2";
            this.Text = "레이아웃";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form2_FormClosing);
            this.Load += new System.EventHandler(this.Form2_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form2_MouseDown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button button_Minimize;
        private System.Windows.Forms.Button button_Exit;
        public System.Windows.Forms.Button button_displayAll;
        private System.Windows.Forms.ComboBox comboBox_layout;
        private System.Windows.Forms.Label label2;
    }
}

