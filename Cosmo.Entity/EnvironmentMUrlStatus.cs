namespace Cosmo.Entity
{
    public class EnvironmentMUrlStatus
    {
        public int Id { get; set; }
        public string ImagePath { get; set; }
        public string Description { get; set; }

        public enum StatusCode
        {
            Running=1,
            Stopped,
            OneOrMoreStopped,
            NotAvailable,
            PartiallyAvailable,
        }
    }
}