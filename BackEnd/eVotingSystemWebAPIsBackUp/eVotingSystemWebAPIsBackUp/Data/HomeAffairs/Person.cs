using System;
using System.Collections.Generic;

namespace eVotingSystemWebAPIsBackUp.Data.HomeAffairs;

public partial class Person
{
    public int Id { get; set; }

    public string Firstname { get; set; } = null!;

    public string? Middlename { get; set; }

    public string Lastname { get; set; } = null!;

    public string IdNo { get; set; } = null!;

    public string Gender { get; set; } = null!;
}
