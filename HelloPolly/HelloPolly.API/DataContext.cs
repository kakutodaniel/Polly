using System;

namespace HelloPolly.API
{
    public class DataContext
    {
        public Guid Id { get; set; }

        public string Description { get; set; }

        public DateTime CreatedDate => DateTime.UtcNow;

    }
}
