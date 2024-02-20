using AutoMapper;
using SchoolWebApp.Core.DTOs.Settings.Relationship;
using SchoolWebApp.Core.Entities.Settings;

namespace SchoolWebApp.Core.Profiles.Settings
{
    public class RelationshipProfile: Profile
    {
        public RelationshipProfile()
        {
            CreateMap<RelationShip, RelationshipDto>();
            CreateMap<RelationshipDto, RelationShip>();
            CreateMap<CreateRelationshipDto, RelationShip>();
            CreateMap<CreateRelationshipDto, RelationshipDto>();
        }
    }
}
