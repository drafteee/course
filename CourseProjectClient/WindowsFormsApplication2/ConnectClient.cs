using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CourseProjectClient
{
    class ConnectClient
    {
        private IPEndPoint IPConnect;
        private IPEndPoint IPConnectTCP;

        private byte[] sizeWindow = new byte[4];
        private WorkWithImage workImage = new WorkWithImage();
        private WorkWithMouse workMouse = new WorkWithMouse();
        private AdditionalCalculations calcul = new AdditionalCalculations();
        public void Connect(MainForm form, string ip, string port,ClientRealization client)
        {
            IPConnect = new IPEndPoint(IPAddress.Parse(ip), Convert.ToInt32(port));
            IPConnectTCP = new IPEndPoint(IPAddress.Parse(ip), Convert.ToInt32(port));
            client.socketSendUDP = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            bool IsConnectTCP = false;
            client.flagConnectToClient = true;
            Task t_startSendPackage = Task.Run(async () =>
            {
                sizeWindow[0] = calcul.GetFactor(Screen.PrimaryScreen.WorkingArea.Width);
                sizeWindow[1] = calcul.summand;
                sizeWindow[2] = calcul.GetFactor(Screen.PrimaryScreen.WorkingArea.Height);
                sizeWindow[3] = calcul.summand;
                client.socketSendUDP.SendTo(sizeWindow, IPConnect);
                client.EP_IPConnectUDP = (EndPoint)IPConnect;
                try {
                    client.socketSendUDP.ReceiveTimeout = 100000;
                    client.socketSendUDP.ReceiveFrom(sizeWindow, ref client.EP_IPConnectUDP);
                }
                catch (Exception exp)
                {
                    MessageBox.Show(exp.ToString());
                    return;
                }
                while (client.flagConnectToClient)
                {
                    workImage.SendImageUDP(client.socketSendUDP, client.EP_IPConnectUDP);
                    await Task.Delay(Constants.ELAPSED_TIME);
                }
                client.flagConnectToClient = true;
                
            });
            client.socketSendTCP = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            Task t_startRecievePackage = Task.Run(() =>
            {
                while (!IsConnectTCP)
                {
                    try
                    {
                        client.socketSendTCP.Connect(IPConnectTCP);
                        IsConnectTCP = true;
                    }//один раз пытаться подключиться
                    catch (Exception exp) {
                        MessageBox.Show(exp.ToString());
                        break;
                    }
                }
                if (IsConnectTCP)
                {
                    client.socketSendTCP.Send(new byte[] { 1 });
                    while (client.flagConnectToClient)
                    {
                        workMouse.RecieveMouseCoordinates(client.socketSendTCP, client.flagConnectToClient);
                    }
                }
            });
        }
    }
}
