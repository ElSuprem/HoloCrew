using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace HoloCrew.Controls
{
    /// <summary>
    /// Control de imagen con carga asíncrona y caché de tres niveles.
    ///
    /// Estrategia de carga (mismo patrón que SDWebImage / Glide / Coil):
    ///   1. Caché en memoria (BitmapImage congelados, compartidos entre vistas).
    ///   2. Caché en disco (%AppData%/HoloCrew/image-cache/).
    ///   3. Red (descarga del CDN; la respuesta se guarda en disco y memoria).
    ///
    /// Si la imagen no se puede cargar (404, sin conexión, URL inválida, etc.),
    /// se muestra un placeholder visual "No image available" en su lugar.
    ///
    /// Toda la I/O usa async/await real, sin bloquear ningún hilo del ThreadPool.
    /// La decodificación a BitmapImage se hace en hilo de fondo con Task.Run
    /// y el bitmap se congela (Freeze) para poder mostrarlo desde el hilo de UI.
    /// </summary>
    public partial class AsyncImage : UserControl
    {
        // Caché en memoria compartida entre todas las instancias del control.
        // ConcurrentDictionary es thread-safe; los BitmapImage van congelados.
        private static readonly ConcurrentDictionary<string, BitmapImage> _memoryCache = new();

        // Set de URLs que ya sabemos que fallan (para no reintentarlas en cada binding).
        // También thread-safe.
        private static readonly ConcurrentDictionary<string, byte> _failedUrls = new();

        // HttpClient único compartido (mucho más eficiente que crear uno por imagen).
        private static readonly HttpClient _httpClient = new()
        {
            Timeout = TimeSpan.FromSeconds(15)
        };

        // Directorio de caché en disco. Se crea bajo demanda en la primera escritura.
        private static readonly string _diskCacheDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "HoloCrew",
            "image-cache");

        // URL que esta instancia está cargando ahora mismo. Sirve para descartar
        // resultados obsoletos cuando el binding cambia mientras todavía estamos
        // descargando la imagen anterior (típico al hacer scroll rápido o navegar).
        private string? _currentLoadingUrl;

        public AsyncImage()
        {
            InitializeComponent();
        }

        #region Source (DependencyProperty)

        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(
            nameof(Source),
            typeof(string),
            typeof(AsyncImage),
            new PropertyMetadata(null, OnSourceChanged));

        public string? Source
        {
            get => (string?)GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        private static async void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is AsyncImage control)
            {
                await control.LoadAsync(e.NewValue as string);
            }
        }

        #endregion

        #region Stretch (DependencyProperty)

        public static readonly DependencyProperty StretchProperty = DependencyProperty.Register(
            nameof(Stretch),
            typeof(Stretch),
            typeof(AsyncImage),
            new PropertyMetadata(Stretch.UniformToFill, OnStretchChanged));

        public Stretch Stretch
        {
            get => (Stretch)GetValue(StretchProperty);
            set => SetValue(StretchProperty, value);
        }

        private static void OnStretchChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is AsyncImage control)
            {
                control.ImageElement.Stretch = (Stretch)e.NewValue;
            }
        }

        #endregion

        private async Task LoadAsync(string? url)
        {
            // Reset visual: ocultar la imagen anterior y el placeholder.
            ImageElement.Source = null;
            FallbackPanel.Visibility = Visibility.Collapsed;

            if (string.IsNullOrWhiteSpace(url))
            {
                ShowFallback();
                return;
            }

            _currentLoadingUrl = url;

            // Si esta URL ya falló antes en esta sesión, mostrar el placeholder directamente.
            if (_failedUrls.ContainsKey(url))
            {
                ShowFallback();
                return;
            }

            // Nivel 1: caché en memoria (instantáneo).
            if (_memoryCache.TryGetValue(url, out var cached))
            {
                ImageElement.Source = cached;
                return;
            }

            try
            {
                byte[] bytes;
                var diskPath = GetDiskPath(url);

                // Nivel 2: caché en disco (lectura asíncrona).
                if (File.Exists(diskPath))
                {
                    bytes = await File.ReadAllBytesAsync(diskPath);
                }
                else
                {
                    // Nivel 3: descarga desde el CDN (Supabase Storage).
                    bytes = await _httpClient.GetByteArrayAsync(url);

                    // Guardar en disco para futuras sesiones (sin bloquear).
                    try
                    {
                        Directory.CreateDirectory(_diskCacheDir);
                        await File.WriteAllBytesAsync(diskPath, bytes);
                    }
                    catch (Exception ex)
                    {
                        // Que no se pueda escribir al disco no es crítico:
                        // la imagen se mostrará igual y se cacheará en memoria.
                        System.Diagnostics.Debug.WriteLine(
                            $"[AsyncImage] Disk cache write failed: {ex.Message}");
                    }
                }

                // Decodificar el bitmap en hilo de fondo (operación CPU intensiva).
                var bitmap = await Task.Run(() => DecodeBitmap(bytes));

                // Si mientras descargábamos el binding cambió a otra URL,
                // descartar este resultado para no pisar la imagen correcta.
                if (_currentLoadingUrl != url)
                    return;

                _memoryCache[url] = bitmap;
                ImageElement.Source = bitmap;
            }
            catch (Exception ex)
            {
                // Algo falló (404, sin conexión, URL inválida, formato no soportado, etc.).
                // Marcamos la URL como fallida para no reintentarla, y mostramos el placeholder.
                System.Diagnostics.Debug.WriteLine(
                    $"[AsyncImage] Failed to load {url}: {ex.Message}");

                _failedUrls[url] = 0;

                // Si esta sigue siendo la URL actual, mostrar el placeholder.
                if (_currentLoadingUrl == url)
                {
                    ShowFallback();
                }
            }
        }

        // Muestra el placeholder de "imagen no disponible" en lugar de la imagen real.
        private void ShowFallback()
        {
            ImageElement.Source = null;
            FallbackPanel.Visibility = Visibility.Visible;
        }

        // Decodifica los bytes a BitmapImage usando DecodePixelWidth para no gastar
        // memoria de más cuando la UI sólo necesita un tamaño concreto.
        // Freeze() permite usar el bitmap desde el hilo de UI con seguridad.
        private static BitmapImage DecodeBitmap(byte[] bytes)
        {
            using var stream = new MemoryStream(bytes);
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.DecodePixelWidth = 800; // suficiente para cards (~300 px) y detalle (~700 px)
            bitmap.StreamSource = stream;
            bitmap.EndInit();
            if (bitmap.CanFreeze)
                bitmap.Freeze();
            return bitmap;
        }

        // Convierte la URL en un nombre de archivo seguro mediante hash SHA-256.
        // Evita caracteres prohibidos y colisiones entre URLs distintas.
        private static string GetDiskPath(string url)
        {
            var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(url));
            var name = Convert.ToHexString(hashBytes).ToLowerInvariant();
            return Path.Combine(_diskCacheDir, name + ".cache");
        }
    }
}