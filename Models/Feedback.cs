using System;
using System.ComponentModel.DataAnnotations;

namespace WasteManagementSystemMVC.Models
{
    public class Feedback
    {
        [Key]
        public int FeedbackID { get; set; }

        public int ComplaintID { get; set; }

        public int CitizenID { get; set; }

        public int Rating { get; set; }

        public string Comments { get; set; }

        public DateTime FeedbackDate { get; set; }
    }

}