namespace CondominiumAlerts.Features.Features.Condominium.Add
{
    public class AddCondominiumResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
    }
}