using System.Text.Json;
using System.Text.Json.Serialization;
using Humanizer;
using MultiFamilyPortal.Data.Models;
using MultiFamilyPortal.Dtos.Underwriting;
using Newtonsoft.Json.Linq;
using ReactiveUI;

namespace MultiFamilyPortal.Converters
{
    internal class UnderwritingAnalysisConverter : JsonConverter<UnderwritingAnalysis>
    {
        public override UnderwritingAnalysis Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = new UnderwritingAnalysis();

            var lineItemConverter = options.GetConverter<List<UnderwritingAnalysisLineItem>>();
            while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
            {
                var type = reader.TokenType;
                if(reader.TokenType == JsonTokenType.PropertyName)
                {
                    var name = reader.GetString();
                    reader.Read();
                    if (reader.TokenType == JsonTokenType.Null)
                    {
                        continue;
                    }

                    switch(name.Pascalize())
                    {
                        case nameof(UnderwritingAnalysis.Address):
                            value.Address = reader.GetString();
                            break;
                        case nameof(UnderwritingAnalysis.AquisitionFeePercent):
                            value.AquisitionFeePercent = reader.GetDouble();
                            break;
                        case nameof(UnderwritingAnalysis.AskingPrice):
                            value.AskingPrice = reader.GetDouble();
                            break;
                        case nameof(UnderwritingAnalysis.BucketList):
                            var bucketListConverter = options.GetConverter<UnderwritingAnalysisBucketList>();
                            value.BucketList = bucketListConverter.Read(ref reader, typeToConvert, options);
                            break;
                        case nameof(UnderwritingAnalysis.CapitalImprovements):
                            var capitalImprovementsConverter = options.GetConverter<List<UnderwritingAnalysisCapitalImprovement>>();
                            value.CapitalImprovements = capitalImprovementsConverter.Read(ref reader, typeToConvert, options);
                            break;
                        case nameof(UnderwritingAnalysis.CapX):
                            value.CapX = reader.GetDouble();
                            break;
                        case nameof(UnderwritingAnalysis.CapXType):
                            value.CapXType = reader.GetEnum<CostType>();
                            break;
                        case nameof(UnderwritingAnalysis.City):
                            value.City = reader.GetString();
                            break;
                        case nameof(UnderwritingAnalysis.ClosingCostMiscellaneous):
                            value.ClosingCostMiscellaneous = reader.GetDouble();
                            break;
                        case nameof(UnderwritingAnalysis.ClosingCostPercent):
                            value.ClosingCostPercent = reader.GetDouble();
                            break;
                        case nameof(UnderwritingAnalysis.DeferredMaintenance):
                            value.DeferredMaintenance = reader.GetDouble();
                            break;
                        case nameof(UnderwritingAnalysis.DesiredYield):
                            value.DesiredYield = reader.GetDouble();
                            break;
                        case nameof(UnderwritingAnalysis.Downpayment):
                            value.Downpayment = reader.GetDouble();
                            break;
                        case nameof(UnderwritingAnalysis.GrossPotentialRent):
                            value.GrossPotentialRent = reader.GetDouble();
                            break;
                        case nameof(UnderwritingAnalysis.HoldYears):
                            value.HoldYears = reader.GetInt32();
                            break;
                        case nameof(UnderwritingAnalysis.Id):
                            value.Id = reader.GetGuid();
                            break;
                        case nameof(UnderwritingAnalysis.LoanType):
                            value.LoanType = reader.GetEnum<UnderwritingLoanType>();
                            break;
                        case nameof(UnderwritingAnalysis.LTV):
                            value.LTV = reader.GetDouble();
                            break;
                        case nameof(UnderwritingAnalysis.Management):
                            value.Management = reader.GetDouble();
                            break;
                        case nameof(UnderwritingAnalysis.Market):
                            value.Market = reader.GetString();
                            break;
                        case nameof(UnderwritingAnalysis.MarketVacancy):
                            value.MarketVacancy = reader.GetDouble();
                            break;
                        case nameof(UnderwritingAnalysis.Models):
                            var modelConverter = options.GetConverter<List<UnderwritingAnalysisModel>>();
                            value.Models = modelConverter.Read(ref reader, typeof(List<UnderwritingAnalysisModel>), options);
                            break;
                        case nameof(UnderwritingAnalysis.Mortgages):
                            var mortgageConverter = options.GetConverter<List<UnderwritingAnalysisMortgage>>();
                            var mortgages = mortgageConverter.Read(ref reader, typeof(List<UnderwritingAnalysisMortgage>), options);
                            value.AddMortgages(mortgages);
                            break;
                        case nameof(UnderwritingAnalysis.Name):
                            value.Name = reader.GetString();
                            break;
                        case nameof(UnderwritingAnalysis.NeighborhoodClass):
                            value.NeighborhoodClass = Enum.Parse<PropertyClass>(reader.GetString());
                            break;
                        case nameof(UnderwritingAnalysis.Notes):
                            var notesConverter = options.GetConverter<List<UnderwritingAnalysisNote>>();
                            value.Notes = notesConverter.Read(ref reader, typeof(List<UnderwritingAnalysisNote>), options);
                            break;
                        case nameof(UnderwritingAnalysis.OfferPrice):
                            value.OfferPrice = reader.GetDouble();
                            break;
                        case nameof(UnderwritingAnalysis.OurEquityOfCF):
                            value.OurEquityOfCF = reader.GetDouble();
                            break;
                        case nameof(UnderwritingAnalysis.Ours):
                            var ourItems = lineItemConverter.Read(ref reader, typeof(List<UnderwritingAnalysisLineItem>), options);
                            value.AddOurItems(ourItems);
                            break;
                        case nameof(UnderwritingAnalysis.PhysicalVacancy):
                            value.PhysicalVacancy = reader.GetDouble();
                            break;
                        case nameof(UnderwritingAnalysis.PropertyClass):
                            value.PropertyClass = Enum.Parse<PropertyClass>(reader.GetString());
                            break;
                        case nameof(UnderwritingAnalysis.PurchasePrice):
                            value.PurchasePrice = reader.GetDouble();
                            break;
                        case nameof(UnderwritingAnalysis.RentableSqFt):
                            value.RentableSqFt = reader.GetInt32();
                            break;
                        case nameof(UnderwritingAnalysis.SECAttorney):
                            value.SECAttorney = reader.GetDouble();
                            break;
                        case nameof(UnderwritingAnalysis.Sellers):
                            var sellerItems = lineItemConverter.Read(ref reader, typeof(List<UnderwritingAnalysisLineItem>), options);
                            value.AddSellerItems(sellerItems);
                            break;
                        case nameof(UnderwritingAnalysis.StartDate):
                            value.StartDate = DateTimeOffset.Parse(reader.GetString());
                            break;
                        case nameof(UnderwritingAnalysis.State):
                            value.State = reader.GetString();
                            break;
                        case nameof(UnderwritingAnalysis.Status):
                            value.Status = reader.GetEnum<UnderwritingStatus>();
                            break;
                        case nameof(UnderwritingAnalysis.StrikePrice):
                            value.StrikePrice = reader.GetDouble();
                            break;
                        case nameof(UnderwritingAnalysis.Timestamp):
                            value.Timestamp = DateTimeOffset.Parse(reader.GetString());
                            break;
                        case nameof(UnderwritingAnalysis.Underwriter):
                            value.Underwriter = reader.GetString();
                            break;
                        case nameof(UnderwritingAnalysis.UnderwriterEmail):
                            value.UnderwriterEmail = reader.GetString();
                            break;
                        case nameof(UnderwritingAnalysis.Units):
                            value.Units = reader.GetInt32();
                            break;
                        case nameof(UnderwritingAnalysis.Vintage):
                            value.Vintage = reader.GetInt32();
                            break;
                        case nameof(UnderwritingAnalysis.Zip):
                            value.Zip = reader.GetString();
                            break;
                    }
                }
            }

