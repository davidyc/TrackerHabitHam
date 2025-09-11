namespace TrackerHabiHamApi.Models.Dto
{
    public class WeightSummaryDto
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public double? Min { get; set; }
        public double? Max { get; set; }
        public double? Average { get; set; }
        public double? StartValue { get; set; }
        public double? EndValue { get; set; }
        public double? Change { get; set; }
    }

    public class WeightPointDto
    {
        public DateTime Date { get; set; }
        public double? Value { get; set; }
    }
}


