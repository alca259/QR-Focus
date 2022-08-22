using QRFocus.Library;
using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace QRFocus.Demo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            openFileDialog1.RestoreDirectory = true;
            openFileDialog1.InitialDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Work");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var result = openFileDialog1.ShowDialog();
            if (result != DialogResult.OK) return;

            var ext = Path.GetExtension(openFileDialog1.SafeFileName);
            var decoder = new QRDecoder();
            var text = string.Empty;

            if (".pdf".Equals(ext, StringComparison.InvariantCultureIgnoreCase))
            {
                var decoderResult = decoder.PDFDecoder(openFileDialog1.FileName, 0);
                text = string.Join(Environment.NewLine, decoderResult.Select(s => s.Value));
            }
            else
            {
                var decoderResult = decoder.ImageDecoder(openFileDialog1.FileName);
                text = string.Join(Environment.NewLine, decoderResult.Select(s => s.Value));
            }

            label1.Text = text;
        }
    }
}
