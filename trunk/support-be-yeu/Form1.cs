using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace support_be_yeu
{
    public partial class Form1 : Form
    {
        private List<TaxIDProcessor.TaxIDInfo> _result;

        public Form1()
        {
            InitializeComponent();

            lblMsg.Text = "";

            backgroundWorker1.DoWork += backgroundWorker1_DoWork;
            backgroundWorker1.RunWorkerCompleted += backgroundWorker1_RunWorkerCompleted;
        }

        void backgroundWorker1_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            _result = e.Result as List<TaxIDProcessor.TaxIDInfo>;

            txtOutput.Text = _result.Aggregate("", (current, s) => string.Format("{0}{1}\r\n", current, s));

            lblMsg.Text = "Hoàn tất.";
        }

        void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            var input = txtInput.Text.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            var processor = new TaxIDProcessor.TaxIDProcessor();

            e.Result = processor.GetTaxIDs(input);
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            lblMsg.Text = "Đang xứ lý ...";

            backgroundWorker1.RunWorkerAsync();
        }

        private void btnCopyAll_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(txtOutput.Text);
        }

        private void btnCopyMST_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(_result.Aggregate("", (current, s) => string.Format("{0}{1}\r\n", current, s.MaSoThue)));
        }
    }
}
