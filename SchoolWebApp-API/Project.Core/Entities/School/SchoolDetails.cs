using SchoolWebApp.Core.Entities.Settings;
using SchoolWebApp.Core.Entities.Shared;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.Entities.School
{
    public class SchoolDetails : Base
    {
        [Required(ErrorMessage = "Enter the shool name")]
        [Display(Name = "School name")]
        [StringLength(255)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Enter the shool address")]
        [Display(Name = "School address")]
        [StringLength(255)]
        public string Address { get; set; }

        [Required(ErrorMessage = "Enter the shool telephone")]
        [Display(Name = "School telephone")]
        [StringLength(255)]
        public string Telephone { get; set; }

        [Display(Name = "School email")]
        [StringLength(255)]
        [RegularExpression(@"(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|""(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21\x23-\x5b\x5d-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])*"")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[a-z0-9-]*[a-z0-9]:(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])+)\])",
            ErrorMessage = "The email address is not entered in a correct format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Enter the shool motto")]
        [Display(Name = "School motto")]
        [StringLength(255)]
        public string Motto { get; set; }

        [Required(ErrorMessage = "Enter the shool vision")]
        [Display(Name = "School vision")]
        [StringLength(255)]
        public string Vision { get; set; }

        [Required(ErrorMessage = "Enter the shool mission")]
        [Display(Name = "School mission")]
        [StringLength(255)]
        public string Mission { get; set; }

        [Required(ErrorMessage = "Enter the shool initials")]
        [Display(Name = "School initials")]
        [StringLength(255)]
        public string Initials { get; set; }

        [Required(ErrorMessage = "Enter the shool website")]
        [Display(Name = "School website")]
        [StringLength(255)]
        public string Website { get; set; }

        [Display(Name = "School logo")]
        [StringLength(255)]
        public string LogoUrl { get; set; }


        [Display(Name = "Enter if pre-primary level is available")]
        public bool PrePrimary { get; set; }
        [Display(Name = "Enter if lower primary level is available")]
        public bool LowerPrimary { get; set; }
        [Display(Name = "Enter if upper primary level is available")]
        public bool UpperPrimary { get; set; }
        [Display(Name = "Enter if junior school level is available")]
        public bool JuniorSchool { get; set; }
        [Display(Name = "Enter if senior school level is available")]
        public bool SeniorSchool { get; set; }



        [Display(Name = "Other details")]
        [StringLength(255)]
        public string OtherDetails { get; set; }

        [Display(Name = "Report headers")]
        [StringLength(255)]
        public string ReportHeader { get; set; }

        [Display(Name = "Report title")]
        [StringLength(255)]
        public string ReportTitle { get; set; }

        [Display(Name = "Report sub title")]
        [StringLength(255)]
        public string ReportSubTitle { get; set; }

        [Display(Name = "Report title details")]
        [StringLength(255)]
        public string ReportTitleDetails { get; set; }

        public string LogoAsBase64 { get; set; }
    }
}
