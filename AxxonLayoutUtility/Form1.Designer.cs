namespace AxxonLayoutUtility
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.button_display1 = new System.Windows.Forms.Button();
            this.button_display2 = new System.Windows.Forms.Button();
            this.button_displayAll = new System.Windows.Forms.Button();
            this.button_Minimize = new System.Windows.Forms.Button();
            this.button_Exit = new System.Windows.Forms.Button();
            this.button_display3 = new System.Windows.Forms.Button();
            this.button_display4 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button_display1
            // 
            this.button_display1.BackColor = System.Drawing.Color.DarkOrange;
            this.button_display1.Location = new System.Drawing.Point(11, 19);
            this.button_display1.Name = "button_display1";
            this.button_display1.Size = new System.Drawing.Size(108, 29);
            this.button_display1.TabIndex = 0;
            this.button_display1.Text = "1번 모니터 시작";
            this.button_display1.UseVisualStyleBackColor = false;
            this.button_display1.TextChanged += new System.EventHandler(this.button_display1_TextChanged);
            this.button_display1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button_display2
            // 
            this.button_display2.BackColor = System.Drawing.Color.DarkOrange;
            this.button_display2.Location = new System.Drawing.Point(11, 47);
            this.button_display2.Name = "button_display2";
            this.button_display2.Size = new System.Drawing.Size(108, 29);
            this.button_display2.TabIndex = 1;
            this.button_display2.Text = "2번 모니터 시작";
            this.button_display2.UseVisualStyleBackColor = false;
            this.button_display2.TextChanged += new System.EventHandler(this.button_display2_TextChanged);
            this.button_display2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button_displayAll
            // 
            this.button_displayAll.BackColor = System.Drawing.Color.DarkOrange;
            this.button_displayAll.Location = new System.Drawing.Point(11, 138);
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
            // button_display3
            // 
            this.button_display3.BackColor = System.Drawing.Color.DarkOrange;
            this.button_display3.Location = new System.Drawing.Point(11, 75);
            this.button_display3.Name = "button_display3";
            this.button_display3.Size = new System.Drawing.Size(108, 29);
            this.button_display3.TabIndex = 5;
            this.button_display3.Text = "3번 모니터 시작";
            this.button_display3.UseVisualStyleBackColor = false;
            this.button_display3.TextChanged += new System.EventHandler(this.button_display3_TextChanged);
            this.button_display3.Click += new System.EventHandler(this.button_display3_Click);
            // 
            // button_display4
            // 
            this.button_display4.BackColor = System.Drawing.Color.DarkOrange;
            this.button_display4.Location = new System.Drawing.Point(11, 103);
            this.button_display4.Name = "button_display4";
            this.button_display4.Size = new System.Drawing.Size(108, 29);
            this.button_display4.TabIndex = 6;
            this.button_display4.Text = "4번 모니터 시작";
            this.button_display4.UseVisualStyleBackColor = false;
            this.button_display4.TextChanged += new System.EventHandler(this.button_display4_TextChanged);
            this.button_display4.Click += new System.EventHandler(this.button_display4_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.ClientSize = new System.Drawing.Size(133, 179);
            this.Controls.Add(this.button_display4);
            this.Controls.Add(this.button_display3);
            this.Controls.Add(this.button_Exit);
            this.Controls.Add(this.button_Minimize);
            this.Controls.Add(this.button_displayAll);
            this.Controls.Add(this.button_display2);
            this.Controls.Add(this.button_display1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "레이아웃";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseDown);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button button_Minimize;
        private System.Windows.Forms.Button button_Exit;
        public System.Windows.Forms.Button button_display1;
        public System.Windows.Forms.Button button_display2;
        public System.Windows.Forms.Button button_displayAll;
        public System.Windows.Forms.Button button_display3;
        public System.Windows.Forms.Button button_display4;
    }
}

