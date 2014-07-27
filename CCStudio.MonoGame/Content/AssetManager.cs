using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace CCStudio.MonoGame.Contents
{
    public class AssetManager
    {
        public static Texture2D PixelBackground;
        public static SpriteFont CoreFont;

        public static void LoadContent(ContentManager Content, GraphicsDevice Device)
        {
            PixelBackground = new Texture2D(Device, 1, 1, false, SurfaceFormat.Color);
            PixelBackground.SetData<Color>(new Color[] { Color.White });

            CoreFont = Content.Load<SpriteFont>("CCFont");
            CoreFont.DefaultCharacter = '?';
        }
    }
}
