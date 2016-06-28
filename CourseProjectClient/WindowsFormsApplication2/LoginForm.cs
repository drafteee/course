using System;
using System.Windows.Forms;

namespace WindowsFormsApplication2
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }
        public string IP { get; set; }
        public string Port { get; set; }
        private void btnOk_Click(object sender, EventArgs e)
        {
            if ((!String.IsNullOrEmpty(tbIP.Text))||(!String.IsNullOrEmpty(tbPort.Text)))
            {
                this.IP = tbIP.Text;
                this.Port = tbPort.Text;
                this.Close();
            }
            else
                MessageBox.Show("Input your Name!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
