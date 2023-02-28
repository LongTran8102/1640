namespace Project_1640.ViewModels
{
    public class IdeaViewModel
    {
        public string IdeaName { get; set; }
        public string IdeaDescription { get; set; }
        public string CategoryId { get; set; }   
        public string UserId { get; set; }
        public IFormFile AttachFile { get; set; }
    }
}
