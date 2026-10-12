using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WasteManagementSystemMVC.Models
{
    public class WasteComplaint
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ComplaintID { get; set; }

        public int CitizenID { get; set; }

        public int? WorkerID { get; set; }

        public string Name { get; set; }

        public string Location { get; set; }

        public string WasteType { get; set; }

        public string Description { get; set; }

        public string Status { get; set; }

        public DateTime ReportDate { get; set; }

        public string ComplaintPhoto { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        // Worker GPS location
        public double? WorkerLatitude { get; set; }

        public double? WorkerLongitude { get; set; }

        // GPS verification result
        public bool LocationVerified { get; set; }
    }

}