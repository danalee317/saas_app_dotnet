using System.Text.Json;
using System.Text.Json.Serialization;
using Humanizer;
using MultiFamilyPortal.Data.Models;
using MultiFamilyPortal.Dtos.Underwrting;

namespace MultiFamilyPortal.Converters
{
    internal class UnderwritingAnalysisConverter : JsonConverter<UnderwritingAnalysis>
    {
        public override UnderwritingAnalysis Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = new UnderwritingAnalysis();

            var lineItemConverter = options.GetConverter<List<UnderwritingAnalysisLineItem>>();
            while (reader.Read())
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
                        case nameof(UnderwritingAnalysis.Downpayment):
                            value.Downpayment = reader.GetDouble();
                            break;
                        case nameof(UnderwritingAnalysis.GrossPotentialRent):
                            value.GrossPotentialRent = reader.GetDouble();
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
                        case nameof(UnderwritingAnalysis.Mortgages):
                            var mortgageConverter = options.GetConverter<List<UnderwritingAnalysisMortgage>>();
                            var mortgages = mortgageConverter.Read(ref reader, typeof(List<UnderwritingAnalysisMortgage>), options);
                            value.AddMortgages(mortgages);
                            break;
                        case nameof(UnderwritingAnalysis.Name):
                            value.Name = reader.GetString();
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
            writer.WriteStartObject();
            Write(writer, nameof(UnderwritingAnalysis.Address), value.Address);
            WriteNumber(writer, nameof(UnderwritingAnalysis.AquisitionFeePercent), value.AquisitionFeePercent);
            WriteNumber(writer, nameof(UnderwritingAnalysis.AskingPrice), value.AskingPrice);
            WriteNumber(writer, nameof(UnderwritingAnalysis.CapX), value.CapX);
            Write(writer, nameof(UnderwritingAnalysis.CapXType), value.CapXType.ToString());
            Write(writer, nameof(UnderwritingAnalysis.City), value.City);
            WriteNumber(writer, nameof(UnderwritingAnalysis.ClosingCostMiscellaneous), value.ClosingCostMiscellaneous);
            WriteNumber(writer, nameof(UnderwritingAnalysis.ClosingCostPercent), value.ClosingCostPercent);
            WriteNumber(writer, nameof(UnderwritingAnalysis.DeferredMaintenance), value.DeferredMaintenance);
            WriteNumber(writer, nameof(UnderwritingAnalysis.Downpayment), value.Downpayment);
            WriteNumber(writer, nameof(UnderwritingAnalysis.GrossPotentialRent), value.GrossPotentialRent);
            Write(writer, nameof(UnderwritingAnalysis.Id), value.Id);
            Write(writer, nameof(UnderwritingAnalysis.LoanType), value.LoanType.ToString());
            WriteNumber(writer, nameof(UnderwritingAnalysis.LTV), value.LTV);
            WriteNumber(writer, nameof(UnderwritingAnalysis.Management), value.Management);
            Write(writer, nameof(UnderwritingAnalysis.Market), value.Market);
            WriteNumber(writer, nameof(UnderwritingAnalysis.MarketVacancy), value.MarketVacancy);

            if(value.Mortgages?.Any() ?? false)
            {
                writer.WritePropertyName(nameof(UnderwritingAnalysis.Mortgages).Pascalize());
                writer.WriteStartArray();
                var converter = options.GetConverter<UnderwritingAnalysisMortgage>();
                foreach(var mortgage in value.Mortgages)
                {
                    writer.WriteStartObject();
                    converter.Write(writer, mortgage, options);
                    writer.WriteEndObject();
                }
                writer.WriteEndArray();
            }

            writer.WriteString(nameof(UnderwritingAnalysis.Name), value.Name);

            if(value.Notes?.Any() ?? false)
            {
                writer.WritePropertyName(nameof(UnderwritingAnalysis.Notes).Pascalize());
                var converter = options.GetConverter<UnderwritingAnalysisNote>();
                writer.WriteStartArray();
                foreach(var note in value.Notes)
                {
                    writer.WriteStartObject();
                    converter.Write(writer, note, options);
                    writer.WriteEndObject();
                }
                writer.WriteEndArray();
            }

