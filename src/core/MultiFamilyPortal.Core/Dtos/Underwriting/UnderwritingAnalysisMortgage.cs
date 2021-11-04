﻿using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace MultiFamilyPortal.Dtos.Underwrting
{
    public class UnderwritingAnalysisMortgage : ReactiveObject
    {
        public UnderwritingAnalysisMortgage()
        {
            this.WhenAnyValue(x => x.LoanAmount, x => x.InterestRate, x => x.TermInYears, x => x.InterestOnly, x => x.BalloonMonths, CalculatePayment)
                .ToProperty(this, nameof(AnnualDebtService), out _annualDebtServiceHelper);

            this.WhenAnyValue(x => x.LoanAmount, x => x.Points, (l, p) => l * p)
                .ToProperty(this, nameof(PointCost), out _pointsCost);
        }

        public Guid Id { get; set; }

        [Reactive]
        public double LoanAmount { get; set; }

        [DisplayFormat(DataFormatString = "{0:P}")]
        [Reactive]
        public double InterestRate { get; set; } = 0.04;

        [Reactive]
        public int TermInYears { get; set; } = 30;

        [Reactive]
        public bool InterestOnly { get; set; }

        [Reactive]
        public int BalloonMonths { get; set; }

        [Reactive]
        [DisplayFormat(DataFormatString = "{0:P}")]
        public double Points { get; set; } = 0.01;

        private ObservableAsPropertyHelper<double> _pointsCost;
        [JsonIgnore]
        public double PointCost => _pointsCost.Value;

        private ObservableAsPropertyHelper<double> _annualDebtServiceHelper;
        [JsonIgnore]
        public double AnnualDebtService => _annualDebtServiceHelper.Value;

        private static double CalculatePayment(double LoanAmount, double InterestRate, int TermInYears, bool InterestOnly, int balloonMonths)
        {
            var termOfLoan = TermInYears * 12;

            if (InterestOnly)
            {
                return LoanAmount * InterestRate;
            }
            else
            {
                var payment = LoanAmount * (Math.Pow((1 + InterestRate / 12), termOfLoan) * InterestRate) / (12 * (Math.Pow((1 + InterestRate / 12), termOfLoan) - 1));

                return payment * 12;
            }
        }
    }
}