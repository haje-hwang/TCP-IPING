using System.Collections.Generic;

public struct Quiz
{
    public List<Question> questions; // ���� ����Ʈ

    // ������
    public Quiz(List<Question> questions)
    {
        this.questions = questions;
    }
}