            WriteNumber(writer, nameof(UnderwritingAnalysis.OfferPrice), value.OfferPrice);
            WriteNumber(writer, nameof(UnderwritingAnalysis.OurEquityOfCF), value.OurEquityOfCF);

            if(value.Ours?.Any() ?? false)
            {
                writer.WritePropertyName(nameof(UnderwritingAnalysis.Ours).Pascalize());
                var converter = options.GetConverter<UnderwritingAnalysisLineItem>();

                writer.WriteStartArray();
                foreach(var line in value.Ours)
                {
                    writer.WriteStartObject();
                    converter.Write(writer, line, options);
                    writer.WriteEndObject();
                }
                writer.WriteEndArray();
            }

            WriteNumber(writer, nameof(UnderwritingAnalysis.PhysicalVacancy), value.PhysicalVacancy);
            WriteNumber(writer, nameof(UnderwritingAnalysis.PurchasePrice), value.PurchasePrice);
            WriteNumber(writer, nameof(UnderwritingAnalysis.RentableSqFt), value.RentableSqFt);
            WriteNumber(writer, nameof(UnderwritingAnalysis.SECAttorney), value.SECAttorney);

            if (value.Sellers?.Any() ?? false)
            {
                writer.WritePropertyName(nameof(UnderwritingAnalysis.Sellers).Pascalize());
                var converter = options.GetConverter<UnderwritingAnalysisLineItem>();

                writer.WriteStartArray();
                foreach (var line in value.Sellers)
                {
                    writer.WriteStartObject();
                    converter.Write(writer, line, options);
                    writer.WriteEndObject();
                }
                writer.WriteEndArray();
            }

            Write(writer, nameof(UnderwritingAnalysis.State), value.State);
            Write(writer, nameof(UnderwritingAnalysis.Status), value.Status.ToString());
            WriteNumber(writer, nameof(UnderwritingAnalysis.StrikePrice), value.StrikePrice);
            Write(writer, nameof(UnderwritingAnalysis.Timestamp), value.Timestamp);
            Write(writer, nameof(UnderwritingAnalysis.Underwriter), value.Underwriter);
            Write(writer, nameof(UnderwritingAnalysis.UnderwriterEmail), value.UnderwriterEmail);
            WriteNumber(writer, nameof(UnderwritingAnalysis.Units), value.Units);
            WriteNumber(writer, nameof(UnderwritingAnalysis.Vintage), value.Vintage);
            Write(writer, nameof(UnderwritingAnalysis.Zip), value.Zip);

            writer.WriteEndObject();
        }

        private void Write(Utf8JsonWriter writer, string name, object value)
        {
            if (value is null)
                return;

            var propertyName = name.Pascalize();
            writer.WriteString(propertyName, value.ToString());
        }

        private void WriteNumber(Utf8JsonWriter writer, string name, int value)
        {
            writer.WriteNumber(name.Pascalize(), value);
        }

        private void WriteNumber(Utf8JsonWriter writer, string name, double value)
        {
            writer.WriteNumber(name.Pascalize(), value);
        }

        private IEnumerable<UnderwritingAnalysisLineItem> ReadLineItems(ref Utf8JsonReader reader)
        {
            var items = new List<UnderwritingAnalysisLineItem>();
            UnderwritingAnalysisLineItem item = null;
            while(reader.Read() && reader.TokenType != JsonTokenType.EndArray)
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                    items.Add(item);
                else if (reader.TokenType == JsonTokenType.StartObject)
                    item = new();
                else if(reader.TokenType == JsonTokenType.PropertyName)
                {
                    var name = reader.GetString();
                    reader.Read();
                    if (reader.TokenType == JsonTokenType.Null)
                    {
                        continue;
                    }


                }
            }

            return items;
        }
    }
}
