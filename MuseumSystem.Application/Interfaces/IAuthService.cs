﻿using MuseumSystem.Application.Dtos.AuthDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuseumSystem.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> LoginAsync(AuthRequest request);
    }
}
