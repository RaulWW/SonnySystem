using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Printing;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using SonnySystem.Data;
using SonnySystem.Models;

namespace SonnySystem.Forms
{
    public partial class ClientForm : Form
    {
        private ClientRepository _repository;
        private BindingList<Client> _clients;
        private Client _currentClient;

        public ClientForm()
        {
            InitializeComponent();
            _repository = new ClientRepository();
            
            // Wire up events
            dgvClients.SelectionChanged += DgvClients_SelectionChanged;
            btnNew.Click += BtnNew_Click;
            btnSave.Click += BtnSave_Click;
            btnSearchCep.Click += async (s, e) => await SearchCep();
            btnDeclaration.Click += BtnDeclaration_Click;

            LoadData();
        }

        private void LoadData()
        {
            var data = _repository.GetAll();
            _clients = new BindingList<Client>(data);
            dgvClients.DataSource = _clients;
            
            if (dgvClients.Rows.Count > 0)
                dgvClients.Rows[0].Selected = true;
        }

        private void DgvClients_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvClients.SelectedRows.Count > 0)
            {
                _currentClient = (Client)dgvClients.SelectedRows[0].DataBoundItem;
                PopulateFields(_currentClient);
            }
        }

        private void PopulateFields(Client client)
        {
            txtNome.Text = client.Nome;
            txtCEP.Text = client.CEP;
            txtEndereco.Text = client.Endereco;
            txtNumero.Text = client.Numero;
            txtBairro.Text = client.Bairro;
            txtCidade.Text = client.Cidade;
            txtComissao.Text = client.ComissaoPercentual.ToString();
        }

        private void BtnNew_Click(object sender, EventArgs e)
        {
            _currentClient = new Client();
            dgvClients.ClearSelection();
            ClearFields();
            txtNome.Focus();
        }

        private void ClearFields()
        {
            txtNome.Text = "";
            txtCEP.Text = "";
            txtEndereco.Text = "";
            txtNumero.Text = "";
            txtBairro.Text = "";
            txtCidade.Text = "";
            txtComissao.Text = "0";
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (_currentClient == null) _currentClient = new Client();

            _currentClient.Nome = txtNome.Text;
            _currentClient.CEP = txtCEP.Text;
            _currentClient.Endereco = txtEndereco.Text;
            _currentClient.Numero = txtNumero.Text;
            _currentClient.Bairro = txtBairro.Text;
            _currentClient.Cidade = txtCidade.Text;
            
            decimal comm;
            if (decimal.TryParse(txtComissao.Text, out comm))
                _currentClient.ComissaoPercentual = comm;
            else
                _currentClient.ComissaoPercentual = 0;

            if (_currentClient.Id == 0)
            {
                _repository.Add(_currentClient);
                _clients.Add(_currentClient); // Add to Grid directly? Or reload. Reload is safer for ID.
                LoadData(); 
            }
            else
            {
                _repository.Update(_currentClient);
                dgvClients.Refresh();
            }
            
            lblMsg.Text = "Salvo com sucesso!";
            var t = Task.Delay(2000).ContinueWith(_ => Invoke(new Action(() => lblMsg.Text = "")));
        }

        private async Task SearchCep()
        {
            var cep = txtCEP.Text.Replace("-", "").Trim();
            if (cep.Length != 8)
            {
                MessageBox.Show("CEP inválido");
                return;
            }

            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetStringAsync($"https://viacep.com.br/ws/{cep}/json/");
                    var data = JsonSerializer.Deserialize<ViaCepResponse>(response);

                    if (data != null && string.IsNullOrEmpty(data.erro))
                    {
                        txtEndereco.Text = data.logradouro;
                        txtBairro.Text = data.bairro;
                        txtCidade.Text = data.localidade;
                        txtNumero.Focus();
                    }
                    else
                    {
                        MessageBox.Show("CEP não encontrado.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao buscar CEP: " + ex.Message);
            }
        }

        private void BtnDeclaration_Click(object sender, EventArgs e)
        {
             if (_currentClient == null || string.IsNullOrEmpty(_currentClient.Nome))
            {
                MessageBox.Show("Selecione ou cadastre um cliente primeiro.");
                return;
            }

            PrintDeclaration(_currentClient);
        }

         private void PrintDeclaration(Client client)
        {
            PrintDocument pd = new PrintDocument();
            pd.PrintPage += (s, ev) => 
            {
                var font = new Font("Arial", 12);
                var headerFont = new Font("Arial", 16, FontStyle.Bold);
                var boldFont = new Font("Arial", 12, FontStyle.Bold);
                
                float x = 50;
                float y = 50;
                
                ev.Graphics.DrawString("DECLARAÇÃO DE CONTEÚDO", headerFont, Brushes.Black, 250, y);
                y += 60;
                
                // Remetente
                ev.Graphics.DrawString("REMETENTE:", boldFont, Brushes.Black, x, y);
                y += 25;
                ev.Graphics.DrawString("Nome: SONNY SYSTEM", font, Brushes.Black, x, y);
                y += 25;
                ev.Graphics.DrawString("Endereço: Rua da Empresa, 123 - Centro - Cidade/UF", font, Brushes.Black, x, y); // Placeholder
                y += 40;

                // Destinatário
                ev.Graphics.DrawString("DESTINATÁRIO:", boldFont, Brushes.Black, x, y);
                y += 25;
                ev.Graphics.DrawString($"Nome: {client.Nome}", font, Brushes.Black, x, y);
                y += 25;
                ev.Graphics.DrawString($"Endereço: {client.Endereco}, {client.Numero}", font, Brushes.Black, x, y);
                y += 25;
                ev.Graphics.DrawString($"Bairro: {client.Bairro} - Cidade: {client.Cidade}", font, Brushes.Black, x, y);
                y += 25;
                ev.Graphics.DrawString($"CEP: {client.CEP}", font, Brushes.Black, x, y);
                
                y += 50;
                ev.Graphics.DrawString("IDENTIFICAÇÃO DOS BENS:", boldFont, Brushes.Black, x, y);
                y += 30;
                
                // Table Header
                ev.Graphics.DrawString("ITEM  |  CONTEÚDO  |  QTD  |  VALOR", boldFont, Brushes.Black, x, y);
                y += 25;
                ev.Graphics.DrawString("1     |  ACESSÓRIOS DE CELULAR  |  -  |  -", font, Brushes.Black, x, y);
                
                y += 100;
                ev.Graphics.DrawString("DECLARAÇÃO:", boldFont, Brushes.Black, x, y);
                y += 25;
                
                string legalText = "Declaro que não me enquadro no conceito de contribuinte previsto no art. 4º da Lei Complementar nº 87/1996, uma vez que não realizo, com habitualidade ou em volume que caracterize intuito comercial, operações de circulação de mercadoria, ainda que se iniciem no exterior, ou estou dispensado da emissão da nota fiscal por força da legislação tributária vigente, responsabilizando-me, nos termos da lei e a quem de direito, pelas informações prestadas.";
                
                RectangleF rect = new RectangleF(x, y, 700, 150);
                ev.Graphics.DrawString(legalText, new Font("Arial", 10), Brushes.Black, rect);
                
                y += 150;
                ev.Graphics.DrawString("_________________________________________________", font, Brushes.Black, x, y);
                y += 25;
                ev.Graphics.DrawString("Assinatura do Declarante/Remetente", font, Brushes.Black, x, y);

            };
            
            PrintDialog dialog = new PrintDialog();
            dialog.Document = pd;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                pd.Print();
            }
        }
    }
    
    // Helper for ViaCEP
    public class ViaCepResponse
    {
        public string cep { get; set; }
        public string logradouro { get; set; }
        public string complemento { get; set; }
        public string bairro { get; set; }
        public string localidade { get; set; }
        public string uf { get; set; }
        public string erro { get; set; }
    }
}
