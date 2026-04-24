namespace frmnetx86
{
    partial class Form2
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form2));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.lblOr = new System.Windows.Forms.Label();
            this.lblMomo = new System.Windows.Forms.Label();
            this.lblTcb = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::frmnetx86.Properties.Resources.momo;
            this.pictureBox1.Location = new System.Drawing.Point(12, 28);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(160, 160);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::frmnetx86.Properties.Resources.tcb;
            this.pictureBox2.Location = new System.Drawing.Point(248, 28);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(160, 160);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox2.TabIndex = 1;
            this.pictureBox2.TabStop = false;
            // 
            // lblOr
            // 
            this.lblOr.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblOr.Location = new System.Drawing.Point(180, 88);
            this.lblOr.Name = "lblOr";
            this.lblOr.Size = new System.Drawing.Size(60, 40);
            this.lblOr.TabIndex = 2;
            this.lblOr.Text = "OR";
            this.lblOr.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblMomo
            // 
            this.lblMomo.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblMomo.Location = new System.Drawing.Point(12, 196);
            this.lblMomo.Name = "lblMomo";
            this.lblMomo.Size = new System.Drawing.Size(160, 20);
            this.lblMomo.TabIndex = 3;
            this.lblMomo.Text = "MoMo";
            this.lblMomo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblTcb
            // 
            this.lblTcb.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblTcb.Location = new System.Drawing.Point(248, 196);
            this.lblTcb.Name = "lblTcb";
            this.lblTcb.Size = new System.Drawing.Size(160, 20);
            this.lblTcb.TabIndex = 4;
            this.lblTcb.Text = "Techcombank";
            this.lblTcb.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(420, 228);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.lblOr);
            this.Controls.Add(this.lblMomo);
            this.Controls.Add(this.lblTcb);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form2";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Donate - Support Us";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Label      lblOr;
        private System.Windows.Forms.Label      lblMomo;
        private System.Windows.Forms.Label      lblTcb;
    }
}
