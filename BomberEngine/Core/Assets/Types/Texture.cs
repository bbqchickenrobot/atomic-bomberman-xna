﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace BomberEngine.Core.Assets.Types
{
    public class Texture : Asset
    {
        private Texture2D texture;

        public Texture(Texture2D texture)
        {
            this.texture = texture;
        }

        protected override void OnDispose()
        {   
            texture.Dispose();
            texture = null;
        }

        public Texture2D GetTexture()
        {
            return texture;
        }
    }
}
