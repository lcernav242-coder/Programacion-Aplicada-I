namespace MantenimientoLibros.Models
{
    public class Autor
    {
        public string AuId { get; set; } = string.Empty;
        public string AuLname { get; set; } = string.Empty;
        public string AuFname { get; set; } = string.Empty;
        public string NombreCompleto => $"{AuFname} {AuLname}";
        public bool IsSelected { get; set; }
    }
}