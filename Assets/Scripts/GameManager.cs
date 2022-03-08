using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

//////////////////////////////////////////////
//Assignment/Lab/Project: Hangman
//Name: Logan Hickman
//Section: 2021SP.SGD.213.2101
//Instructor: Aurore Wold
//Date: 01/24/2022
/////////////////////////////////////////////
public class GameManager : MonoBehaviour
{
    /// <summary>
    /// The 12 sprites to indicate wrong guesses
    /// </summary>
    [SerializeField]
    private Sprite[] failImages;
    /// <summary>
    /// All possible words that could be selected for the player to try to guess
    /// </summary>
    private string[] possibleWords;
    /// <summary>
    /// The single word chosen from possibleWords that is part of the active game
    /// </summary>
    private string wordToGuess;
    /// <summary>
    /// The text field showing the guesses the player has made so far
    /// </summary>
    [SerializeField]
    private TMP_Text guessText;
    /// <summary>
    /// The text field showing the word
    /// </summary>
    [SerializeField]
    private TMP_Text wordText;
    /// <summary>
    /// The text field providing direct feedback to the player for any messages
    /// </summary>
    [SerializeField]
    private TMP_Text feedbackText;
    /// <summary>
    /// Image field showing sprites indicating failed guesses
    /// </summary>
    [SerializeField]
    private Image failDisplay;
    /// <summary>
    /// Used to enable and disable when the player can restart the game
    /// </summary>
    [SerializeField]
    private Button replayButton;
    /// <summary>
    /// List of all letters that the player has tried
    /// </summary>
    private List<char> guessedLetters;
    /// <summary>
    /// Number of times the player guessed wrong
    /// </summary>
    private int failedGuesses = 0;
    /// <summary>
    /// The three possible game states
    /// </summary>
    private enum GameStatus { Win, Loss, Ongoing };

    // Start is called before the first frame update
    void Start()
    {
        possibleWords = File.ReadAllLines(Path.Combine(Path.GetFullPath(Application.dataPath), "words.txt"));
        PlayGame();
    }

    /// <summary>
    /// Start game and resets all values
    /// </summary>
    public void PlayGame()
    {
        failedGuesses = 0;
        wordToGuess = possibleWords[Random.Range(0, possibleWords.Length)].ToUpper();
        Debug.Log("The new word to guess is \"" + wordToGuess + "\"");
        guessText.text = "Preivous Guesses: ";
        guessedLetters = new List<char>();
        UpdateWordHintDisplay();
        feedbackText.text = "";
        UpdateFailDisplay();
        replayButton.interactable = false;
    }

    /// <summary>
    /// Handles what happens when a player guesses a letter
    /// </summary>
    public void GuessLetter(TMP_InputField toGuess)
    {
        feedbackText.text = "";
        try
        {
            char guess = char.Parse(toGuess.text.ToUpper());

            if(guess == '1' || guess == '2' || guess == '3' || guess == '4' || guess == '5' ||
               guess == '6' || guess == '7' || guess == '8' || guess == '9' || guess == '0')
            {
                feedbackText.text = toGuess.text.ToUpper() + " is not a valid character from A-Z. Please choose a letter.";
            }
            else if(guessedLetters.Contains(guess))
            {
                feedbackText.text = "You already guessed " + guess + ". Try another letter.";
            }
            else
            {
                guessedLetters.Add(guess);
                if(CheckGuess(guess))
                {
                    feedbackText.text = "Good job, " + guess + " is in the word!";
                }
                else
                {
                    feedbackText.text = "The letter " + guess + " is not in the word, try again!";
                }

                CheckAllGuesses();
                UpdateGuessesDisplay();
                UpdateWordHintDisplay();
                UpdateFailDisplay();
            }
            
        }
        catch
        {
            feedbackText.text = "\"" + toGuess.text.ToUpper() + "\" is not a valid single character from A-Z. Please choose a single character.";
        }
        finally
        {
            toGuess.text = "";
        }

        CheckEndGame();
    }

    /// <summary>
    /// Checks to see what status the game is currently in
    /// </summary>
    /// <returns>Current status of game</returns>
    private GameStatus CheckGameWinStatus()
    {
        if(!wordText.text.Contains("_"))
        {
            return GameStatus.Win;
        }
        else if(failedGuesses > 11)
        {
            return GameStatus.Loss;
        }
        else
        {
            return GameStatus.Ongoing;
        }
    }

    /// <summary>
    /// Handles winning and losing
    /// </summary>
    private void CheckEndGame()
    {
        GameStatus status = CheckGameWinStatus();

        if(status == GameStatus.Win)
        {
            feedbackText.text = "Congrats, you win! The word was " + wordToGuess + ". You had " + (12 - failedGuesses) + " guesses left. Play again?";
        }
        if(status == GameStatus.Loss)
        {
            feedbackText.text = "You lost! The word was " + wordToGuess + ".  Play again?";
        }
        if(status != GameStatus.Ongoing)
        {
            replayButton.interactable = true;
        }
    }
    
    /// <summary>
    /// Updates what letters the player has guessed
    /// </summary>
    private void UpdateGuessesDisplay()
    {
        string result = "Previous Guesses: ";
        foreach(char c in guessedLetters)
        {
            result = result + c + ", ";
        }
        Debug.Log(result);
        result = result.Substring(0, result.Length - 2);
        Debug.Log(result);

        guessText.text = result;
        Debug.Log(guessText);
        Debug.Log(guessText.text);
    }

    /// <summary>
    /// Updates the hangman display to the appropriate sprite
    /// </summary>
    private void UpdateFailDisplay()
    {
        if(failedGuesses == 0)
        {
            failDisplay.sprite = null;
        }
        else
        {
            failDisplay.sprite = failImages[failedGuesses - 1];
        }        
    }

    /// <summary>
    /// Checks the entire list of guesses to see which are right and wrong
    /// </summary>
    private void CheckAllGuesses()
    {
        failedGuesses = 0;

        foreach (char guessed in guessedLetters)
        {
            if(!CheckGuess(guessed))
            {
                failedGuesses++;
            }
        }
    }

    /// <summary>
    /// Checks to see if a <paramref name="guess"/> is a part of the word
    /// </summary>
    /// <param name="guess">What char should we check?</param>
    /// <returns>Was <paramref name="guess"/> a part of wordToGuess</returns>
    private bool CheckGuess(char guess)
    {
        foreach (char c in wordToGuess.ToCharArray())
        {
            if (guess == c)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Updates the text field displaying to word hint to what letters have been guessed and which still need to be guessed
    /// </summary>
    private void UpdateWordHintDisplay()
    {
        string result = "";
        bool isCorrect = false;

        foreach(char c in wordToGuess.ToCharArray())
        {
            if(guessedLetters.Count <= 0)
            {
                result += " ___ ";
            }
            else
            {
                isCorrect = false;
                foreach (char guessed in guessedLetters)
                {
                    if (c == guessed)
                    {
                        result += c;
                        isCorrect = true;
                        continue;
                    }
                }

                if(!isCorrect)
                {
                    result += " ___ ";
                }
            }            
        }

        wordText.text = result;
    }

    /// <summary>
    /// Quits the game
    /// </summary>
    public void ExitGame()
    {
        Application.Quit();
    }
}
