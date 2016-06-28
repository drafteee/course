using System;
using System.Drawing;


namespace CourseProjectClient
{
    class AdditionalCalculations 
    {
        private bool DoSendImage = false;
        private string[] additionalArrayBitsForHash = new string[256];
        public byte summand;

        public byte GetFactor(int value)
        {
            byte additionalValue = (byte)(value / Constants.MAX_BYTE);
            summand = (byte)(value - (Constants.MAX_BYTE * additionalValue));
            return additionalValue;
        }
        private void CycleForImage(bool choose, ref Bitmap additionalImageBlackWhite, ref int mediumValue, ref string[] arrayBitsForHash)
        {
            int countBits = 0, countLineBits = 0;
            for (int i = 0; i < additionalImageBlackWhite.Height; i++)
            {
                for (int j = 0; j < additionalImageBlackWhite.Width; j++)
                {
                    if (choose)
                    {
                        Color pixelColor = additionalImageBlackWhite.GetPixel(j, i);
                        int mediumValueColor = (pixelColor.R + pixelColor.G + pixelColor.B) / Constants.COUNT_COLORS_IMAGE;
                        mediumValue += mediumValueColor;
                        additionalImageBlackWhite.SetPixel(i, j, (mediumValueColor <= Constants.MID_BLACKWHITE ? Color.Black : Color.White));
                    }
                    else
                    {
                        Color pixelColor = additionalImageBlackWhite.GetPixel(j, i);
                        int mediumValueColor = (pixelColor.R + pixelColor.G + pixelColor.B) / Constants.COUNT_COLORS_IMAGE;

                        if (countBits < Constants.COUNT_BITS_FOR_HASH)
                        {
                            arrayBitsForHash[countLineBits] += (byte)(mediumValueColor < mediumValue ? 0 : 1);
                        }
                        else
                        {
                            countLineBits++;
                            arrayBitsForHash[countLineBits] += (byte)(mediumValueColor < mediumValue ? 0 : 1);
                            countBits = 0;
                        }
                        countBits++;
                    }
                }
            }
        }
        public bool CheckSendImage(Image screenshotClient)
        {
            Bitmap additionalImageBlackWhite = new Bitmap(Constants.SIZE_BLACKWHITE_IMAGE, Constants.SIZE_BLACKWHITE_IMAGE);
            int mediumValue = 0;
            string[] arrayBitsForHash = new string[Constants.COUNT_HASH +1];

            using (var graphisc = Graphics.FromImage(additionalImageBlackWhite))
            {
                graphisc.DrawImage(screenshotClient, 0, 0, Constants.SIZE_BLACKWHITE_IMAGE, Constants.SIZE_BLACKWHITE_IMAGE);
                graphisc.Dispose();
            }

            CycleForImage(true, ref additionalImageBlackWhite, ref mediumValue, ref arrayBitsForHash);

            mediumValue = mediumValue / (Constants.SIZE_BLACKWHITE_IMAGE * Constants.SIZE_BLACKWHITE_IMAGE);

            CycleForImage(false, ref additionalImageBlackWhite, ref mediumValue, ref arrayBitsForHash);

            DoSendImage = false;

            for (int i = 0; i <= Constants.COUNT_HASH; i++)
            {
                arrayBitsForHash[i] = Convert.ToString(Convert.ToInt64(arrayBitsForHash[i], Constants.SYSTEM_NUMERATION_2), Constants.SYSTEM_NUMERATION_16);
                if (additionalArrayBitsForHash[i] != arrayBitsForHash[i])
                    DoSendImage = true;
            }

            if (DoSendImage)
            {
                additionalArrayBitsForHash = arrayBitsForHash;
            }

            screenshotClient.Dispose();
            additionalImageBlackWhite.Dispose();
            return DoSendImage;
        }
    }
}
