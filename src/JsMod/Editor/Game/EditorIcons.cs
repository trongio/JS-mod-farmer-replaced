using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace JsMod.Editor.Game
{
    /// <summary>Sprites for the mod's buttons, loaded once from the PNGs embedded in the plugin. They outlive scene changes.</summary>
    internal static class EditorIcons
    {
        private static Dictionary<string, Sprite> sprites;

        /// <summary>False when any icon failed to load, after one warning. Buttons then use text labels.</summary>
        internal static bool Available
        {
            get
            {
                Load();
                return sprites.Count == IconNames.All.Length;
            }
        }

        internal static Sprite Get(string name)
        {
            Load();
            return name != null && sprites.TryGetValue(name, out Sprite sprite) ? sprite : null;
        }

        private static void Load()
        {
            if (sprites != null)
            {
                return;
            }
            sprites = new Dictionary<string, Sprite>();
            try
            {
                foreach (string name in IconNames.All)
                {
                    sprites[name] = LoadSprite(name);
                }
            }
            catch (Exception ex)
            {
                sprites.Clear();
                Plugin.Log.LogWarning("Could not load the button icons, so the buttons use text labels: " + ex.Message);
            }
        }

        private static Sprite LoadSprite(string name)
        {
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("icons/" + name + ".png"))
            {
                if (stream == null)
                {
                    throw new FileNotFoundException("Missing embedded icon " + name);
                }
                var bytes = new byte[stream.Length];
                if (stream.Read(bytes, 0, bytes.Length) != bytes.Length)
                {
                    throw new IOException("Could not read icon " + name);
                }
                var texture = new Texture2D(2, 2, TextureFormat.RGBA32, true)
                {
                    name = "JsMod " + name,
                    filterMode = FilterMode.Bilinear,
                    wrapMode = TextureWrapMode.Clamp,
                    hideFlags = HideFlags.HideAndDontSave
                };
                // By reflection, because compiling against this module drags in a newer netstandard than net472's.
                MethodInfo decode = Type.GetType("UnityEngine.ImageConversion, UnityEngine.ImageConversionModule")?.GetMethod("LoadImage", new[] { typeof(Texture2D), typeof(byte[]) });
                if (decode == null)
                {
                    throw new MissingMethodException("ImageConversion.LoadImage");
                }
                if (!(bool)decode.Invoke(null, new object[] { texture, bytes }))
                {
                    throw new InvalidDataException("Could not decode icon " + name);
                }
                Sprite sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect);
                sprite.name = texture.name;
                sprite.hideFlags = HideFlags.HideAndDontSave;
                return sprite;
            }
        }
    }
}
