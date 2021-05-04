using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace Jwt_Core1.Models.Entities
{
    public partial class TblFileDetail
    {
        public int Id { get; set; }
        [Required]
        public string Filename { get; set; }
        [Required]
        public string Fileurl { get; set; }
    }

    public class ListFile
    {
        public List<TblFileDetail> FileList { get; set; }
        public ListFile()
        {
            FileList = new List<TblFileDetail>();
        }
    }
}
