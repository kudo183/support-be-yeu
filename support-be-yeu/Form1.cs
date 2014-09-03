using System;
using System.Linq;
using System.Windows.Forms;

namespace support_be_yeu
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            lblMsg.Text = "";

            backgroundWorker1.DoWork += backgroundWorker1_DoWork;
            backgroundWorker1.RunWorkerCompleted += backgroundWorker1_RunWorkerCompleted;
        }

        void backgroundWorker1_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            txtOutput.Text = e.Result.ToString();
            
            lblMsg.Text = "Hoàn tất.";
        }

        void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            var input = txtInput.Text.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            var processor = new TaxIDProcessor.TaxIDProcessor();

            e.Result = processor.GetTaxIDs(input).Aggregate("", (current, s) => string.Format("{0}{1}\r\n", current, s));
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            lblMsg.Text = "Đang xứ lý ...";
         
            backgroundWorker1.RunWorkerAsync();
        }

        private void btnCopyToClipboard_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(txtOutput.Text);
        }
    }
}
