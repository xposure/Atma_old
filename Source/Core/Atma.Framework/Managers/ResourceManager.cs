using Atma.Assets;
using Atma.Engine;
using Atma.Graphics;
using Atma.Json;
using Atma.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;
using System.IO;

namespace Atma.Managers
{
    public sealed class ResourceManager
    {
        public static readonly GameUri Uri = "engine:resources";

        public Material defaultMaterial { get; private set; }
        public Texture2D basewhite { get; private set; }

        private ushort _textureid = 1;
        private int _nameIndex = 0;
        private Dictionary<string, Song> _music = new Dictionary<string, Song>();
        private Dictionary<string, SoundEffect> _sounds = new Dictionary<string, SoundEffect>();
        private Dictionary<string, Material> _materials = new Dictionary<string, Material>();
        private Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();
        private Dictionary<string, BmFont> fonts = new Dictionary<string, BmFont>();
        private Dictionary<string, JsonObject> jsonobjects = new Dictionary<string, JsonObject>();

        private List<string> searchPathes = new List<string>();

        internal ResourceManager()
        {

        }

        internal void init()
        {
            //var m = Create("basewhite");
            //m.textureName = "basewhite";
            //releaseTextures();
            //reload()
        }

        public SoundEffect createSoundFromFile(string file)
        {
            file = file.ToLower();

            SoundEffect soundFile;
            if (!_sounds.TryGetValue(file, out soundFile))
            {
                soundFile = Root.instance.content.Load<SoundEffect>(findFile(file));
                _sounds.Add(file, soundFile);
            }

            return soundFile;
        }

        public Song createMusicFromFile(string file)
        {
            file = file.ToLower();

            Song songFile;
            if (!_music.TryGetValue(file, out songFile))
            {
                songFile = Root.instance.content.Load<Song>(findFile(file));
                _music.Add(file, songFile);
            }

            return songFile;
        }

        public Material createEmptyMaterial()
        {
            return createMaterial(string.Format("_material_{0}_", _nameIndex++));
        }

        public Material createMaterial(string name, MaterialData data)
        {
            var m = new Material("asset:material:" + name, data);
            _materials.Add(name, m);
            return m;
        }

        public Material createMaterial(string name)
        {
            return createMaterial(name, new MaterialData());
        }

        public Material createMaterialFromTexture(string textureName, MaterialData data)
        {
            Material material;
            if (!_materials.TryGetValue(textureName, out material))
            {
                material = createMaterial(textureName, data);
                material.texture = loadTexture(textureName);
            }
            return material;
        }

        public Material createMaterialFromTexture(string textureName)
        {
            return createMaterialFromTexture(textureName, new MaterialData());
        }

        public Material createMaterialFromTexture(string materialName, string textureName)
        {
            Material material;
            if (!_materials.TryGetValue(materialName, out material))
            {
                material = createMaterial(materialName);
                material.texture = loadTexture(textureName);
            }
            return material;
        }

        //public Material CreateFromTexture(string texture)
        //{
        //    var m = Create();
        //    m.textureName = texture;
        //    return m;
        //}

        public Material findMaterial(string name)
        {
            return _materials[name];
        }

        public Texture2D createTexture(string name, int width, int height)
        {
            var texture = new Texture2D("asset:texture:" + name, width, height);
            textures.Add(name, texture);
            return texture;
        }

        public Texture2D loadTexture(string name, Color transparentKey)
        {
            var file = findFile(name);
            var texture = new Texture2D("asset:texture:" + name, TextureData.fromStream(file));
            var data = new Color[texture.width * texture.height];
            texture.getData(ref data);

            for (int x = 0; x < texture.width; x++)
            {
                for (int y = 0; y < texture.height; y++)
                {
                    if (data[x + y * texture.width] == transparentKey)
                        data[x + y * texture.width] = Color.Transparent;
                }
            }

            texture.setData(data);
            return texture;
        }

