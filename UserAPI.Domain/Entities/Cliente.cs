namespace UserAPI.Domain.Entities
{
    public class Cliente
    {
        public string? Clave { get; set; }
        public string? Nombre { get; set; }
        public int Edad { get; set; }
        public DateOnly Fecha_Nacimiento { get; set; }
    }
}
