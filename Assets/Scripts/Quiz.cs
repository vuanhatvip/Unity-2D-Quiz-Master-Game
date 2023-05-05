using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Quiz : MonoBehaviour
{
    [Header("Questions")]
    [SerializeField]
    TextMeshProUGUI questionText;
    [SerializeField]
    List<QuestionSO> questions = new List<QuestionSO>();
    QuestionSO currentQuestion;

    [Header("Answer")]
    [SerializeField]
    GameObject[] answerButton;

    [Header("Button Colors")]
    [SerializeField]
    Sprite defaultAnswerSpirte;
    [SerializeField]
    Sprite correctAnswerSprite;

    [Header("Timer")]
    [SerializeField]
    Image timerImage;

    [Header("Scoring")]
    [SerializeField] 
    TextMeshProUGUI scoreText;
    ScoreKeeper scoreKeeper;

    [Header("Progress Bar")]
    [SerializeField] 
    Slider progressBar;
    public bool isComplete;

    int correctAnswerIndex;
    bool hasAnsweredEarly = true;
    TImer timer;

    // Start is called before the first frame update
    void Awake()
    {
        timer = FindObjectOfType<TImer>();
        scoreKeeper = FindObjectOfType<ScoreKeeper>();
        progressBar.maxValue = questions.Count;
        progressBar.value = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        timerImage.fillAmount = timer.fillFraction;
        if (timer.loadNextQuestion)
        {
            if (progressBar.value == progressBar.maxValue)
            {
                isComplete = true;
                return;
            }

            hasAnsweredEarly = false;
            GetNextQuestion();
            timer.loadNextQuestion = false;
        }
        else if (!hasAnsweredEarly && !timer.isAnsweringQuestion)
        {
            DisplayAnswer(-1);
            SetButtonState(false);
        }
    }

    public void OnAnswerSelected(int index)
    {
        hasAnsweredEarly = true;
        DisplayAnswer(index);
        SetButtonState(false);
        timer.CancelTimer();
        scoreText.text = "Score: " + scoreKeeper.CalculateScore() + "%";
    }

    private void DisplayAnswer(int index)
    {
        Image buttonImage;

        if (index == currentQuestion.GetCorrectAnswerIndex())
        {
            questionText.text = "Correct";
            buttonImage = answerButton[index].GetComponent<Image>();
            buttonImage.sprite = correctAnswerSprite;
            scoreKeeper.IncrementCorrectAnswer();
        }
        else
        {
            correctAnswerIndex = currentQuestion.GetCorrectAnswerIndex();
            string correctAnswer = currentQuestion.GetAnswer(correctAnswerIndex);
            questionText.text = "Too bad the correct answer is " + correctAnswer;
            buttonImage = answerButton[correctAnswerIndex].GetComponent<Image>();
            buttonImage.sprite = correctAnswerSprite;
        }
    }

    void GetNextQuestion()
    {
        if (questions.Count > 0)
        {
            SetButtonState(true);
            SetDefaultButtonSprites();
            GetRandomQuestion();
            DisplayQuestion();
            progressBar.value++;
            scoreKeeper.IncrementQuestionsSeen();
        }
    }

    void GetRandomQuestion()
    {
        int index = Random.Range(0, questions.Count);
        currentQuestion = questions[index];

        if (questions.Contains(currentQuestion))
        {
            questions.Remove(currentQuestion);
        }
    }

    void DisplayQuestion()
    {
        questionText.text = currentQuestion.GetQuestion();
        for (int i = 0; i < answerButton.Length; i++)
        {
            TextMeshProUGUI buttonText = answerButton[i].GetComponentInChildren<TextMeshProUGUI>();
            buttonText.text = currentQuestion.GetAnswer(i);
        }
    }

    void SetButtonState(bool state)
    {
        for (int i = 0; i < answerButton.Length; i++)
        {
            Button button = answerButton[i].GetComponent<Button>();
            button.interactable = state;
        }
    }

    void SetDefaultButtonSprites()
    {
        for (int i = 0; i < answerButton.Length; i++)
        {
            Image buttonSprite = answerButton[i].GetComponent<Image>();
            buttonSprite.sprite = defaultAnswerSpirte;
        }
    }
}
