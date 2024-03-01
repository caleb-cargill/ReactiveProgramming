namespace ReactiveProgramming;

public class Cube : PropertyChangedBase, ICube
{
    public Square Face
    {          
        get => _side;
        set
        {
            if (_side == value) return;
            _side = value;
            OnPropertyChanged();
        }
    }
    private Square _side;

    public int Volume
    {
        get => _volume;
        set
        {
            if (_volume == value) return;
            _volume = value;
            OnPropertyChanged();
        }   
    }
    private int _volume;

    public int SurfaceArea
    {
        get => _surfaceArea;
        set
        {
            if (_surfaceArea == value) return;  
            _surfaceArea = value;
            OnPropertyChanged();
        }
    }
    private int _surfaceArea;

    public int EdgeLength
    {
       get => _edgeLength;
        set
        {
            if (_edgeLength == value) return;
            _edgeLength = value;
            OnPropertyChanged();
        }
    }
    private int _edgeLength;

    public int Height
    {
        get => _height;
        set
        {
            if (_height != value) 
            {
                _height = value;
                OnPropertyChanged();
            }
        }   
    }
    private int _height;
}
