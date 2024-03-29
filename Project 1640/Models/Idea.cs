﻿using System.ComponentModel.DataAnnotations;

namespace Project_1640.Models
{
    public class Idea
    {
        [Key]
        public int IdeaId { get; set; }
        public string IdeaName { get; set; }
        public string IdeaDescription { get; set; }
        public string CategoryId { get; set; }
        public string UserId { get; set; }
        public string TopicId { get; set; }
        public string? FilePath { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? TotalLike { get; set; } = 0;
        public int? TotalDislike { get; set; } = 0;
        public int? TotalView { get; set; } = 0;
    }
}
