using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardHandler : MonoBehaviour
{

    [SerializeField]
    private Card _card;

    [SerializeField]
    Image _image;

    [SerializeField]
    private TextMeshProUGUI _titleText;

    [SerializeField]
    private TextMeshProUGUI _descriptionText;

    [SerializeField]
    private TextMeshProUGUI _priceText;

    [SerializeField]
    private Deck _deck;

    [SerializeField]
    private ShopHandler _shopHandler;

    public int Price
    {
        get
        {
            if (_card == null)
            {
                return 0;
            }
            return _card.Price;
        }
    }

    public Card Card { 
        get { return _card; } 
        set { 
            _card = value; 
            UpdateCard(); 
        } 
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateCard();
    }

    private void OnEnable()
    {
        UpdateCard();
    }

    private void UpdateCard()
    {
        if (_card == null)
        {
            return;
        }

        if (_image != null)
        {
            _image.sprite = _card.BackGround;
        }

        if (_titleText != null)
        {
            _titleText.text = _card.Title;
        }

        if (_descriptionText != null)
        {
            _descriptionText.text = _card.Description;
        }

        if (_priceText != null)
        {
            _priceText.text = _card.Price.ToString();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnValidate()
    {
        UpdateCard();
    }

    public void ChangeCard()
    {
        if (_deck == null)
        {
            Debug.LogError("Change card requires a valid deck.");
            return;
        }

        HashSet<Card> excludedCards = new HashSet<Card>() { _card };
        if (_shopHandler != null)
        {
            _shopHandler.AddAllCards(excludedCards);
        } else
        {
            Debug.Log("Couldn't find shop handler to get excluded cards.");
            return;
        }

        List<Card> cards = _deck.selectRandomCards(excludedCards, 1);
        if (cards == null || cards.Count == 0)
        {
            Debug.LogError("Could not select random card.");
            return;
        }

        _card = cards[0];

        _shopHandler.UpdateShowDeck();

        UpdateCard();
    }
}
