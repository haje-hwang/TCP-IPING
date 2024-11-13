using System.Collections.Generic;

public struct Question
{
    public int id; // 퀴즈 ID
    public string question; // 질문
    public List<string> options; // 보기
    public int answer; // 정답 (보기의 인덱스)
    public string category; // 카테고리
    public string difficulty; // 난이도

    // 생성자
    public Question(int id, string question, List<string> options, int answer, string category, string difficulty)
    {
        this.id = id;
        this.question = question;
        this.options = options;
        this.answer = answer;
        this.category = category;
        this.difficulty = difficulty;
    }
}
