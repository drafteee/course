
namespace CourseProjectClient
{
    public class Constants
    {
        public const byte BEGIN_RECIEVE_ARRAY = 5;
        public const byte SIZE_HEIGHT_FRAME = 200;
        public const byte SIZE_BLACKWHITE_IMAGE = 128;
        public const byte COUNT_COLORS_IMAGE = 3;
        public const byte COUNT_BITS_FOR_HASH = 64;
        public const byte COUNT_HASH = 255;
        public const byte MID_BLACKWHITE = 200;
        public const byte SYSTEM_NUMERATION_2 = 2;
        public const byte SYSTEM_NUMERATION_16 = 16;
        public const byte BEGIN_ARRAY_SEND = 5;
        public const byte ELAPSED_TIME = 60;
        public const byte HEIGHT_SEND_IMAGE = 200;
        public const ushort UDP_LENGTH = 65471;
        public const ushort PORT = 9090;
        public const byte MAX_BYTE = 255;
        public enum STATUS_SEND { START_SEND, STOP_SEND };
    }
}
