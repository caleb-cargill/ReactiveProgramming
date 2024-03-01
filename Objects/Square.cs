namespace ReactiveProgramming;

public class Square : PropertyChangedBase, ISquare
{
    public int Side
    {
        get => _side;
        set
        {
            if (_side == value) return;
            _side = value;
            OnPropertyChanged();
        }
    }
    private int _side;

    public int Area
    {
        get => _area;
        set
        {
            if (_area == value) return;
            _area = value;
            OnPropertyChanged();
        }
    }   
    private int _area;

    public int Perimeter
    {
        get => _perimeter;
        set
        {
            if (_perimeter == value) return;
            _perimeter = value;
            OnPropertyChanged();
        }
    }
    private int _perimeter;

    public double Diagonal
    {
        get => _diagonal;
        set
        {
            if (_diagonal == value) return;
            _diagonal = value;
            OnPropertyChanged();
        }
    }
    private double _diagonal;
}
