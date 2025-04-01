using System;
using System.Collections.Generic;

namespace Mangrove.Data;

public partial class TblAdmin
{
    public string Id { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string CodeSendEmail { get; set; } = null!;
}
