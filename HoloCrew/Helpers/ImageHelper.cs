using System;
using System.IO;
using System.Windows.Media.Imaging;

// Helper para cargar imágenes desde ruta o desde bytes, redimensionarlas,
// y obtener una imagen placeholder por defecto. Las imágenes se congelan
// (Freeze) para poder usarlas desde varios hilos.

namespace HoloCrew.Helpers
{
    public static class ImageHelper
    {
        // carga una imagen desde una ruta (puede ser relativa o absoluta)
        // si falla, devuelve null
        public static BitmapImage LoadImage(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return null;

            try
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(path, UriKind.RelativeOrAbsolute);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                bitmap.Freeze(); // se congela para poder usarla desde varios hilos sin problemas
                return bitmap;
            }
            catch
            {
                return null;
            }
        }

        // carga una imagen desde un array de bytes (ejemplo: imagen descargada de internet o sacada de base de datos)
        public static BitmapImage LoadImageFromBytes(byte[] imageBytes)
        {
            if (imageBytes == null || imageBytes.Length == 0)
                return null;

            try
            {
                var bitmap = new BitmapImage();
                using (var stream = new MemoryStream(imageBytes))
                {
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.StreamSource = stream;
                    bitmap.EndInit();
                    bitmap.Freeze();
                }
                return bitmap;
            }
            catch
            {
                return null;
            }
        }

        // carga una imagen pero la redimensiona al ancho y alto máximo indicados
        // mantiene el aspecto original, no la estira
        public static BitmapImage ResizeImage(string path, int maxWidth, int maxHeight)
        {
            if (string.IsNullOrWhiteSpace(path))
                return null;

            try
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(path, UriKind.RelativeOrAbsolute);
                bitmap.DecodePixelWidth = maxWidth;
                bitmap.DecodePixelHeight = maxHeight;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                bitmap.Freeze();
                return bitmap;
            }
            catch
            {
                return null;
            }
        }

        // devuelve una imagen por defecto para cuando no hay foto disponible
        // la imagen está en Resources/Images/placeholder.png
        public static BitmapImage GetPlaceholderImage()
        {
            return LoadImage("pack://application:,,,/Resources/Images/placeholder.png");
        }
    }
}