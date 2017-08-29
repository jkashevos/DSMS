using AutoMapper;

using DSMS.Data.Models;
using DSMS.Web.Models;

namespace DSMS.Web.Models
{
    public class VisitorMappingProfile : Profile
    {
        public VisitorMappingProfile()
        {
            CreateMap<Visitor, VisitorViewModel>()
                   .ReverseMap();
        }

    }
}