using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;

namespace CourseProjectClient
{
    class WorkWithImage
    {
        private bool isSameImages = false;
        private Image additionalScreenshot = null;
        private Bitmap copyReallyScreen = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
        private AdditionalCalculations calcul = new AdditionalCalculations();

        private Bitmap ImageFromScreen()
        {

            Bitmap localBmp = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            using (Graphics g = Graphics.FromImage(localBmp))
            {
                try
                {
                    g.CopyFromScreen(0, 0, 0, 0, localBmp.Size);
                }
                catch (Exception ex) { return copyReallyScreen; }
            }
            copyReallyScreen = localBmp;
            return localBmp;
        }

        private void SendUdpPackage(int positionY, MemoryStream mstreamSendArray, EndPoint EP, int sizeOfHeight, Socket socket)
        {
            Rectangle rectFromScreenshot = new Rectangle(new Point(0, positionY), new Size(Screen.PrimaryScreen.WorkingArea.Width, sizeOfHeight));
            Bitmap additionalBMP = new Bitmap(Screen.PrimaryScreen.WorkingArea.Width, sizeOfHeight);

            Graphics g = Graphics.FromImage(additionalBMP);

            g.DrawImage(additionalScreenshot, 0, 0, rectFromScreenshot, GraphicsUnit.Pixel);
            additionalBMP.Save(mstreamSendArray, ImageFormat.Jpeg);
            byte[] sendArray = new byte[mstreamSendArray.Length + Constants.BEGIN_ARRAY_SEND];
            sendArray[0] = (byte)Constants.STATUS_SEND.START_SEND;
            sendArray[1] = calcul.GetFactor((int)mstreamSendArray.Length);
            sendArray[2] = calcul.summand;
            sendArray[3] = calcul.GetFactor(positionY);
            sendArray[4] = calcul.summand;
            Array.Copy(mstreamSendArray.ToArray(), 0, sendArray, Constants.BEGIN_ARRAY_SEND, mstreamSendArray.Length);
            try
            {
                socket.SendTo(sendArray, EP);
            }
            catch (Exception ex) { }
        }
        public void SendImageUDP(Socket socket, EndPoint EP_IPConnectUDP)
        {
            try
            {
                Image screenshot = ImageFromScreen();
                additionalScreenshot = (Bitmap)screenshot.Clone();
                isSameImages = calcul.CheckSendImage(screenshot);
                screenshot.Dispose();
            }
            catch (Exception ex)
            {
                isSameImages = calcul.CheckSendImage(additionalScreenshot);
            }
            if (isSameImages)
            {
                for (int i = 0; i < Screen.PrimaryScreen.WorkingArea.Height; i += Constants.HEIGHT_SEND_IMAGE)
                {
                    MemoryStream mstreamSendArray = new MemoryStream();

                    if (i + Constants.HEIGHT_SEND_IMAGE <= Screen.PrimaryScreen.WorkingArea.Height)
                    {
                        SendUdpPackage(i, mstreamSendArray, EP_IPConnectUDP, Constants.HEIGHT_SEND_IMAGE, socket);
                    }
                    else
                    {
                        SendUdpPackage(i, mstreamSendArray, EP_IPConnectUDP, Screen.PrimaryScreen.WorkingArea.Height - i, socket);
                    }
                }
            }
        }
        public bool RecieveFrames(Socket socket, MainForm form, EndPoint EP,ref Bitmap screen)
        {
            byte[] recieveArray = new byte[64000];

            socket.ReceiveFrom(recieveArray, ref EP);

            switch (recieveArray[0])
            {
                case 0:
                    {
                        int lengthRecieveArray = recieveArray[1] * Constants.MAX_BYTE + recieveArray[2];
                        int positionOfFrame = recieveArray[3] * Constants.MAX_BYTE + recieveArray[4];
                        Rectangle rectForScreen = new Rectangle(new Point(0, 0), new Size(Screen.PrimaryScreen.WorkingArea.Width, Constants.SIZE_HEIGHT_FRAME));

                        Array.Copy(recieveArray, Constants.BEGIN_RECIEVE_ARRAY, recieveArray, 0, lengthRecieveArray);
                        Array.Resize(ref recieveArray, lengthRecieveArray);

                        MemoryStream mstreamRecieveArray = new MemoryStream(recieveArray);
                        Graphics g = Graphics.FromImage(screen);
                        Bitmap partScreenBMP = (Bitmap)Image.FromStream(mstreamRecieveArray);

                        g.DrawImage(partScreenBMP, 0, positionOfFrame, rectForScreen, GraphicsUnit.Pixel);
                        form.pbScreen.Image = (Image)screen.Clone();

                        return true;
                    }
                case 1:
                    {
                        return false;
                    }
            }
            return false;
        }
    }
}
