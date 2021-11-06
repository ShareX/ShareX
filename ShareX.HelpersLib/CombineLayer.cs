namespace ShareX.HelpersLib
{
    public class CombineLayer
    {
        public int YPosition { get; set; }
        public int Height { get; set; }

        public CombineLayer(int y, int height)
        {
            YPosition = y;
            Height = height;
        }
    }
}
