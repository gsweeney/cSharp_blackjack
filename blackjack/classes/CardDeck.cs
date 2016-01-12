using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blackjack.classes
{
    class CardDeck
    {
        public const int NUMSUITS = 4;// 4 suits
        public const int CARDSPERSUIT = 13;// 13 cards per suit
        public const int CARDSPERDECK = 52;// 13 * 4 = 52 cards per deck
        Random rand = new Random(Guid.NewGuid().GetHashCode());

        /* backing fields */
        private List<Card> _cards;

        /* properties */
        public List<Card> Cards
        {
            get { return _cards; }
            set { _cards = value; }
        }
        
        /* constructor */
        public CardDeck(){
            // initialize deck
            _cards = new List<Card>();
            create_deck();
        }

        
        // add 52 card obects to List deck
        void create_deck() {

            // for each suit
            foreach(Suit s in Enum.GetValues(typeof(Suit)))
            {
                // add 13 cards to the deck
                foreach(Face f in Enum.GetValues(typeof(Face)))
                {
                    Card card = new Card(f.ToString() +"_of_" + s.ToString());
                    card.Suit = s;
                    card.Face = f;

                    if ((int)card.Face > 10)
                    {
                        card.Value = 10;
                    }
                    else {
                        card.Value = (int)card.Face;
                    }
                    Cards.Add(card);
                }
            }
        }


        // shuffle deck (randomly reorder cards)
        public void shuffle() {
            int low = 1;
            int high = Cards.Count;
            int i;
            
            List<Card> shuffled_deck = new List<Card>();
            while(high > low){
            
                // choose a random number between high and low
                i = rand.Next(low, high);

                // add that card index from cards to shuffled_deck
                Card card = Cards[i];
                shuffled_deck.Add(card);
                Cards.RemoveAt(i);

                // decrement value of high
                high--;
            }
            // add last card to shuffled_deck
            shuffled_deck.Add(Cards[0]);

            // set cards = shuffled_deck
            Cards = shuffled_deck;
        }
    }
}
