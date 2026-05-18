using HoloCrew.Services.Interfaces;
using Supabase.Storage;
using System;
using System.IO;
using System.Threading.Tasks;

// Implementación REAL del servicio de avatar conectada a Supabase Storage.
// 
// Sustituye al mockup local que copiaba el archivo a %AppData%/HoloCrew/avatars/.
// Ahora cada usuario tiene su avatar persistido en el bucket 'avatars' de Supabase,
// con la estructura: avatars/{user_id}/avatar.{ext}
//
// La interfaz IAvatarService NO ha cambiado: el ViewModel sigue llamando a
// UploadAvatarAsync con la ruta del archivo local, y este servicio se encarga
// de subirlo al bucket y devolver la URL pública.
// Este es el caso de uso clásico de inversión de dependencias documentado en P2.

namespace HoloCrew.Services
{
    public class AvatarService : IAvatarService
    {
        private readonly Supabase.Client _supabase;
        private const string BUCKET_NAME = "avatars";

        public AvatarService(Supabase.Client supabase)
        {
            _supabase = supabase;
        }


        // Sube el archivo seleccionado por el usuario al bucket de Supabase Storage.
        // Devuelve la URL pública del archivo subido, o null si falla.
        public async Task<string?> UploadAvatarAsync(string sourceFilePath)
        {
            try
            {
                // Validaciones básicas
                if (string.IsNullOrEmpty(sourceFilePath) || !File.Exists(sourceFilePath))
                    return null;

                // Necesitamos saber el ID del usuario logueado, porque las políticas
                // RLS exigen que cada usuario solo escriba en su propia carpeta.
                var currentUser = _supabase.Auth.CurrentUser;
                if (currentUser == null || string.IsNullOrEmpty(currentUser.Id))
                {
                    System.Diagnostics.Debug.WriteLine("[Avatar] No hay usuario logueado");
                    return null;
                }

                var userId = currentUser.Id;
                var extension = Path.GetExtension(sourceFilePath).ToLowerInvariant();
                if (string.IsNullOrEmpty(extension))
                    extension = ".jpg";

                // Estructura: avatars/{user_id}/avatar.{ext}
                // Las políticas RLS verifican que el primer "folder" sea el user_id.
                var remotePath = $"{userId}/avatar{extension}";

                // Leer el archivo en bytes (async, sin bloquear el hilo de UI).
                var bytes = await File.ReadAllBytesAsync(sourceFilePath);

                // Subir al bucket. Usamos upsert=true para que si ya había un avatar
                // anterior, simplemente lo sobrescriba (un usuario = un avatar).
                var bucket = _supabase.Storage.From(BUCKET_NAME);
                var fileOptions = new Supabase.Storage.FileOptions
                {
                    CacheControl = "3600",
                    Upsert = true,
                    ContentType = GetMimeType(extension)
                };

                await bucket.Upload(bytes, remotePath, fileOptions);

                // Construir la URL pública del archivo subido.
                var publicUrl = bucket.GetPublicUrl(remotePath);

                // Añadir un parámetro de cache-busting para que la imagen nueva se
                // muestre inmediatamente en la UI (sin esperar a que expire la caché
                // del AsyncImage o del navegador).
                var bustedUrl = $"{publicUrl}?t={DateTimeOffset.UtcNow.ToUnixTimeSeconds()}";

                System.Diagnostics.Debug.WriteLine($"[Avatar] Uploaded to: {bustedUrl}");

                return bustedUrl;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Avatar] Upload error: {ex.Message}");
                return null;
            }
        }


        // Borra el avatar del usuario en Storage.
        // (El parámetro avatarPath se ignora: usamos el user_id de la sesión actual,
        // que es más seguro que confiar en lo que venga por argumento.)
        public async Task<bool> DeleteAvatarAsync(string avatarPath)
        {
            try
            {
                var currentUser = _supabase.Auth.CurrentUser;
                if (currentUser == null || string.IsNullOrEmpty(currentUser.Id))
                    return false;

                var userId = currentUser.Id;
                var bucket = _supabase.Storage.From(BUCKET_NAME);

                // Como no sabemos la extensión exacta, intentamos borrar las extensiones
                // típicas. Es defensivo: si no existe, la llamada simplemente no hace nada.
                var extensions = new[] { ".jpg", ".jpeg", ".png", ".bmp", ".webp" };
                foreach (var ext in extensions)
                {
                    try
                    {
                        await bucket.Remove(new System.Collections.Generic.List<string> { $"{userId}/avatar{ext}" });
                    }
                    catch
                    {
                        // Si la extensión no existe, Supabase devuelve error.
                        // Lo ignoramos y seguimos con la siguiente.
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Avatar] Delete error: {ex.Message}");
                return false;
            }
        }


        // Mapea una extensión de archivo a su MIME type para que Supabase guarde
        // el archivo con el Content-Type correcto y el navegador lo sirva bien.
        private static string GetMimeType(string extension)
        {
            return extension.ToLowerInvariant() switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".bmp" => "image/bmp",
                ".webp" => "image/webp",
                _ => "application/octet-stream"
            };
        }
    }
}