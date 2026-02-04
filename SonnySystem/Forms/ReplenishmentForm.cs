using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Windows.Forms;
using SonnySystem.Data;
using SonnySystem.Models;
using SonnySystem.ViewModels;

namespace SonnySystem.Forms
{
    public partial class ReplenishmentForm : Form
    {
        private ClientRepository _clientRepo;
        private ProductRepository _productRepo;
        private ClientProductRepository _cpRepo;
        private MovementHistoryRepository _moveRepo;
        
        private BindingList<ReplenishmentViewModel> _items = new(); 
        private List<Client> _clients = new();

        public ReplenishmentForm()
        {
            InitializeComponent();
            _clientRepo = new ClientRepository();
            _productRepo = new ProductRepository();
            _cpRepo = new ClientProductRepository();
            _moveRepo = new MovementHistoryRepository();
            
            LoadClients();
            InitializeGrid();
        }

        private void LoadClients()
        {
            _clients = _clientRepo.GetAll();
            cmbClients.DataSource = _clients;
            cmbClients.DisplayMember = "Nome";
            cmbClients.ValueMember = "Id";
            cmbClients.SelectedIndex = -1;
            
            cmbClients.SelectedIndexChanged += (s, e) => LoadProducts();
        }

        private void InitializeGrid()
        {
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.AllowUserToAddRows = false;
            
            var colProduct = new DataGridViewTextBoxColumn { HeaderText = "Produto", DataPropertyName = "DisplayName", ReadOnly = true, Width = 200 };
            var colIdeal = new DataGridViewTextBoxColumn { HeaderText = "Estoque Ideal", DataPropertyName = "IdealStock", ReadOnly = true, Width = 80 };
            var colQty = new DataGridViewTextBoxColumn { HeaderText = "Qtd Reposta", DataPropertyName = "RepositionQty", Width = 80 }; // Editable
            var colMaster = new DataGridViewTextBoxColumn { HeaderText = "Estoque Mestre", DataPropertyName = "MasterStock", ReadOnly = true, Width = 80 };
            var colTotal = new DataGridViewTextBoxColumn { HeaderText = "Total", DataPropertyName = "TotalValue", ReadOnly = true, DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" } };

            dataGridView1.Columns.Add(colProduct);
            dataGridView1.Columns.Add(colIdeal);
            dataGridView1.Columns.Add(colQty);
            dataGridView1.Columns.Add(colMaster);
            dataGridView1.Columns.Add(colTotal);

            dataGridView1.CellEndEdit += DataGridView1_CellEndEdit;
            dataGridView1.EditingControlShowing += DataGridView1_EditingControlShowing;
        }

        private void LoadProducts()
        {
            if (cmbClients.SelectedItem == null) return;
            var clientId = (int)cmbClients.SelectedValue;
            
            // Get all products + consignment info
            var rawItems = _cpRepo.GetConsignmentItems(clientId);
            var productDetails = _productRepo.GetAll().ToDictionary(p => p.Id);

            _items = new BindingList<ReplenishmentViewModel>();
            
            foreach (var item in rawItems)
            {
                int masterStock = productDetails.ContainsKey(item.ProductId) ? productDetails[item.ProductId].EstoqueMestre : 0;
                
                _items.Add(new ReplenishmentViewModel 
                { 
                    ProductId = item.ProductId, 
                    DisplayName = item.DisplayName, 
                    UnitPrice = item.PrecoVenda,
                    IdealStock = item.QtdAtualLa, // From DB (Consigned)
                    MasterStock = masterStock,
                    RepositionQty = 0
                });
            }
            
            dataGridView1.DataSource = _items;
            CalculateTotals();
        }

        private void DataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (e.Control is TextBox tb)
            {
                tb.KeyPress -= Tb_KeyPress;
                tb.KeyPress += Tb_KeyPress;
            }
        }

