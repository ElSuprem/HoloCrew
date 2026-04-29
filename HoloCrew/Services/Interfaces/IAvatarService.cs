using System.Threading.Tasks;

// Servicio para gestionar el avatar del usuario.
// Implementación actual: copia local en %AppData%/HoloCrew/avatars/
// Cuando se quiera pasar a Supabase Storage, basta con cambiar la implementación
// (la interfaz no cambia, los ViewModels no se enteran).

namespace HoloCrew.Services.Interfaces
{
    public interface IAvatarService
    {
        // Sube/copia un archivo de imagen y devuelve la URL/ruta donde quedó guardado.
        // Devuelve null si falla.
        Task<string?> UploadAvatarAsync(string sourceFilePath);

        // Borra el avatar local del usuario actual.
        Task<bool> DeleteAvatarAsync(string avatarPath);
    }
}