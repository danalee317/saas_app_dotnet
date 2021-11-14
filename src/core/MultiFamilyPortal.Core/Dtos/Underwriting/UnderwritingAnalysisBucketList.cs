using System.Text.Json.Serialization;
using MultiFamilyPortal.Converters;
using ReactiveUI;

namespace MultiFamilyPortal.Dtos.Underwrting
{
    [JsonConverter(typeof(ReactiveObjectConverter<UnderwritingAnalysisBucketList>))]
    public class UnderwritingAnalysisBucketList : ReactiveObject
    {
        public string Summary { get; set; }

        public string ValuePlays { get; set; }

        public string ConstructionType { get; set; }

        public string UtilityNotes { get; set; }

        public string CompetitionNotes { get; set; }

        public string HowUnderwritingWasDetermined { get; set; }
    }
}
