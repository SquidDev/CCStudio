using System.Collections.Generic;

namespace CCStudio.Core.Display
{
    public class PixelGrid
    {
        public List<Pixel> Pixels = new List<Pixel>();
        public int Width;
        public int Height;

        public PixelGrid(int Width, int Height){
            this.Width = Width;
            this.Height = Height;

            Generate();
        }

        protected void Generate()
        {
            Pixels.Clear();
            for (int Y = 0; Y < Height; Y++)
            {   
                for (int X = 0; X < Width; X++)
                {
                    Pixels.Add(new Pixel(X, Y));
                }
            }
        }

        #region Clear
        public void Clear(byte Background = 15)
        {
            foreach (Pixel P in Pixels)
            {
                P.Character = ' ';
                P.Background = Background;
            }
        }
        #endregion
        public void Scroll(int YAmmount, byte Background = 15)
        {
            if (YAmmount >= Height)
            {
                foreach (Pixel P in Pixels)
                {
                    P.Character = ' ';
                    P.Background = Background;
                }
                return;
            }


            if (YAmmount < 0)
            {
                //TODO: Other way scrolling
                return;
            }

            int Copy = (Height - YAmmount) * Width;
            int CopyFrom = YAmmount * Width;
            for (int Cell = 0; Cell < Copy; Cell++)
            {
                Pixel ThisPixel = Pixels[Cell];
                Pixel ScrollPixel = Pixels[Cell + CopyFrom];

                ThisPixel.Character = ScrollPixel.Character;
                ThisPixel.Foreground = ScrollPixel.Foreground;
                ThisPixel.Background = ScrollPixel.Background;
            }

            int End = Height * Width;

            for (int Cell = Copy; Cell < End; Cell++)
            {
                Pixel P = Pixels[Cell];
                P.Character = ' ';
                P.Background = Background;
            }
        }
        public Pixel GetPixel(int X, int Y)
        {
            return Pixels[Y * Width + X];
        }
    }
}
