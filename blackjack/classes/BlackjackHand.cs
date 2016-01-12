using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blackjack.classes
{
    public class BlackjackHand
    {

        // enums
        public enum Result { WIN, LOSE, PUSH};

        /* properties */
        public int Status { get; set; }// -100 = LOSE, -1 = NEW HAND, 0 = ?, 100 = BLACKJACK
	    public double Bet 
        {
            get { return _bet;  }
            set { _bet = value; label_bet.Text = _bet.ToString(); }
        
        }
        public int x_nextOffset { get; set; }
        public int y_nextOffset { get; set; }
        public int Sum 
        {
            get { sumCards(); return sum; }
            //set { sumCards(); } 
        }

  	    public List<Card> cards = new List<Card>(); 
	    int sum;
        double _bet;
        public Result result;

        // display the bet on the hand
        public System.Windows.Forms.Label label_bet;

        public BlackjackHand() {
            label_bet = new System.Windows.Forms.Label();
            Bet = 0;
            Status = -1;
        }

        // return the cards in the hand
	    public List<Card> getCards(){
            return cards;
        }
        // return a single card
	    public Card getCard(int card){
            return cards[card];
        }
        // add card to hand
	    public void addCard(Card card){
            cards.Add(card);
        }

        // check a hand for blackjack or bust
        public void check()
        {
            sumCards();

            // test
            Console.WriteLine("check in BJHand says sum of cards is: " + sum);

            if (Status == -1)
            {// new hand
                Status = 0;
            }
            else
            {// not a new hand
                Status = Sum;
            }

            // if BLACKJACK (sum is 21), then hand wins
            if (sum == BlackjackGame.BLACKJACK)
            {
                Status = 100; // 100 == WIN
            }

            // if BUST (sum > 21), then hand loses
            if (sum > BlackjackGame.BLACKJACK)
            {
                Status = -100; // -100 == LOSE
            }
        }// end function

        // return the sum of the face values
        void sumCards()
        {
            int aces = 0;
            sum = 0;

            // loop through cards and sum them
            for (int i = 0; i < cards.Count; i++)
            {
                if (cards[i].Face == Face.ace)// if card is an ace
                {
                    aces++;// take note and add its value last
                }
                else// add card value to sum
                {
                    sum += cards[i].Value;
                }
            }

            // now, add aces to sum
            while (aces > 0)
            {
                switch (aces)
                {
                    case 1:
                        if (sum <= 10)
                        {
                            sum += 11;
                        }
                        else
                        {
                            sum += 1;
                        }
                        aces--;
                        break;
                    case 2:
                        if (sum <= 9)
                        {
                            sum += 11;
                        }
                        else
                        {
                            sum += 1;
                        }
                        aces--;
                        break;
                    case 3:
                        if (sum <= 8)
                        {
                            sum += 11;
                        }
                        else
                        {
                            sum += 1;
                        }
                        aces--;
                        break;
                    case 4:
                        if (sum <= 7)
                        {
                            sum += 11;
                        }
                        else
                        {
                            sum += 1;
                        }
                        aces--;
                        break;
                    default:
                        Console.WriteLine("BlackjackHand class: problem in sumCards");
                        break;
                }// end switch
            }
            //return sum;
        } 
    }
}
