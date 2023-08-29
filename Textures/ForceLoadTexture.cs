﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using tsorcRevamp.Items;
using Terraria.Localization;

namespace tsorcRevamp.Textures;

internal class ForceLoadTexture
{
    public string _texturePath { get; private set; }
    public Texture2D texture { get; private set; }

    public ForceLoadTexture(string path)
    {
        _texturePath = path;
    }

    internal void KeepLoaded()
    {
        if (texture == null || texture.IsDisposed)
        {
            texture = ModContent.Request<Texture2D>(_texturePath, ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
        }
    }
}
