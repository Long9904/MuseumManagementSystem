using AutoMapper;
using MuseumSystem.Application.Dtos.AccountDtos;
using MuseumSystem.Application.Dtos.AreaDtos;
using MuseumSystem.Application.Dtos.DisplayPositionDtos;
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
            CreateMap<Account, AccountRespone>();
            CreateMap<AreaRequest, Area>();
            CreateMap<Area, AreaResponse>()
                .ForMember(dest => dest.DisplayPositions,
                           opt => opt.MapFrom(src => src.DisplayPositions));

            CreateMap<DisplayPosition, DisplayPositionSummaryDto>();
            CreateMap<DisplayPosition, DisplayPositionDetailDto>();
            CreateMap<DisplayPositionRequest, DisplayPosition>();
        }

        public class BasePaginatedListConverter<TSource, TDestination> : ITypeConverter<BasePaginatedList<TSource>,BasePaginatedList<TDestination>>
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
