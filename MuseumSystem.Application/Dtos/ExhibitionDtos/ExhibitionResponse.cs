using MuseumSystem.Domain.Entities;
using MuseumSystem.Domain.Enums;
using System;

namespace MuseumSystem.Application.Dtos.ExhibitionDtos
{
    public class ExhibitionResponse
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public int Priority { get; set; }
        public ExhibitionStatus Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string MuseumId { get; set; }

        public ExhibitionResponse(Exhibition ex)
        {
            Id = ex.Id;
            Name = ex.Name;
            Description = ex.Description;
            Priority = ex.Priority;
            Status = ex.Status;
            StartDate = ex.StartDate;
            EndDate = ex.EndDate;
            MuseumId = ex.MuseumId;
        }
    }
}
