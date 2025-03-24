using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Movie.Models;

[PrimaryKey("MovieId", "ActorsId")]
[Table("MovieActor")]
public partial class MovieActor
{
    public int MovieActorId { get; set; }

    [Key]
    [Column("MovieID")]
    public int MovieId { get; set; }

    [Key]
    [Column("ActorsID")]
    public int ActorsId { get; set; }

    [ForeignKey("ActorsId")]
    [InverseProperty("MovieActors")]
    public virtual Actor Actors { get; set; } = null!;

    [ForeignKey("MovieId")]
    [InverseProperty("MovieActors")]
    public virtual Movies Movie { get; set; } = null!;
}
