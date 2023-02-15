namespace CoreWebApi.Services
{
    public class SingleVacancyDto
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public int Previews { get; set; } = 0;

        public bool IsActive { get; set; } = true;

        public int OfficeId { get; set; }
    }
}
