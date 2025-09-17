namespace TrackerHabiHamApi.Models.Dto
{
    public class WeightSummaryDto
    {
        public DateOnly Start { get; set; }
        public DateOnly End { get; set; }
        public double? Min { get; set; }
        public double? Max { get; set; }
        public double? Average { get; set; }
        public double? StartValue { get; set; }
        public double? EndValue { get; set; }
        public double? Change { get; set; }
    }

    public class WeightPointDto
    {
        public DateOnly Date { get; set; }
        public double? Value { get; set; }
    }
}


