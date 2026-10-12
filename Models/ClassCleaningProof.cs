using System;
using System.ComponentModel.DataAnnotations;

using System;
using System.ComponentModel.DataAnnotations;

namespace WasteManagementSystemMVC.Models
{
    public class CleaningProof
    {
        [Key]
        public int ProofID { get; set; }

        public int ComplaintID { get; set; }

        public int WorkerID { get; set; }

        public string Photo { get; set; }

        public DateTime UploadedDate { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public double? DistanceFromComplaint { get; set; }

        public bool LocationVerified { get; set; }
    }

}