using HoloCrew.Services.Interfaces;
using System;
using System.IO;
using System.Threading.Tasks;

// Implementación MOCKUP del servicio de avatar.
// Copia el archivo seleccionado por el usuario a %AppData%/HoloCrew/avatars/
// y devuelve la ruta absoluta del archivo copiado.
//
// NOTA: cuando se quiera migrar a Supabase Storage, sustituir el método
// UploadAvatarAsync por la subida real al bucket 'avatars' usando _supabase.Storage.

namespace HoloCrew.Services
{
    public class AvatarService : IAvatarService
    {
        private readonly string _avatarsFolder;

        public AvatarService()
        {
            // Carpeta local donde guardamos los avatares de los usuarios
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            _avatarsFolder = Path.Combine(appData, "HoloCrew", "avatars");

            // Asegurar que existe
            Directory.CreateDirectory(_avatarsFolder);
        }


        public async Task<string?> UploadAvatarAsync(string sourceFilePath)
        {
            try
            {
                if (string.IsNullOrEmpty(sourceFilePath) || !File.Exists(sourceFilePath))
                    return null;

                // Generar nombre único basado en timestamp + extensión original
                var extension = Path.GetExtension(sourceFilePath).ToLower();
                var fileName = $"avatar_{DateTime.Now:yyyyMMddHHmmss}{extension}";
                var destPath = Path.Combine(_avatarsFolder, fileName);

                // Copiar el archivo
                await Task.Run(() => File.Copy(sourceFilePath, destPath, overwrite: true));

                System.Diagnostics.Debug.WriteLine($"[Avatar] Saved to: {destPath}");

                // Devolvemos la ruta absoluta. WPF puede mostrar imágenes desde rutas locales
                // directamente con su URL "file:///C:/Users/..."
                return destPath;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Avatar] Upload error: {ex.Message}");
                return null;
            }
        }


        public Task<bool> DeleteAvatarAsync(string avatarPath)
        {
            try
            {
                if (!string.IsNullOrEmpty(avatarPath) && File.Exists(avatarPath))
                {
                    File.Delete(avatarPath);
                    return Task.FromResult(true);
                }
                return Task.FromResult(false);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Avatar] Delete error: {ex.Message}");
                return Task.FromResult(false);
            }
        }
    }
}