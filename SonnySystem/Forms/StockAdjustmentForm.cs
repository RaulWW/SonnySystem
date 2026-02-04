using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using SonnySystem.Data;
using SonnySystem.Models;

namespace SonnySystem.Forms
{
    public partial class StockAdjustmentForm : Form
    {
        private ProductRepository _productRepo;
        private ClientRepository _clientRepo;
        private ClientProductRepository _cpRepo;
        private List<Product> _products = new();
        private List<Client> _clients = new();

        public StockAdjustmentForm()
        {
            InitializeComponent();
            _productRepo = new ProductRepository();
            _clientRepo = new ClientRepository();
            _cpRepo = new ClientProductRepository();
            
            LoadData();
        }

        private void LoadData()
        {
            _products = _productRepo.GetAll();
            _clients = _clientRepo.GetAll();
            
            cmbProducts.DataSource = _products;
            cmbProducts.DisplayMember = "DisplayName"; 
            cmbProducts.ValueMember = "Id";

            cmbClients.DataSource = _clients;
            cmbClients.DisplayMember = "Nome";
            cmbClients.ValueMember = "Id";
            cmbClients.SelectedIndex = -1;

            cmbType.SelectedIndex = 0; // Master Stock Default
            cmbClients.Enabled = false;

            cmbType.SelectedIndexChanged += (s, e) => 
            {
                cmbClients.Enabled = cmbType.SelectedIndex == 1; // 1 = Client Consignment
                UpdateCurrentStockDisplay();
            };

            cmbProducts.SelectedIndexChanged += (s, e) => UpdateCurrentStockDisplay();
            cmbClients.SelectedIndexChanged += (s, e) => UpdateCurrentStockDisplay();
        }

        private void UpdateCurrentStockDisplay()
        {
            if (cmbProducts.SelectedItem == null) return;
            var product = (Product)cmbProducts.SelectedItem;

            if (cmbType.SelectedIndex == 0) // Master
            {
                lblCurrent.Text = $"Estoque Mestre Atual: {product.EstoqueMestre}";
            }
            else // Client
            {
                if (cmbClients.SelectedItem == null) 
                {
                    lblCurrent.Text = "Selecione um Cliente.";
                    return;
                }
                var client = (Client)cmbClients.SelectedItem;
                var link = _cpRepo.Get(client.Id, product.Id);
                int qty = link?.QuantidadeNoCliente ?? 0;
                lblCurrent.Text = $"Consignado no Cliente Atual: {qty}";
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (cmbProducts.SelectedItem == null) return;
            var product = (Product)cmbProducts.SelectedItem;
            
            if (!int.TryParse(txtNewQty.Text, out int newQty))
            {
                MessageBox.Show("Quantidade inválida.");
                return;
            }

            if (cmbType.SelectedIndex == 0) // Master
            {
                // We don't have a specific "SetStock" in repo, but we have Update.
                // Let's rely on simple update for now or add a Set method.
                // Since I have the object, I can update property and save.
                product.EstoqueMestre = newQty;
                _productRepo.Update(product);
                MessageBox.Show("Estoque Mestre Atualizado.");
            }
            else // Client
            {
                if (cmbClients.SelectedItem == null) 
                {
                    MessageBox.Show("Selecione um Cliente.");
                    return;
                }
                var client = (Client)cmbClients.SelectedItem;
                
                _cpRepo.Save(new ClientProduct 
                { 
                    ClienteId = client.Id, 
                    ProdutoId = product.Id, 
                    QuantidadeNoCliente = newQty 
                });
                MessageBox.Show("Estoque Consignado Atualizado.");
            }
            UpdateCurrentStockDisplay();
        }
    }
}
