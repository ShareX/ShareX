namespace ShareX.HelpersLib
{
    public class CombineLeftMostPoint
    {
        public int XPosition { get; set; }
        public int YPosition { get; set; }
        public int Layer { get; set; }

        public CombineLeftMostPoint(int x, int y, int layer)
        {
            XPosition = x;
            YPosition = y;
            Layer = layer;
        }
    }
}
