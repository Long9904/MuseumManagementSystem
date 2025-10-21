using AutoMapper;
using MuseumSystem.Application.Dtos.AccountDtos;
using MuseumSystem.Application.Dtos.AreaDtos;
using MuseumSystem.Application.Dtos.ArtifactDtos;
using MuseumSystem.Application.Dtos.DisplayPositionDtos;
using MuseumSystem.Application.Dtos.InteractionDtos;
using MuseumSystem.Application.Dtos.VisitorDtos;
using MuseumSystem.Domain.Abstractions;
using MuseumSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuseumSystem.Application.Dtos
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //Config Mapper Paging
            CreateMap(typeof(BasePaginatedList<>), typeof(BasePaginatedList<>))
                .ConvertUsing(typeof(BasePaginatedListConverter<,>));

            // Add other mappings as needed
            CreateMap<Account, AccountRespone>()
                .ForMember(dest => dest.RoleId, opt => opt.MapFrom(src => src.Role != null ? src.Role.Id : null))
                .ForMember(dest => dest.MuseumId, opt => opt.MapFrom(src => src.Museum != null ? src.Museum.Id : null));
            CreateMap<AreaRequest, Area>();
            CreateMap<Area, AreaResponse>()
                .ForMember(dest => dest.DisplayPositions,
                           opt => opt.MapFrom(src => src.DisplayPositions));

            CreateMap<DisplayPosition, DisplayPositionSummaryDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.DisplayPositionName, opt => opt.MapFrom(src => src.DisplayPositionName))
                .ForMember(dest => dest.PositionCode, opt => opt.MapFrom(src => src.PositionCode));
            CreateMap<DisplayPosition, DisplayPositionResponse>()
                .ForMember(dest => dest.AreaName, opt => opt.MapFrom(src => src.Area != null ? src.Area.Name : null))
                .ForMember(dest => dest.ArtifactName, opt => opt.MapFrom(src => src.Artifact != null ? src.Artifact.Name : null));
            CreateMap<DisplayPositionRequest, DisplayPosition>();

            CreateMap<ArtifactRequest, Artifact>();
            CreateMap<Artifact, ArtifactResponse>()
                .ForMember(dest => dest.DisplayPositionName,
                           opt => opt.MapFrom(src => src.DisplayPosition != null ? src.DisplayPosition.DisplayPositionName : null))
                .ForMember(dest => dest.AreaName,
                           opt => opt.MapFrom(src => src.DisplayPosition != null && src.DisplayPosition.Area != null ? src.DisplayPosition.Area.Name : null))
                .ForMember(dest => dest.AreaId,
                            opt => opt.MapFrom(src => src.DisplayPosition != null ? src.DisplayPosition.AreaId : null));
            CreateMap<VisitorRequest, Visitor>();
            CreateMap<VisitorUpdateRequest, Visitor>();
            CreateMap<Visitor, VisitorResponse>();

            // Interaction
            CreateMap<InteractionRequest, Interaction>();
            CreateMap<InteractionUpdateRequest, Interaction>();
            CreateMap<Interaction, InteractionResponse>()
                .ForMember(dest => dest.VisitorPhoneNumber, opt => opt.MapFrom(src => src.Visitor != null ? src.Visitor.PhoneNumber : null))
                .ForMember(dest => dest.ArtifactName, opt => opt.MapFrom(src => src.Artifact != null ? src.Artifact.Name : null))
                .ForMember(dest => dest.ArtifactCode, opt => opt.MapFrom(src => src.Artifact != null ? src.Artifact.ArtifactCode : null));

        }


        public class BasePaginatedListConverter<TSource, TDestination> : ITypeConverter<BasePaginatedList<TSource>, BasePaginatedList<TDestination>>
        {
            public BasePaginatedList<TDestination> Convert(
                BasePaginatedList<TSource> source,
                BasePaginatedList<TDestination> destination,
                ResolutionContext context)
            {
                var mappedItems = context.Mapper.Map<List<TDestination>>(source.Items);

                return new BasePaginatedList<TDestination>(
                    mappedItems,
                    source.TotalItems,
                    source.PageIndex,
                    source.PageSize
                );
            }
        }
    }
}
