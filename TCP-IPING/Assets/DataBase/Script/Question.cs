using System.Collections.Generic;

public struct Question
{
    public int id; // ���� ID
    public string question; // ����
    public List<string> options; // ����
    public int answer; // ���� (������ �ε���)
    public string category; // ī�װ�
    public string difficulty; // ���̵�

    // ������
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
