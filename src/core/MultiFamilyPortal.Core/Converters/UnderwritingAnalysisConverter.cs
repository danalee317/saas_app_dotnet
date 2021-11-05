using System.Text.Json;
using System.Text.Json.Serialization;
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

                    switch(name)
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
                            var ourItems = lineItemConverter.Read(ref reader, typeof(List<UnderwritingAnalysis>), options);
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
                            value.Timestamp = reader.GetDateTimeOffset();
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
            writer.WriteString(nameof(UnderwritingAnalysis.Address), value.Address);
            writer.WriteNumber(nameof(UnderwritingAnalysis.AquisitionFeePercent), value.AquisitionFeePercent);
            writer.WriteNumber(nameof(UnderwritingAnalysis.AskingPrice), value.AskingPrice);
            writer.WriteNumber(nameof(UnderwritingAnalysis.CapX), value.CapX);
            writer.WriteString(nameof(UnderwritingAnalysis.CapXType), value.CapXType.ToString());
            writer.WriteString(nameof(UnderwritingAnalysis.City), value.City);
            writer.WriteNumber(nameof(UnderwritingAnalysis.ClosingCostMiscellaneous), value.ClosingCostMiscellaneous);
            writer.WriteNumber(nameof(UnderwritingAnalysis.ClosingCostPercent), value.ClosingCostPercent);
            writer.WriteNumber(nameof(UnderwritingAnalysis.DeferredMaintenance), value.DeferredMaintenance);
            writer.WriteNumber(nameof(UnderwritingAnalysis.Downpayment), value.Downpayment);
            writer.WriteNumber(nameof(UnderwritingAnalysis.GrossPotentialRent), value.GrossPotentialRent);
            writer.WriteString(nameof(UnderwritingAnalysis.Id), value.Id);
            writer.WriteString(nameof(UnderwritingAnalysis.LoanType), value.LoanType.ToString());
            writer.WriteNumber(nameof(UnderwritingAnalysis.LTV), value.LTV);
            writer.WriteNumber(nameof(UnderwritingAnalysis.Management), value.Management);
            writer.WriteString(nameof(UnderwritingAnalysis.Market), value.Market);
            writer.WriteNumber(nameof(UnderwritingAnalysis.MarketVacancy), value.MarketVacancy);

            if(value.Mortgages?.Any() ?? false)
            {
                writer.WritePropertyName(nameof(UnderwritingAnalysis.Mortgages));
                var converter = options.GetConverter<List<UnderwritingAnalysisMortgage>>();
                converter.Write(writer, value.Mortgages.ToList(), options);
            }

            writer.WriteString(nameof(UnderwritingAnalysis.Name), value.Name);

            if(value.Notes?.Any() ?? false)
            {
                writer.WritePropertyName(nameof(UnderwritingAnalysis.Notes));
                var converter = options.GetConverter<List<UnderwritingAnalysisNote>>();
                converter.Write(writer, value.Notes, options);
            }

            writer.WriteNumber(nameof(UnderwritingAnalysis.OfferPrice), value.OfferPrice);
            writer.WriteNumber(nameof(UnderwritingAnalysis.OurEquityOfCF), value.OurEquityOfCF);

            if(value.Ours?.Any() ?? false)
            {
                writer.WritePropertyName(nameof(UnderwritingAnalysis.Ours));
                var converter = options.GetConverter<List<UnderwritingAnalysisLineItem>>();
                converter.Write(writer, value.Ours.ToList(), options);
            }

            writer.WriteNumber(nameof(UnderwritingAnalysis.PhysicalVacancy), value.PhysicalVacancy);
            writer.WriteNumber(nameof(UnderwritingAnalysis.PurchasePrice), value.PurchasePrice);
            writer.WriteNumber(nameof(UnderwritingAnalysis.RentableSqFt), value.RentableSqFt);
            writer.WriteNumber(nameof(UnderwritingAnalysis.SECAttorney), value.SECAttorney);

            if (value.Sellers?.Any() ?? false)
            {
                writer.WritePropertyName(nameof(UnderwritingAnalysis.Sellers));
                var converter = options.GetConverter<List<UnderwritingAnalysisLineItem>>();
                converter.Write(writer, value.Sellers.ToList(), options);
            }

            writer.WriteString(nameof(UnderwritingAnalysis.State), value.State);
            writer.WriteString(nameof(UnderwritingAnalysis.Status), value.Status.ToString());
            writer.WriteNumber(nameof(UnderwritingAnalysis.StrikePrice), value.StrikePrice);
            writer.WriteString(nameof(UnderwritingAnalysis.Timestamp), value.Timestamp);
            writer.WriteString(nameof(UnderwritingAnalysis.Underwriter), value.Underwriter);
            writer.WriteString(nameof(UnderwritingAnalysis.UnderwriterEmail), value.UnderwriterEmail);
            writer.WriteNumber(nameof(UnderwritingAnalysis.Units), value.Units);
            writer.WriteNumber(nameof(UnderwritingAnalysis.Vintage), value.Vintage);
            writer.WriteString(nameof(UnderwritingAnalysis.Zip), value.Zip);

            writer.WriteEndObject();
        }
    }
}
