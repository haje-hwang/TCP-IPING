using System.Collections.Generic;

public struct Quiz
{
    public List<Question> questions; // 퀴즈 리스트

    // 생성자
    public Quiz(List<Question> questions)
    {
        this.questions = questions;
    }
}
