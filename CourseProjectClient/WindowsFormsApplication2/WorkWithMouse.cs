using System;
using System.Drawing;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace CourseProjectClient
{
    class WorkWithMouse
    {
        private bool flagOfReallyPosition = false;
        private WinAPIMouse mouse = new WinAPIMouse();
        private AdditionalCalculations calcul = new AdditionalCalculations();

        [DllImport("user32.dll")]
        public static extern bool SetCursorPos(int X, int Y);

        public void SendActionMouse(Socket socket, Point mousePosition, MainForm form, bool[] arrayMouse)
        {
            byte[] mouseArray = new byte[11];

            mouseArray[0] = calcul.GetFactor(mousePosition.X);
            mouseArray[1] = calcul.summand;
            mouseArray[2] = calcul.GetFactor(mousePosition.Y);
            mouseArray[3] = calcul.summand;
            mouseArray[4] = calcul.GetFactor(form.pbScreen.Width);
            mouseArray[5] = calcul.summand;
            mouseArray[6] = calcul.GetFactor(form.pbScreen.Height);
            mouseArray[7] = calcul.summand;
            mouseArray[8] = (byte)(arrayMouse[0] ? 1 : 0);
            mouseArray[9] = (byte)(arrayMouse[1] ? 1 : 0);
            mouseArray[10] = (byte)(arrayMouse[2] ? 1 : 0);

            if (arrayMouse[3])
                socket.Send(mouseArray);
            else
                socket.Send(new byte[1] { 0 });

            mousePosition = new Point();
        }
        public void RecieveMouseCoordinates(Socket socket,bool flagConnectToClient)
        {
            byte[] mouseArray = new byte[11];
            if (flagConnectToClient)
            {
                try {
                    socket.Receive(mouseArray);

                    for (int i = 0; i < mouseArray.Length - 2; i++)
                    {
                        if (mouseArray[i] != 0)
                        {
                            flagOfReallyPosition = true;
                        }
                    }

                    if (flagOfReallyPosition)
                    {
                        int x = 255 * mouseArray[0] + mouseArray[1], y = 255 * mouseArray[2] + mouseArray[3];
                        int width = 255 * mouseArray[4] + mouseArray[5], height = 255 * mouseArray[6] + mouseArray[7];
                        bool flagOfLeftClick = (bool)(mouseArray[8] == 1 ? true : false),
                            flagOfDoubleClick = (bool)(mouseArray[9] == 1 ? true : false),
                            flagOfRightClick = (bool)(mouseArray[10] == 1 ? true : false);
                        float fx = (float)(100 * width) / (float)Screen.PrimaryScreen.Bounds.Width;
                        fx = 100 / fx;
                        x = (int)(fx * (float)x);
                        float fy = (float)(100 * height) / (float)Screen.PrimaryScreen.Bounds.Height;
                        fy = 100 / fy;
                        y = (int)(fy * (float)y);
                        SetCursorPos(x, y);

                        if (flagOfLeftClick)
                            mouse.sendMouseLeftclick(new Point { X = x, Y = y });
                        if (flagOfDoubleClick)
                            mouse.sendMouseDoubleClick(new Point { X = x, Y = y });
                        if (flagOfRightClick)
                            mouse.sendMouseRightclick(new Point { X = x, Y = y });
                        flagOfReallyPosition = false;
                    }
                }catch(Exception exp)
                {

                }
            }
        }

    }
}
