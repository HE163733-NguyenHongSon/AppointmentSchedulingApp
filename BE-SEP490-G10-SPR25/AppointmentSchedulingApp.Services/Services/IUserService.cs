﻿using AppointmentSchedulingApp.Services.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentSchedulingApp.Services.Services
{
    public interface IUserService
    {
        Task<UserDTO?> LoginUser(SignInDTO userLogin, StringBuilder message);
        Task<UserDTO?> RegisterUser(RegistrationDTO registrationDto, StringBuilder message);
        string GenerateToken(UserDTO user);
    }
}
