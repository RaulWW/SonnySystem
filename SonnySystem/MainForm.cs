using System;
using System.Windows.Forms;
using SonnySystem.Forms;

namespace SonnySystem
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void btnReplenishment_Click(object sender, EventArgs e)
        {
            var form = new ReplenishmentForm();
            form.ShowDialog();
        }

        private void btnClients_Click(object sender, EventArgs e)
        {
            var form = new ClientForm();
            form.ShowDialog();
        }

        private void btnProducts_Click(object sender, EventArgs e)
        {
            var form = new ProductForm();
            form.ShowDialog();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnAdjustment_Click(object sender, EventArgs e)
        {
            var form = new Forms.StockAdjustmentForm();
            form.ShowDialog();
        }
    }
}
