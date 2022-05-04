using UnityEditor;

namespace UnityTools.TextureTools
{
    public class TextureSettingTools
    {
        public static void AssetSetReadWriteEnabled(string path, bool b, bool b1)
        {
            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            importer.isReadable = b;
            importer.SaveAndReimport();
        }

        public static void AssetSetFormat(string path, TextureImporterFormat argb32)
        {
            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            var defaultPlatformTextureSettings = importer.GetDefaultPlatformTextureSettings();
            defaultPlatformTextureSettings.format = argb32;
            importer.SetPlatformTextureSettings(defaultPlatformTextureSettings);
            importer.SaveAndReimport();
        }

        public static void AssetSetCompresss(string path, TextureImporterCompression uncompressed)
        {
            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            importer.textureCompression = uncompressed;
            importer.SaveAndReimport();
        }
    }
}