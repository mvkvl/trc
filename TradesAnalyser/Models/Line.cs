using WpfCommon;

namespace TradesAnalyser.Charts
{
    public class Line: BaseModel
    {
        private double _x1;
        public double X1 { get => _x1; set { _x1 = value; OnPropertyChanged(nameof(X1)); } }

        private double _y1;
        public double Y1 { get => _y1; set { _y1 = value; OnPropertyChanged(nameof(Y1)); } }

        private double _x2;
        public double X2 { get => _x2; set { _x2 = value; OnPropertyChanged(nameof(X2)); } }

        private double _y2;
        public double Y2 { get => _y2; set { _y2 = value; OnPropertyChanged(nameof(Y2)); } }
    }
}
