namespace UserAPI.Models
{
    /// <summary>
    /// Cuerpo de todas las respuestas de error. "message" se mantiene igual que antes para no romper a los clientes.
    /// </summary>
    public sealed record ErrorResponse(string Message, string? Code = null);
}