        public Texture2D loadTexture(string name)
        {
            var file = findFile(name);
            var texture = new Texture2D("asset:texture:" + name, TextureData.fromStream(file));
            //var texture = new Texture2D(_textureid++, name, Root.instance.content.Load<Microsoft.Xna.Framework.Graphics.Texture2D>(findFile(name)));
            var data = new Color[texture.width * texture.height];
            texture.getData(ref data);

            if (data[0] == Color.Magenta)
            {
                //var data = new Color[texture.Width * texture.Height];
                texture.getData(ref data);

                for (int x = 0; x < texture.width; x++)
                {
                    for (int y = 0; y < texture.height; y++)
                    {
                        if (data[x + y * texture.width] == Color.Magenta)
                            data[x + y * texture.width] = Color.Transparent;
                    }
                }

                texture.setData(data);
            }

            return texture;
        }

        public void touchTexture(string name)
        {
            findTexture(name);
        }

        public Texture2D findTexture(string name)
        {
            Texture2D texture = null;
            if (!textures.TryGetValue(name, out texture))
            {
                texture = new Texture2D("asset:texture:" + name, 1, 1);
                texture.setData(new Color[] { Color.Pink });
                textures.Add(name, texture);
            }
            return texture;
        }

        internal void releaseTextures()
        {
            releaseTextures(true);
        }

        internal void releaseTextures(bool reload)
        {
            foreach (var texture in textures.Values)
                texture.dispose();

            textures.Clear();
            if (reload)
            {
                basewhite = new Texture2D("asset:texture:baseWhite", 1, 1);
                basewhite.setData(new Color[] { Color.White });
                defaultMaterial = new Material("asset:material:" + "default", new MaterialData());
                defaultMaterial.texture = basewhite;

                //basewhite = new TextureRef("basewhite");
                //basewhite.texture = new Texture2D(0, "basewhite", new Microsoft.Xna.Framework.Graphics.Texture2D(Root.instance.graphics.graphicsDevice, 1, 1));
                //basewhite.texture.SetData(new Color[] { Color.White });
                //textures.Add("basewhite", basewhite);
            }
        }

        internal void reload()
        {
            releaseTextures(true);
            clearFonts();
            _materials.Clear();
            defaultMaterial = new Material("asset:material:" + "default", new MaterialData());
        }

        internal void cleanup()
        {
            releaseTextures(false);
            clearFonts();
            _materials.Clear();
            defaultMaterial = new Material("asset:material:" + "default", new MaterialData());
        }

        public BmFont findFont(string name)
        {
            BmFont font;
            if (!fonts.TryGetValue(name, out font))
            {
                font = loadFont(name);
                fonts.Add(name, font);
            }
            return fonts[name];
        }

        private BmFont loadFont(string path)
        {
            var fontFilePath = findFile(path);
            using (var stream = TitleContainer.OpenStream(fontFilePath))
            {
                var fontFile = FontLoader.Load(stream);
                var fontPng = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(path), fontFile.Pages[0].File);

                // textRenderer initialization will go here
                stream.Close();
                return new BmFont(fontFile, fontPng);
            }
        }

        //public Effect loadEffect(string file)
        //{
        //    var effectFilePath = findFile(file);
        //    return Root.instance.content.Load<Effect>(effectFilePath);

        //}

        private void clearFonts()
        {
            fonts.Clear();
        }

        public JsonObject findJson(string path)
        {
            JsonObject jo;
            if (!jsonobjects.TryGetValue(path, out jo))
            {
                var filepath = findFile(path);
                using (var stream = TitleContainer.OpenStream(filepath))
                {
                    var sr = new StreamReader(stream);
                    var text = sr.ReadToEnd();

                    jo = JsonReader.ParseObject(text);
                    jsonobjects.Add(path, jo);
                }
            }

            return jo;
        }

        private void clearJson()
        {
            jsonobjects.Clear();
        }

        public void setSearchPath(string path)
        {
            Root.instance.content.RootDirectory = path;
            //searchPathes.Add(path);
        }

        internal string findFile(string file)
        {
            if (System.IO.File.Exists(file))
                return file;

            var contentFile = System.IO.Path.Combine(Root.instance.content.RootDirectory, file);
            if (System.IO.File.Exists(contentFile))
                return contentFile;

            //foreach (var path in searchPathes)
            //{
            //    var newfile = System.IO.Path.Combine(path, file);
            //    if (System.IO.File.Exists(newfile))
            //        return newfile;
            //}

            throw new FileNotFoundException(file);
        }


    }
}
