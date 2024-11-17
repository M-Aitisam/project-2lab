
namespace project_2lab.Data
{
    public class AnnouncementService
    {
        private List<Announcement> _announcements = new List<Announcement>();

        public AnnouncementService()
        {
            // Simulate some initial data
            _announcements.Add(new Announcement { Message = "Welcome to the Announcement Board!", Timestamp = DateTime.Now, IsRead = false });
        }

        public Task<List<Announcement>> GetAnnouncementsAsync()
        {
            return Task.FromResult(_announcements);
        }

        public Task AddAnnouncementAsync(Announcement announcement)
        {
            _announcements.Insert(0, announcement); // Insert new announcement at the top
            return Task.CompletedTask;
        }
    }
}