        private void Tb_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) e.Handled = true;
        }

        private void DataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
             if (e.RowIndex < 0) return;
             dataGridView1.Refresh();
             CalculateTotals();
        }

        private void CalculateTotals()
        {
             if (cmbClients.SelectedItem == null) return;
            var client = (Client)cmbClients.SelectedItem;

            decimal totalGross = _items != null ? _items.Sum(i => i.TotalValue) : 0;
            decimal commission = totalGross * client.ComissaoPercentual; 
            decimal totalNet = totalGross - commission;

            lblTotalGross.Text = totalGross.ToString("C2");
            lblCommission.Text = commission.ToString("C2");
            lblTotalNet.Text = totalNet.ToString("C2");
        }

        private void btnFinish_Click(object sender, EventArgs e)
        {
            if (cmbClients.SelectedItem == null) { MessageBox.Show("Selecione um cliente."); return; }
            var client = (Client)cmbClients.SelectedItem;
            
            var changedItems = _items.Where(i => i.RepositionQty > 0).ToList();
            if (!changedItems.Any()) { MessageBox.Show("Nenhuma reposição informada."); return; }

            bool isMixIncrease = chkMixIncrease.Checked;
            DateTime timestamp = DateTime.Now;
            
            // Transaction? SQLite is file based, simple sequence is usually fine for single user.
            foreach (var item in changedItems)
            {
                // 1. Deduct from Master Stock
                _productRepo.DeductStock(item.ProductId, item.RepositionQty);

                // 2. Update Consignment Link?
                // Rule: "QuantidadeConsignada NÃO MUDA (pois eu repus o que saiu), exceto se eu marcar um 'Checkbox'"
                // If Mix Increase: New Consignment = Old Consignment + Reposition Qty
                // If Sale (Normal): Consignment = Old Consignment (No Change, just restored)
                if (isMixIncrease)
                {
                    int newConsigned = item.IdealStock + item.RepositionQty;
                    _cpRepo.Save(new ClientProduct 
                    { 
                        ClienteId = client.Id, 
                        ProdutoId = item.ProductId, 
                        QuantidadeNoCliente = newConsigned 
                    });
                }
                // Else: Do nothing to ClientProduct. Matches "Venda Pronta Entrega" - replenishment restores the level.

                // 3. Log History
                _moveRepo.Add(new MovementHistory
                {
                    Data = timestamp,
                    ClienteId = client.Id,
                    ProdutoId = item.ProductId,
                    Quantidade = item.RepositionQty,
                    Tipo = isMixIncrease ? "Aumento Mix" : "Reposicao",
                    Valor = item.TotalValue
                });
            }
            
            PrintReceipt(client, changedItems, timestamp);
            
            decimal net = _items.Sum(i => i.TotalValue) * (1 - client.ComissaoPercentual);
            MessageBox.Show($"Visita Finalizada!\nValor Líquido a Receber: {net:C2}");
            
            this.DialogResult = DialogResult.OK;
            Close();
        }

        // ... Printing Logic (Same as before but adapted for view model)
        private void PrintReceipt(Client client, List<ReplenishmentViewModel> items, DateTime date)
        {
             PrintDocument pd = new PrintDocument();
            pd.PrintPage += (s, ev) => 
            {
                var font = new Font("Courier New", 9); 
                var fontBold = new Font("Courier New", 9, FontStyle.Bold);
                float sectionHeight = ev.PageBounds.Height / 3;
                
                for (int i = 0; i < 3; i++)
                {
                    float yOffset = i * sectionHeight;
                    DrawReceiptSection(ev.Graphics, client, items, date, yOffset, font, fontBold);
                     if (i < 2)
                    {
                        float lineY = yOffset + sectionHeight - 20;
                        using (Pen dashed = new Pen(Color.Black)) { dashed.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash; ev.Graphics.DrawLine(dashed, 0, lineY, ev.PageBounds.Width, lineY); }
                    }
                }
            };
            
             PrintDialog dialog = new PrintDialog();
             dialog.Document = pd;
             if (dialog.ShowDialog() == DialogResult.OK) pd.Print();
        }

        private void DrawReceiptSection(Graphics g, Client client, List<ReplenishmentViewModel> items, DateTime date, float startY, Font font, Font fontBold)
        {
            float y = startY + 20; float left = 20;
            g.DrawString("SONNY SYSTEM - Venda Pronta Entrega", fontBold, Brushes.Black, left, y); y += 20;
            g.DrawString($"CLIENTE: {client.Nome}", font, Brushes.Black, left, y); y += 15;
            g.DrawString($"DATA: {date:dd/MM/yyyy HH:mm}", font, Brushes.Black, left, y); y += 20;
            g.DrawString("--------------------------------------------------", font, Brushes.Black, left, y); y += 15;
            g.DrawString(String.Format("{0,-20} {1,5} {2,10}", "PRODUTO", "QTD", "TOTAL"), fontBold, Brushes.Black, left, y); y += 15;

            decimal total = items.Sum(i => i.TotalValue);
            foreach (var item in items)
            {
                string name = item.DisplayName.Length > 18 ? item.DisplayName.Substring(0, 18) + "." : item.DisplayName;
                g.DrawString(String.Format("{0,-20} {1,5} {2,10:N2}", name, item.RepositionQty, item.TotalValue), font, Brushes.Black, left, y); y += 15;
            }
            g.DrawString("--------------------------------------------------", font, Brushes.Black, left, y); y += 15;
            
            decimal comm = total * client.ComissaoPercentual;
            decimal net = total - comm;
            g.DrawString($"TOTAL BRUTO:   {total,10:C2}", fontBold, Brushes.Black, left, y); y += 15;
            g.DrawString($"COMISSÃO ({client.ComissaoPercentual*100:0}%): {comm,10:C2}", font, Brushes.Black, left, y); y += 15;
            g.DrawString($"TOTAL LÍQUIDO: {net,10:C2}", fontBold, Brushes.Black, left, y);
        }
    }
}
