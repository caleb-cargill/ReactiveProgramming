using ReactiveProgramming.Models;

namespace ReactiveProgramming;

public static class DbContext
{
    public static List<Subject> GetSubjects()
        => new List<Subject>()
            {
                new Subject()
                {
                    Name = "Square",
                    Members = new List<Member>()
                    {
                        new Member() { Name ="Side" },
                        new Member() { Name ="Area" },
                        new Member() { Name ="Perimeter" },
                        new Member() { Name ="Diagonal" }
                    }
                },
                new Subject()
                {
                    Name = "Cube",
                    Members = new List<Member>()
                    {
                        new Member() { Name ="Face" },
                        new Member() { Name ="Volume" },
                        new Member() { Name ="SurfaceArea" },
                        new Member() { Name ="EdgeLength" },
                        new Member() { Name ="Height" }
                    }
                },
                new Subject()
                {
                    Name = "RubixCube",
                    Members = new List<Member>()
                    {
                        new Member() { Name ="Cubes" },
                        new Member() { Name ="Volume" },
                        new Member() { Name ="SurfaceArea" },
                        new Member() { Name ="EdgeLength" },
                        new Member() { Name ="Height" }
                    }
                }
            };

    public static List<Calculation> GetCalculations()
        => new List<Calculation>()
        {
            new Calculation()
            {
                Name = "CalculateArea",
                Body = "Area = Side * Side",
                Updates = "Area",
                UpdatesSubject = "Square",
                MemberArguments = new List<(string, string)>() { ("Square", "Side") }
            },
            new Calculation()
            {
                Name = "CalculatePerimeter",
                Body = "Perimeter = 4 * Side",
                Updates = "Perimeter",
                UpdatesSubject = "Square",
                MemberArguments = new List<(string, string)>() { ("Square", "Side") }
            },
            new Calculation()
            {
                Name = "CalculateDiagonal",
                Body = "Diagonal = math.sqrt((Side * Side) + (Side * Side))",
                Updates = "Diagonal",
                UpdatesSubject = "Square",
                MemberArguments = new List<(string, string)>() { ("Square", "Side") }
            },
            new Calculation()
            {
                Name = "CalculateVolume",
                Body = "Volume = Height * Height * Height",
                Updates = "Volume",
                UpdatesSubject = "Cube",
                MemberArguments = new List<(string, string)>() { ("Cube", "Height") }
            },
            new Calculation()
            {
                Name = "CalculateSurfaceArea",
                Body = "SurfaceArea = 6 * Area",
                Updates = "SurfaceArea",
                UpdatesSubject = "Cube",
                MemberArguments = new List<(string, string)>() { ("Square", "Area") }
            },
            new Calculation()
            {
                Name = "CalculateEdgeLength",
                Body = "EdgeLength = 12 * Height",
                Updates = "EdgeLength",
                UpdatesSubject = "Cube",
                MemberArguments = new List<(string, string)>() { ("Cube", "Height") }
            },
            new Calculation()
            {
                Name = "CalculateHeight",
                Body = "Height = Side",
                Updates = "Height",
                UpdatesSubject = "Cube",
                MemberArguments = new List<(string, string)>() { ("Square", "Side") }
            },
            new Calculation()
            {
                Name = "CalculateVolume",
                Body = "Volume = (Height * Height * Height) * 8",
                Updates = "Volume",
                UpdatesSubject = "RubixCube",
                MemberArguments = new List<(string, string)>() { ("Cube", "Height") }
            },
            new Calculation()
            {
                Name = "CalculateSurfaceArea",
                Body = "SurfaceArea = 24 * Area",
                Updates = "SurfaceArea",
                UpdatesSubject = "RubixCube",
                MemberArguments = new List<(string, string)>() { ("Square", "Area") }
            },
            new Calculation()
            {
                Name = "CalculateEdgeLength",
                Body = "EdgeLength = 12 * Height",
                Updates = "EdgeLength",
                UpdatesSubject = "RubixCube",
                MemberArguments = new List<(string, string)>() { ("RubixCube", "Height") }
            },
            new Calculation()
            {
                Name = "CalculateHeight",
                Body = "Height = Side * 2",
                Updates = "Height",
                UpdatesSubject = "RubixCube",
                MemberArguments = new List<(string, string)>() { ("Square", "Side") }
            }
        };
}
