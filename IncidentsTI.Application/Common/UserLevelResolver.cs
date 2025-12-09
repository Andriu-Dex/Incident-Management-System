namespace IncidentsTI.Application.Common;

/// <summary>
/// Resuelve el nivel de escalamiento según el rol del usuario.
/// Mapeo: Pasante = Nivel 1, Técnico = Nivel 2, Admin = Nivel 3
/// </summary>
public static class UserLevelResolver
{
    /// <summary>
    /// Obtiene el nivel de escalamiento según los roles del usuario.
    /// El nivel más alto tiene prioridad.
    /// </summary>
    /// <param name="roles">Lista de roles del usuario</param>
    /// <returns>Nivel de escalamiento (1-3), o 0 si no es personal de soporte</returns>
    public static int GetUserLevel(IEnumerable<string> roles)
    {
        // Admin tiene el nivel más alto (puede manejar cualquier incidente)
        if (roles.Contains("Administrator"))
            return 3;
        
        // Técnico tiene nivel 2
        if (roles.Contains("Technician"))
            return 2;
        
        // Pasante tiene nivel 1
        if (roles.Contains("Pasante"))
            return 1;
        
        // Usuarios sin rol de soporte no tienen nivel (no pueden reclamar)
        return 0;
    }

    /// <summary>
    /// Verifica si el usuario puede ver/reclamar incidentes del nivel especificado.
    /// </summary>
    /// <param name="userLevel">Nivel del usuario (1-3)</param>
    /// <param name="incidentLevel">Nivel actual del incidente (null = nuevo, sin nivel)</param>
    /// <returns>True si el usuario puede acceder al incidente</returns>
    public static bool CanAccessLevel(int userLevel, int? incidentLevel)
    {
        // El usuario debe ser personal de soporte
        if (userLevel == 0)
            return false;
        
        // Si el incidente no tiene nivel asignado (nuevo), todos pueden verlo
        if (!incidentLevel.HasValue)
            return true;
        
        // El usuario puede acceder si su nivel es >= al del incidente
        return userLevel >= incidentLevel.Value;
    }

    /// <summary>
    /// Obtiene el nivel de escalamiento para un rol específico.
    /// </summary>
    /// <param name="role">Nombre del rol</param>
    /// <returns>Nivel de escalamiento</returns>
    public static int GetLevelForRole(string role) => role switch
    {
        "Administrator" => 3,
        "Technician" => 2,
        "Pasante" => 1,
        _ => 0
    };

    /// <summary>
    /// Obtiene el nombre del nivel de soporte.
    /// </summary>
    /// <param name="level">Nivel (1-3)</param>
    /// <returns>Nombre descriptivo del nivel</returns>
    public static string GetLevelName(int level) => level switch
    {
        1 => "Nivel 1 - Soporte Inicial",
        2 => "Nivel 2 - Soporte Técnico",
        3 => "Nivel 3 - Soporte Avanzado",
        _ => "Sin nivel"
    };

    /// <summary>
    /// Verifica si un usuario es personal de soporte (puede reclamar incidentes).
    /// </summary>
    /// <param name="roles">Lista de roles del usuario</param>
    /// <returns>True si es Pasante, Técnico o Admin</returns>
    public static bool IsSupportStaff(IEnumerable<string> roles)
    {
        return GetUserLevel(roles) > 0;
    }
}
