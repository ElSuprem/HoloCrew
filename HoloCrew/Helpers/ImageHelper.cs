using CommunityToolkit.Mvvm.Input;
using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace HoloCrew.Helpers
{
    /// <summary>
    /// Funciones de ayuda para manejo de imágenes
    /// </summary>
    public static class ImageHelper
    {
        /// <summary>
        /// Carga una imagen desde una URL local o recurso
        /// </summary>
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
                bitmap.Freeze(); // Para thread-safety
                return bitmap;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Carga una imagen desde bytes
        /// </summary>
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

        /// <summary>
        /// Redimensiona una imagen manteniendo aspecto
        /// </summary>
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

        /// <summary>
        /// Obtiene una imagen placeholder por defecto
        /// </summary>
        public static BitmapImage GetPlaceholderImage()
        {
            // Retornar una imagen por defecto desde recursos
            return LoadImage("pack://application:,,,/Resources/Images/placeholder.png");
        }
    }
}