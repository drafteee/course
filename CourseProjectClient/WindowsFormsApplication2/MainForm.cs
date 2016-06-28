using System;
using System.Drawing;
using System.Windows.Forms;
using System.Net.Sockets;
using WindowsFormsApplication2;

namespace CourseProjectClient
{
    public partial class MainForm : Form
    {
        public bool IsConnect = false,flagOfMoveImage = false, flagOfLeftClickImage = false, flagOfDoubleClickImage = false, flagOfRightClickImage = false;
        public Socket additionalSocketTCP;

        private ClientRealization clientRealization;
        private WorkWithMouse mouse = new WorkWithMouse();

        public MainForm()
        {
            InitializeComponent();
        }
        private void btnLogin_Click(object sender, EventArgs e)
        {
            LoginForm loginDialog = new LoginForm();
            loginDialog.ShowDialog(this);

            if (!String.IsNullOrEmpty(loginDialog.IP))
            {
                clientRealization = new ClientRealization();
                btnDisconnect.Enabled = true;
                clientRealization.Connect(this,loginDialog.IP,loginDialog.Port);
                btnConnect.Enabled = false;
                btnLogin.Enabled = false;
            }
            
        }
        private void btnConnect_Click(object sender, EventArgs e)
        {
            clientRealization = new ClientRealization();
            clientRealization.Wait(this);
            btnDisconnect.Enabled = true;
            btnConnect.Enabled = false;
            btnLogin.Enabled = false;
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            IsConnect = false;
            clientRealization.Disconnect();
            btnDisconnect.Enabled = false;
            btnLogin.Enabled = true;
            btnConnect.Enabled = true;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (IsConnect)
                clientRealization.Disconnect();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            if(IsConnect)
                clientRealization.Disconnect();
        }
        private void pbScreen_MouseClick(object sender, MouseEventArgs e)
        {
            switch (e.Button){
                case MouseButtons.Left:
                    {
                        flagOfLeftClickImage = true;
                        break;
                    }
                case MouseButtons.Right:
                    {
                        flagOfRightClickImage = true;
                        break;
                    }
            }
        }
        private void pbScreen_DoubleClick(object sender, EventArgs e)
        {
            flagOfDoubleClickImage = true;
            flagOfLeftClickImage = false;
        }
        private void pictureBox1_MouseMove_1(object sender, MouseEventArgs e)
        {
            flagOfMoveImage = true;
            Point mousePosition = new Point { X = e.X, Y = e.Y };
            if (IsConnect)
            {
                mouse.SendActionMouse(additionalSocketTCP, mousePosition, this, new bool[4] { flagOfLeftClickImage,flagOfDoubleClickImage,flagOfRightClickImage,flagOfMoveImage});
                flagOfLeftClickImage = false;
                flagOfDoubleClickImage = false;
                flagOfRightClickImage = false;
            }
        }
    }
}