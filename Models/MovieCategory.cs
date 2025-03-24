using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Movie.Models;

[PrimaryKey("MovieId", "CategoriesId")]
public partial class MovieCategory
{
    [Key]
    [Column("MovieID")]
    public int MovieId { get; set; }

    [Key]
    [Column("CategoriesID")]
    public int CategoriesId { get; set; }

    [Column("MovieCategoryID")]
    public int MovieCategoryId { get; set; }

    [ForeignKey("CategoriesId")]
    [InverseProperty("MovieCategories")]
    public virtual Category Categories { get; set; } = null!;

    [ForeignKey("MovieId")]
    [InverseProperty("MovieCategories")]
    public virtual Movies Movie { get; set; } = null!;
}
