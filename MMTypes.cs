using System.Collections.Generic;

class Trail
{
    public int Trigger;
    public List<int> Body;
    public int ms_Decay;

    public override string ToString() => $"$ {Trigger} > {string.Join(" > ", Body)} : {ms_Decay} $";
}

class Segment
{
    public int Id;
    public int XOffset;
    public int YOffset;
    public int ms_Build;
    public int ms_Decay;

    public override string ToString()
        => $"({Id} @ ({XOffset}, {YOffset}) : {ms_Build}, {ms_Decay})";
}

class Structure
{
    public int Trigger;
    public List<Segment> Segments; 

    public override string ToString()
        => $"& {Trigger} > {string.Join(" > ", Segments)} &";
}

class MMBundle
{
    public List<Trail> Trails;
    public List<Structure> Structures;
}