            return value;
        }

        public override void Write(Utf8JsonWriter writer, UnderwritingAnalysis value, JsonSerializerOptions options)
        {
            if(value is null)
            {
                writer.WriteNullValue();
                return;
            }

            writer.WriteStartObject();
            Write(writer, nameof(UnderwritingAnalysis.Address), value.Address);
            WriteNumber(writer, nameof(UnderwritingAnalysis.AquisitionFeePercent), value.AquisitionFeePercent);
            WriteNumber(writer, nameof(UnderwritingAnalysis.AskingPrice), value.AskingPrice);
            Write(writer, nameof(UnderwritingAnalysis.BucketList), value.BucketList, options);
            WriteArray(writer, nameof(UnderwritingAnalysis.CapitalImprovements), value.CapitalImprovements, options);
            WriteNumber(writer, nameof(UnderwritingAnalysis.CapX), value.CapX);
            Write(writer, nameof(UnderwritingAnalysis.CapXType), value.CapXType);
            Write(writer, nameof(UnderwritingAnalysis.City), value.City);
            WriteNumber(writer, nameof(UnderwritingAnalysis.ClosingCostMiscellaneous), value.ClosingCostMiscellaneous);
            WriteNumber(writer, nameof(UnderwritingAnalysis.ClosingCostPercent), value.ClosingCostPercent);
            WriteNumber(writer, nameof(UnderwritingAnalysis.DeferredMaintenance), value.DeferredMaintenance);
            WriteNumber(writer, nameof(UnderwritingAnalysis.DesiredYield), value.DesiredYield);
            WriteNumber(writer, nameof(UnderwritingAnalysis.Downpayment), value.Downpayment);
            WriteNumber(writer, nameof(UnderwritingAnalysis.GrossPotentialRent), value.GrossPotentialRent);
            WriteNumber(writer, nameof(UnderwritingAnalysis.HoldYears), value.HoldYears);
            Write(writer, nameof(UnderwritingAnalysis.Id), value.Id);
            Write(writer, nameof(UnderwritingAnalysis.LoanType), value.LoanType);
            WriteNumber(writer, nameof(UnderwritingAnalysis.LTV), value.LTV);
            WriteNumber(writer, nameof(UnderwritingAnalysis.Management), value.Management);
            Write(writer, nameof(UnderwritingAnalysis.Market), value.Market);
            WriteNumber(writer, nameof(UnderwritingAnalysis.MarketVacancy), value.MarketVacancy);
            WriteArray(writer, nameof(UnderwritingAnalysis.Models), value.Models, options);
            WriteArray(writer, nameof(UnderwritingAnalysis.Mortgages), value.Mortgages, options);
            Write(writer, nameof(UnderwritingAnalysis.Name), value.Name);
            Write(writer, nameof(UnderwritingAnalysis.NeighborhoodClass), value.NeighborhoodClass);
            WriteArray(writer, nameof(UnderwritingAnalysis.Notes), value.Notes, options);
            WriteNumber(writer, nameof(UnderwritingAnalysis.OfferPrice), value.OfferPrice);
            WriteNumber(writer, nameof(UnderwritingAnalysis.OurEquityOfCF), value.OurEquityOfCF);
            WriteArray(writer, nameof(UnderwritingAnalysis.Ours), value.Ours, options);
            WriteNumber(writer, nameof(UnderwritingAnalysis.PhysicalVacancy), value.PhysicalVacancy);
            Write(writer, nameof(UnderwritingAnalysis.PropertyClass), value.PropertyClass);
            WriteNumber(writer, nameof(UnderwritingAnalysis.PurchasePrice), value.PurchasePrice);
            WriteNumber(writer, nameof(UnderwritingAnalysis.RentableSqFt), value.RentableSqFt);
            WriteNumber(writer, nameof(UnderwritingAnalysis.SECAttorney), value.SECAttorney);
            WriteArray(writer, nameof(UnderwritingAnalysis.Sellers), value.Sellers, options);
            Write(writer, nameof(UnderwritingAnalysis.StartDate), value.StartDate);
            Write(writer, nameof(UnderwritingAnalysis.State), value.State);
            Write(writer, nameof(UnderwritingAnalysis.Status), value.Status);
            WriteNumber(writer, nameof(UnderwritingAnalysis.StrikePrice), value.StrikePrice);
            Write(writer, nameof(UnderwritingAnalysis.Timestamp), value.Timestamp);
            Write(writer, nameof(UnderwritingAnalysis.Underwriter), value.Underwriter);
            Write(writer, nameof(UnderwritingAnalysis.UnderwriterEmail), value.UnderwriterEmail);
            WriteNumber(writer, nameof(UnderwritingAnalysis.Units), value.Units);
            WriteNumber(writer, nameof(UnderwritingAnalysis.Vintage), value.Vintage);
            Write(writer, nameof(UnderwritingAnalysis.Zip), value.Zip);

            writer.WriteEndObject();
        }

