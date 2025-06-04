using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO;

public class ReviewDto
{
    [Range(1, 5)]
    public double? Rating { get; set; }

    public string? Comment { get; set; }
}
