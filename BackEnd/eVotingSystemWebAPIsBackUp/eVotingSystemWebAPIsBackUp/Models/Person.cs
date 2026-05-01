using System;
using System.Collections.Generic;

namespace eVotingSystemWebAPIsBackUp.Models;

public partial class Person
{
    public int Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string? MiddleName { get; set; }

    public string LastName { get; set; } = null!;

    public string IdNo { get; set; } = null!;

    public string Gender { get; set; } = null!;
}