        private void WriteArray<T>(Utf8JsonWriter writer, string name, IEnumerable<T> collection, JsonSerializerOptions options)
        {
            if (collection is null || !collection.Any())
                return;

            var propertyName = name.Camelize();
            writer.WritePropertyName(propertyName);
            var converter = options.GetConverter<T>();
            writer.WriteStartArray();
            foreach (var item in collection)
            {
                converter.Write(writer, item, options);
            }
            writer.WriteEndArray();

        }

        private void Write<T>(Utf8JsonWriter writer, string name, T value, JsonSerializerOptions options = null)
        {
            var propertyName = name.Camelize();
            if (typeof(ReactiveObject).IsAssignableFrom(typeof(T)))
            {
                writer.WritePropertyName(propertyName);
                if (options is null)
                {
                    throw new Exception($"JsonSerializationOptions not provided to serialize: {name}");
                }
                else
                {
                    var converter = options.GetConverter<T>();
                    converter.Write(writer, value, options);
                }
                return;
            }

            if (value is null)
                return;

            writer.WriteString(propertyName, value.ToString());
        }

        private void WriteNumber(Utf8JsonWriter writer, string name, int value)
        {
            writer.WriteNumber(name.Camelize(), value);
        }

        private void WriteNumber(Utf8JsonWriter writer, string name, double value)
        {
            writer.WriteNumber(name.Camelize(), value);
        }
    }
}
