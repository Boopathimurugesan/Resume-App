namespace ResumeDBTracker.Business.ViewModel
{
    public class FileUpload
    {
        public string? FileName { get; set; }
        public bool IsProcessed { get; set; }
        public string Message { get; set; }
		public string resume_file { get; set; }

        public string category_id{ get; set; }
    }
}