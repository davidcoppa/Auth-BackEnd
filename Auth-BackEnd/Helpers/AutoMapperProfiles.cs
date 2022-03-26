using Auth_BackEnd.DTOs;
using Auth_BackEnd.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;

namespace Auth_BackEnd.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<UserInfo, NewUserDTO>().ReverseMap();
            CreateMap<UserInfo, UserDTO>().ReverseMap();

        }
    }
}
