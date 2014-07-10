﻿using Atma.Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atma.Fonts
{
    public interface IFont : IAsset
    {

        int getWidth(string text);
        int getLineHeight();
        bool has(char ch);
        FontCharacter get(char ch);

        //float getWidth(char ch);
        //float getHeight(string text);


    }
}
