namespace SonnySystem
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Button btnReplenishment;
        private System.Windows.Forms.Button btnClients;
        private System.Windows.Forms.Button btnProducts;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Button btnAdjustment;
        private System.Windows.Forms.Label labelTitle;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.btnReplenishment = new System.Windows.Forms.Button();
            this.btnClients = new System.Windows.Forms.Button();
            this.btnProducts = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnAdjustment = new System.Windows.Forms.Button();
            this.labelTitle = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnReplenishment
            // 
            this.btnReplenishment.BackColor = System.Drawing.Color.LightSeaGreen;
            this.btnReplenishment.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnReplenishment.ForeColor = System.Drawing.Color.White;
            this.btnReplenishment.Location = new System.Drawing.Point(50, 80);
            this.btnReplenishment.Name = "btnReplenishment";
            this.btnReplenishment.Size = new System.Drawing.Size(300, 100);
            this.btnReplenishment.TabIndex = 0;
            this.btnReplenishment.Text = "NOVA REPOSIÇÃO";
            this.btnReplenishment.UseVisualStyleBackColor = false;
            this.btnReplenishment.Click += new System.EventHandler(this.btnReplenishment_Click);
            // 
            // btnClients
            // 
            this.btnClients.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnClients.Location = new System.Drawing.Point(50, 200);
            this.btnClients.Name = "btnClients";
            this.btnClients.Size = new System.Drawing.Size(140, 60);
            this.btnClients.TabIndex = 1;
            this.btnClients.Text = "Gerenciar Clientes";
            this.btnClients.UseVisualStyleBackColor = true;
            this.btnClients.Click += new System.EventHandler(this.btnClients_Click);
            // 
            // btnProducts
            // 
            this.btnProducts.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnProducts.Location = new System.Drawing.Point(210, 200);
            this.btnProducts.Name = "btnProducts";
            this.btnProducts.Size = new System.Drawing.Size(140, 60);
            this.btnProducts.TabIndex = 2;
            this.btnProducts.Text = "Gerenciar Produtos";
            this.btnProducts.UseVisualStyleBackColor = true;
            this.btnProducts.Click += new System.EventHandler(this.btnProducts_Click);
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(150, 300);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(100, 30);
            this.btnExit.TabIndex = 3;
            this.btnExit.Text = "Sair";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnAdjustment
            // 
            this.btnAdjustment.Location = new System.Drawing.Point(130, 270);
            this.btnAdjustment.Name = "btnAdjustment";
            this.btnAdjustment.Size = new System.Drawing.Size(140, 25);
            this.btnAdjustment.TabIndex = 5;
            this.btnAdjustment.Text = "Ajuste Manual";
            this.btnAdjustment.UseVisualStyleBackColor = true;
            this.btnAdjustment.Click += new System.EventHandler(this.btnAdjustment_Click);
            // 
            // labelTitle
            // 
            this.labelTitle.AutoSize = true;
            this.labelTitle.Font = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.labelTitle.Location = new System.Drawing.Point(45, 20);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(225, 37);
            this.labelTitle.TabIndex = 4;
            this.labelTitle.Text = "SONNY SYSTEM";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(400, 360);
            this.Controls.Add(this.btnAdjustment);
            this.Controls.Add(this.labelTitle);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnProducts);
            this.Controls.Add(this.btnClients);
            this.Controls.Add(this.btnReplenishment);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Menu Principal";
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
