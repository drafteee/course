using System.Net;
using System.Net.Sockets;

namespace CourseProjectClient
{
    public class ClientRealization
    {
        public bool flagConnectToClient = false;

        public Socket socketSendTCP = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        public Socket socketSendUDP = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        public EndPoint EP_IPConnectUDP;

        private WaitClient waitClient = new WaitClient();
        private ConnectClient connectClient = new ConnectClient();

        //rdp -скриншоты
        //
        public void Connect(MainForm form,string ip,string port)
        {
            connectClient.Connect(form, ip, port,this); 
        }
        public void Wait(MainForm form)
        {
            waitClient.Wait(form);
        }
        public void Disconnect()
        {
            if (flagConnectToClient)
            {
                socketSendUDP.SendTo(new byte[] { (byte)Constants.STATUS_SEND.STOP_SEND }, EP_IPConnectUDP);
                flagConnectToClient = false;

                socketSendUDP.Shutdown(SocketShutdown.Both);
                socketSendTCP.Shutdown(SocketShutdown.Both);
                socketSendUDP.Close();
                socketSendTCP.Close();
            }
        }
    }
}
