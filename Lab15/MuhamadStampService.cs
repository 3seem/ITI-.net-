

namespace Lab15_StudentPortalWeb.Services
{
    public class MuhamadStampService : IMuhamadStampService
    {
        public string Owner => "Muhamad Assem Ahmed";

        public string Stamp { get; }

        public MuhamadStampService()
        {
            Stamp = Guid.NewGuid().ToString().Substring(0, 8);
        }
    }
}
