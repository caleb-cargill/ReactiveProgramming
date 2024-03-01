namespace ReactiveProgramming;

public class RubixCube : PropertyChangedBase, IRubixCube
{
    public List<Cube> Cubes
    {
        get => _cubes; 
        set
        {
            if (value == _cubes) return;    
            _cubes = value;
            OnPropertyChanged();
        }
    }
    private List<Cube> _cubes;

    public int Volume
    {
        get => _volume; 
        set
        {
            if (value == _volume) return;
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
            if (value == _surfaceArea) return;
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
            if (value == _edgeLength) return;
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
            if (value == _height) return;
            _height = value;
            OnPropertyChanged();
        }
    }
    private int _height;
}
