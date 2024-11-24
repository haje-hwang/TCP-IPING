using System.Collections.Generic;

    public class Question
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
        public List<string> Options { get; set; }
        public int Answer { get; set; }
        public string Category { get; set; }
        public string Difficulty { get; set; }
    }