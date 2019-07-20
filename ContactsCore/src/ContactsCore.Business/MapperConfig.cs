using AutoMapper;

namespace ContactsCore.Business
{
    public class MapperConfig
    {
        public static IConfigurationProvider Init()
        {
            var mapperConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<Data.Dao.ContactWithDetailsQueryResult, Model.ViewModels.ContactWithDetailsViewModel>()
                    .ForMember(src => src.ContactDetails, opt => opt.MapFrom(o => o.Details))
                    .AfterMap((src, dest) =>
                    {
                        dest.Uid = src.Contact.Uid;
                        dest.FirstName = src.Contact.FirstName;
                        dest.LastName = src.Contact.LastName;
                    });

                config.CreateMap<Data.Dao.Contact, Model.Models.Contact>()
                    .ForSourceMember(src => src.Id, opt => opt.DoNotValidate())
                    .ForSourceMember(src => src.Created, opt => opt.DoNotValidate())
                    .ForSourceMember(src => src.Modified, opt => opt.DoNotValidate())
                    .ForSourceMember(src => src.IsDeleted, opt => opt.DoNotValidate())
                    .ForSourceMember(src => src.Details, opt => opt.DoNotValidate())
                    .ReverseMap();

                config.CreateMap<Data.Dao.ContactDetail, Model.Models.ContactDetail>()
                    .ForSourceMember(src => src.Id, opt => opt.DoNotValidate())
                    .ForSourceMember(src => src.Created, opt => opt.DoNotValidate())
                    .ForSourceMember(src => src.Modified, opt => opt.DoNotValidate())
                    .ForSourceMember(src => src.IsDeleted, opt => opt.DoNotValidate())
                    .ForSourceMember(src => src.Contact, opt => opt.DoNotValidate())
                    .ForSourceMember(src => src.ContactId, opt => opt.DoNotValidate())
                    .ReverseMap();                
            });
            return mapperConfig;
        }
    }
}
