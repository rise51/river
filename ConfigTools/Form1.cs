using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ConfigTools
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnWork_Click(object sender, EventArgs e)
        {
            txtOutput.Text = MakeConfig(txtInput.Text);
        }

        private string MakeConfig(string input)
        {
            string  tempUrlEncode = System.Web.HttpUtility.UrlEncode(input, System.Text.Encoding.UTF8);
            int tempIntahpplid = tempUrlEncode.IndexOf("ahpplid");
            string tempahpplid = tempUrlEncode.Substring(tempUrlEncode.Substring(0, tempIntahpplid).IndexOf("%26"), tempUrlEncode.Substring(tempIntahpplid).IndexOf("%26"));
            int tempIntahpsign = tempUrlEncode.IndexOf("ahpsign");
            string tempahpsign = tempUrlEncode.Substring(tempUrlEncode.Substring(0, tempIntahpsign).IndexOf("%26"), tempUrlEncode.Substring(tempIntahpsign).IndexOf("%26"));
            string tempreferer = tempUrlEncode.Substring(tempUrlEncode.IndexOf("cur%3d"), tempUrlEncode.IndexOf("%26scene_type"));
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("< add key = \"{0}\" value=\"{1}\" />", "mda_pv_init", tempUrlEncode);
            sb.AppendLine();
            sb.AppendFormat("< add key = \"{0}\" value=\"{1}\" />", "mda_pv_init_ahpplid", tempahpplid);
            sb.AppendLine();
            sb.AppendFormat("< add key = \"{0}\" value=\"{1}\" />", "mda_pv_init_ahpsign", tempahpsign);
            sb.AppendLine();
            sb.AppendFormat("< add key = \"{0}\" value=\"{1}\" />", "mda_pv_init_referer", tempreferer);
            sb.AppendLine();
            return sb.ToString();
        }
    }
}
