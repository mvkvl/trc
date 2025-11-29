using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using WpfCommon;

namespace TradesAnalyser.Charts
{
    public class Chart: BaseModel
    {
        private readonly object _lock = new object();

        private double _canvasWidth = 293;
        public double CanvasWidth { get => _canvasWidth; set { _canvasWidth = value; OnPropertyChanged(nameof(CanvasWidth)); MessageBox.Show($"WIDTH: {_canvasWidth}");  } }

        private double _canvasHeight = 220; 
        public double CanvasHeight { get => _canvasHeight; set { _canvasHeight = value; OnPropertyChanged(nameof(CanvasHeight)); } }

        public ObservableCollection<Line> Lines { get; set; }

        public Chart()
        {
            Lines = new ObservableCollection<Line>();
            BindingOperations.EnableCollectionSynchronization(Lines, _lock);
        }
        public void Clear()
        {
            Lines.Clear();
            OnPropertyChanged(nameof(Lines));
        }
        public void Draw(List<decimal> values)
        {
            Task.Run(async () =>
            {
                doDraw(values);
            });
        }
        private void doDraw(List<decimal> values)
        {
            if (values == null || values.Count == 0)
            {
                Lines.Clear();
                OnPropertyChanged(nameof(Lines));
                return;
            }
            decimal maxEquity = values.Max();
            decimal minEquity = values.Min();
            double sx = _canvasWidth / values.Count;
            double ky = (double)(maxEquity - minEquity) / _canvasHeight;
            double x = 0, y = 0, y2;
            lock (_lock)
            {
                Lines.Clear();
            }
            for (int i = 0; i < values.Count - 1; i++)
            {
                y = _canvasHeight - (double)(values[i] - minEquity) / ky;
                y2 = _canvasHeight - (double)(values[i + 1] - minEquity) / ky;
                Line ln = new Line()
                {
                    X1 = x,
                    Y1 = y,
                    X2 = x + sx,
                    Y2 = y2,
                };
                lock (_lock)
                {
                    Lines.Add(ln);
                }
                x += sx;
            }
            OnPropertyChanged(nameof(Lines));
        }
    }
}
