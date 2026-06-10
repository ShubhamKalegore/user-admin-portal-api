using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchDemo.Application.DTOs;

public class RefreshTokenRequestDto
{
    public Guid UserId { get; set; }
    public string RefreshToken { get; set; } = string.Empty;
}