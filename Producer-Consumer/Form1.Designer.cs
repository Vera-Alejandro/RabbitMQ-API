namespace Producer_Consumer
{
    partial class ProducerConsumer
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
            this.Download = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // Download
            // 
            this.Download.BackColor = System.Drawing.Color.White;
            this.Download.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Download.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Download.ForeColor = System.Drawing.Color.Black;
            this.Download.Location = new System.Drawing.Point(329, 262);
            this.Download.Name = "Download";
            this.Download.Size = new System.Drawing.Size(123, 47);
            this.Download.TabIndex = 0;
            this.Download.Text = "Download";
            this.Download.UseVisualStyleBackColor = false;
            this.Download.Click += new System.EventHandler(this.Download_Click);
            // 
            // ProducerConsumer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.Download);
            this.ForeColor = System.Drawing.Color.Black;
            this.Name = "ProducerConsumer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Producer Consumer";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button Download;
    }
}

