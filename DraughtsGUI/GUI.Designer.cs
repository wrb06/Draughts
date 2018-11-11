namespace DraughtsGUI
{
    partial class GUI
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
            this.EndTurn = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // EndTurn
            // 
            this.EndTurn.Location = new System.Drawing.Point(972, 25);
            this.EndTurn.Name = "EndTurn";
            this.EndTurn.Size = new System.Drawing.Size(75, 23);
            this.EndTurn.TabIndex = 0;
            this.EndTurn.Text = "Next";
            this.EndTurn.UseVisualStyleBackColor = true;
            this.EndTurn.Click += new System.EventHandler(this.EndTurn_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(972, 90);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 20);
            this.textBox1.TabIndex = 1;
            // 
            // GUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1102, 507);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.EndTurn);
            this.Name = "GUI";
            this.Text = "Draughts";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button EndTurn;
        private System.Windows.Forms.TextBox textBox1;
    }
}

