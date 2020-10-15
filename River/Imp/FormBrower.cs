using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace River
{
    public partial class FormBrower : System.Windows.Forms.Form
    {
        public FormBrower()
        {
            InitializeComponent();
        }
        public void RequestWeb(string url)
        {
            this.webBrowser1.Navigate(url);
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
