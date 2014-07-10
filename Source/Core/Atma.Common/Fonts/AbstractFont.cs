//using Atma.Assets;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace Atma.Common.Font
//{
//    public abstract class AbstractFont : AbstractAsset<FontData>, IFont
//    {

//        protected AbstractFont(AssetUri uri)
//            : base(uri)
//        {
            
//        }

//        public float getWidth(string text)
//        {
//            if (string.IsNullOrEmpty(text))
//                return 0f;

//            var w = 0f;
//            for (var i = 0; i < text.Length; i++)
//            {
//                var fc = get(text[i]);
//                if (fc != null)
//                    w += fc.Width;
//            }

//            return w;
//        }

//        public abstract  float getLineHeight();

//        public bool has(char ch)
//        {
//            return _characterMap.ContainsKey(ch);
//        }

//        public FontCharacter get(char ch)
//        {
//            FontCharacter fc;
//            if (_characterMap.TryGetValue(ch, out fc))
//                return fc;

//            return null;
//        }

//        protected override void ondispose()
//        {
//            _characterMap.Clear();
//        }
//    }
//}
