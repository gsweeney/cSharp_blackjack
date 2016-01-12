using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using blackjack.interfaces;

namespace blackjack.classes
{
    class BlackjackDealer : BlackjackPlayer
    {
        // to access controls on Form
        public AccessFormControls form_controls;

        public int current_card;
        public CardDeck deck = new CardDeck();

        // constructor
        public BlackjackDealer(AccessFormControls form)
        {
            id = 4;
            form_controls = form;
            deck.shuffle();
            current_card = 0;
        }

        // deal 1 card from the card deck 
        public void dealCard(BlackjackPlayer player, int hand_id)
        {
            Console.WriteLine("current_card: " + current_card);// test

            Card card = deck.Cards[current_card];

            if (current_card == 51)
            {
                Console.WriteLine("DEALER RESHUFFLES");// test
                deck.shuffle();
                current_card = 0;
            }
            else
            {
                current_card++;
            }

            // set the offset for the card
            card.x = player.hands[hand_id].x_nextOffset;
            card.y = player.hands[hand_id].y_nextOffset;
            player.hands[hand_id].x_nextOffset += 10;
            player.hands[hand_id].y_nextOffset += 20;

            // if this is dealer's 1st card, set it as face down
            if (player.id == this.id && player.hands[hand_id].cards.Count == 0)
            {
                card.IsFaceDown = true;
            }

            // give Player the card
            player.hands[hand_id].addCard(card);

            // display the card on the form
            form_controls.displayCard(card, player.id);
        }// close function


        // make first deal of the game
        public void first_deal(List<BlackjackPlayer> players)
        {
            // set hand status as a new hand
            this.hands[0].Status = 0;

	        // all active players and dealer get 2 cards
	        for (int i = 0; i < 2; i++)
	        {
                // give BlackJackDealer card
                dealCard(this, 0);// get a card from the deck

                foreach(BlackjackPlayer player in players)
		        {
                    //************************
                    Console.WriteLine("player id: " + player.id + "active: " + player.IsActive);

                    if(player.IsActive){

                        dealCard(player, 0);
                    }
		        }
	        }
        }// end function

        public void completesHand()
        {
            // show dealer bottom card
            form_controls.flipDealerCard(hands[0].cards[0].card_front);

            BlackjackHand currentHand = hands[0];

            hands[0].cards[0].IsFaceDown = false;

            // check hand for blackjack or bust
            currentHand.check();

            // dealer takes hit until sum of cards is at least 17
            while (currentHand.Sum < 17)
            {
                //hitSelf();
                dealCard(this, 0);
                currentHand.check();
            }
        }// close function

    }// close class
}// close namespace
