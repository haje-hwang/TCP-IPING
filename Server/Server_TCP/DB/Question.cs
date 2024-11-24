using System.Collections.Generic;

public struct Question
{
    public int id { get; set; }
    public string question { get; set; }
    public List<string> options { get; set; }
    public int answer { get; set; }
    public string category { get; set; }
    public string difficulty { get; set; }
}
