using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace AsyncApp
{
    public partial class Form1 : Form
    {
        private test5Entities m_context;
        private BindingSource m_bindingSource;

        public Form1()
        {
            InitializeComponent();
            Load += Form1_Load;            
        }       

        void Form1_Load(object sender, EventArgs e)
        {
            m_context = new test5Entities();
            m_bindingSource = new BindingSource();
            LoadData();
        }

        private async void LoadData()
        {
            m_bindingSource.DataSource = await m_context.Osoba.ToListAsync();
            lbOsoby.DataSource = m_bindingSource;
            tbJmeno.DataBindings.Add("Text", m_bindingSource, "Jmeno", false, DataSourceUpdateMode.OnPropertyChanged);
            tbPrijmeni.DataBindings.Add("Text", m_bindingSource, "Prijmeni", false, DataSourceUpdateMode.OnPropertyChanged);
            dtpNarozen.DataBindings.Add("Value", m_bindingSource, "DatumNarozeni", true, DataSourceUpdateMode.OnPropertyChanged);
            btnSave.Enabled = true;
            btnExport.Enabled = true;
        }

        private async void SaveData()
        {
            btnSave.Enabled = false;
            btnExport.Enabled = false;
            await m_context.SaveChangesAsync();
            btnSave.Enabled = true;
            btnExport.Enabled = true;
        }

        private async void ExportToXmlAsync(string path)
        {
            btnExport.Enabled = false;
            await Task.Run(() => ExportToXml(path));
            btnExport.Enabled = true;
        }

        private void ExportToXml(string path)
        {
            var data = m_context.Osoba.ToList();
            XDocument doc = new XDocument(new XElement("Osoby"));
            data.ForEach(o => doc.Root.Add(
                new XElement("Osoba",
                    new XElement("Jmeno", o.Jmeno ?? ""),
                    new XElement("Prijmeni", o.Prijmeni ?? ""),
                    new XElement("Narozen", o.DatumNarozeni == null ? "" : ((DateTime)o.DatumNarozeni).ToShortDateString()))));

            doc.Save(path);
        }

        private void ExportData()
        {
            var dialog = new SaveFileDialog { DefaultExt = ".xml", Filter = "Xml Files (*.xml)|*.xml" };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                ExportToXmlAsync(dialog.FileName);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveData();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            ExportData();
        }        
    }
}
