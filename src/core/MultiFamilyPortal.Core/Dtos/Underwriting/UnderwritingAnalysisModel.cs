using System.Text.Json.Serialization;
using MultiFamilyPortal.Converters;
using ReactiveUI;

namespace MultiFamilyPortal.Dtos.Underwriting
{
    [JsonConverter(typeof(ReactiveObjectConverter<UnderwritingAnalysisModel>))]
    public class UnderwritingAnalysisModel : ReactiveObject
    {
        public string Name { get; set; }

        public int Beds { get; set; }

        public double Baths { get; set; }

        public double MarketRent { get; set; }

        public List<UnderwritingAnalysisUnit> Units { get; set; }
    }
}
