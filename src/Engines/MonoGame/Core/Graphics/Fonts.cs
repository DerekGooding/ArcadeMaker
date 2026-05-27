using ArcadeMaker.Core.Resources.Serializeables;
using Microsoft.Xna.Framework.Graphics;
using SpriteFontPlus;
using System.Collections.Generic;
using System.IO;

namespace ArcadeMaker.Engines.MonoGame.Core.Graphics
{
    internal static class Fonts
    {
        internal static List<SpriteFont> All { get; } = [];
        internal static SpriteFont? Current { get; set; }

        internal static SpriteFont FromTTF(string ttfPath, float size, GraphicsDevice graphicsDevice)
        {
            byte[] fdata = File.ReadAllBytes(ttfPath);

            // bake the font into a SpriteFont
            var fontBakeResult = TtfFontBaker.Bake(fdata, size, 1024, 1024, [CharacterRange.BasicLatin]);
            return fontBakeResult.CreateSpriteFont(graphicsDevice);
        }

        internal static SpriteFont FromGameFont(GameFont gameFont, GraphicsDevice graphicsDevice)
        {
            return FromTTF(gameFont.ttf, gameFont.size, graphicsDevice);
        }
    }
}