using System;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CourseProjectClient
{
    class WaitClient
    {
        private WorkWithImage workImage = new WorkWithImage();
        public void Wait(MainForm form)
        {
            IPEndPoint IPEP_ConnectUDP = new IPEndPoint(IPAddress.Any, Constants.PORT);
            IPEndPoint IPEP_ConnectTCP = new IPEndPoint(IPAddress.Any, Constants.PORT);
            EndPoint EP_ConnectUDP = (EndPoint)IPEP_ConnectUDP;
            EndPoint EP_ConnectTCP = (EndPoint)IPEP_ConnectTCP;

            Socket socketTCP = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Socket socketUDP = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            form.additionalSocketTCP = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            byte[] sizeRecieveScreen = new byte[4];

            Task t_RecieveFrames = Task.Run(() =>
            {
                socketUDP.Bind(EP_ConnectUDP);
                try {
                    //socketUDP.ReceiveTimeout = 100000;
                    socketUDP.ReceiveFrom(sizeRecieveScreen, ref EP_ConnectUDP);
                    form.pbScreen.Image = new Bitmap(sizeRecieveScreen[0] * Constants.MAX_BYTE + sizeRecieveScreen[1], sizeRecieveScreen[2] * Constants.MAX_BYTE + sizeRecieveScreen[3]);
                    Bitmap screen = new Bitmap(sizeRecieveScreen[0] * Constants.MAX_BYTE + sizeRecieveScreen[1], sizeRecieveScreen[2] * Constants.MAX_BYTE + sizeRecieveScreen[3]);
                    socketUDP.SendTo(sizeRecieveScreen, EP_ConnectUDP);
                    while (form.IsConnect) { workImage.RecieveFrames(socketUDP, form, EP_ConnectUDP, ref screen); }
                }catch(Exception exp)
                {
                    MessageBox.Show(exp.ToString());
                }
                //additionalSocketTCP.Shutdown(SocketShutdown.Both);
                //socketUDP.Shutdown(SocketShutdown.Both);
                //additionalSocketTCP.Close();
                //socketUDP.Close();
                //socketTCP.Close();
            });

            Task t_SendMousePosition = Task.Run(() =>
            {
                socketTCP.Bind(EP_ConnectTCP);
                socketTCP.Listen(10);
                form.additionalSocketTCP = socketTCP.Accept();
                form.additionalSocketTCP.Receive(new byte[1]);

                form.IsConnect = true;
            });
        }

    }
}
