using System;
using System.Windows.Forms;
using SonnySystem.Data;
using SonnySystem.Models;
using System.ComponentModel;

namespace SonnySystem.Forms
{
    public partial class ProductForm : Form
    {
        private ProductRepository _repository;
        private BindingList<Product> _products;

        public ProductForm()
        {
            InitializeComponent();
            _repository = new ProductRepository();
            LoadData();
        }

        private void LoadData()
        {
            var data = _repository.GetAll();
            _products = new BindingList<Product>(data);
            dataGridView1.DataSource = _products;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // Simple save logic: iterate and update/add
            // For a real app, track changes. Here, we'll just re-add or update all?
            // Actually, simpler: The user edits in the grid. We can have a "Save" button that iterates rows.
            
            foreach (var product in _products)
            {
                if (product.Id == 0)
                {
                    _repository.Add(product);
                }
                else
                {
                    _repository.Update(product);
                }
            }
            MessageBox.Show("Produtos salvos com sucesso!");
            LoadData(); // Refresh IDs
        }
    }
}
