using AutoMapper;
using SchoolWebApp.Core.DTOs.School.ToDoList;
using SchoolWebApp.Core.Entities.School;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolWebApp.Core.Profiles.School
{
    public class ToDoListProfile: Profile
    {
        public ToDoListProfile()
        {
            CreateMap<ToDoList, ToDoListDto>();
            CreateMap<ToDoListDto, ToDoList>();
            CreateMap<CreateToDoListDto, ToDoList>();
            CreateMap<CreateToDoListDto, ToDoListDto>();
        }
    }
}
