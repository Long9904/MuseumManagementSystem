
namespace MuseumSystem.Application.Dtos.AIChatDtos
{
    public class ArtifactListInfo : BaseResponse
    {
    }
    public class ArtifactInfo : BaseResponse
    {
        public string artifactName { get; set; } = string.Empty;

        public string artifactPeriod { get; set; } = string.Empty;

        public bool isOrigin { get; set; }

        public string displayPosition { get; set; } = string.Empty;

        public string area { get; set; } = string.Empty;
    }

    public class MuseumInfo : BaseResponse
    {

    }

    public class ListAreasInfo : BaseResponse
    {
        public string areaName { get; set; } = string.Empty;
    }

    public class ArtifactInfoInArea : BaseResponse
    {
        public string areaName { get; set; } = string.Empty;
    }

    public class ExhibitionDetailsInfo : BaseResponse
    {
        public string exhibitionName { get; set; } = string.Empty;
    }

    public class SuggestedExhibitionsInfo : BaseResponse
    {
    }

    // Suggest by personality
    public class SuggestedTourByMood : BaseResponse
    {
        public string mood { get; set; } = string.Empty;
    }

    public class SuggestedTourByInterest : BaseResponse
    {
        public string interest { get; set; } = string.Empty;
    }

    public class SuggestedTourByTime : BaseResponse
    {
        public string availableTime { get; set; } = string.Empty;
    }

    public class SuggestedTourByAge : BaseResponse
    {
        public string ageGroup { get; set; } = string.Empty;
    }

    public class GetLastArtifactInfo : BaseResponse
    {
    }

    public class GetLastAreaInfo : BaseResponse
    {
    }

    public class GetLastExhibitionInfo : BaseResponse
    {
    }
}
