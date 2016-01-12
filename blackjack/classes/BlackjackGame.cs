using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using blackjack.interfaces;

namespace blackjack.classes
{
    class BlackjackGame
    {
        // to access controls on Form
        AccessFormControls form_controls;

        // game constants
        public const int INITIAL_BALANCE = 1000;
	    public const int BLACKJACK = 21;
	    const int MAX_HANDS = 4;
	    const int MAX_PLAYERS = 4;
	    const int MIN_BET = 5;
	    const int MAX_BET = 100;

        public bool GameOver { get; set; }
        public int NumActivePlayers { get; set; }
        public int NumGamePlayers { get; set; }

        // track player and hand being played
        public BlackjackHand currentHand;
        public int cur_player;
        public int cur_hand;

        //players and dealer objects
        public List <BlackjackPlayer> players;// vector to store Player objects
        public BlackjackDealer dealer;// BlackJackDealer controls the game
        
        // contstructor
        public BlackjackGame(AccessFormControls form) {
            // access functions in form class
            form_controls = form;

            // create the dealer
            dealer = new BlackjackDealer(form);

            // create the players
            players = new List<BlackjackPlayer>();

            GameOver = false;
        }

        /// <summary>
        /// 
        /// </summary>
        public void startNewGame() {

            createPlayers(form_controls.NumPlayers);
            resetCurrentHand();
        }

        /// <summary>
        /// 
        /// </summary>
        public void startNewRound(){

            dealer.first_deal(players);

            resetCurrentHand();

            if (!players[cur_player].IsActive){
                findNextActivePlayer();
            }

            currentHand.check();
        }

        /// <summary>
        /// 
        /// </summary>
        public void continuePlaying() 
        {
            // just continue, do not reset game elements
            reset(false);

            resetCurrentHand();

            // set next active player as current player
            if (!players[cur_player].IsActive){
                findNextActivePlayer();
            }     
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stop"></param>
        public void reset(bool stop)
        {
            foreach (BlackjackPlayer player in players){
                player.hands.Clear();// reset HandOfCards
                player.hands.Add(new BlackjackHand());
            }
            dealer.hands.Clear();
            dealer.hands.Add(new BlackjackHand());
      
            if(stop == true){
                players.Clear();
                dealer.deck.shuffle();
                dealer.current_card = 0;
            }
        }

        // set List to store Player objects
        public void createPlayers(string num_players)
        {
            NumGamePlayers = int.Parse(num_players);
            NumActivePlayers = NumGamePlayers;

            // use NumPlayers to create CasinoPlayer objects 
            for (int i = 0; i < NumGamePlayers; i++)
            {
                BlackjackPlayer player = new BlackjackPlayer();       
                player.id = i;
                players.Add(player);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool findNextActivePlayer() {

            bool success = false;
            int next_player = cur_player + 1;

            for (int i = next_player; i < players.Count; i++)
            {
                if(players[i].IsActive){
                    success = true;
                    cur_player = i;
                    currentHand = players[cur_player].hands[cur_hand];
                    break;
                }
            }
            return success;
        }


        // reset current hand to player 1's first hand
        public void resetCurrentHand()
        {
            cur_player = 0;
            cur_hand = 0;
            currentHand = players[cur_player].hands[cur_hand];
        }
        

        // 
        public void gotoNextHand() {

            // check if current player has any more hands (split was made)
            //if (players[cur_player].hands.Count > cur_hand + 1)
            //{
                //cur_hand++;
                //form_controls.update_player();
            //}
            //else 
            //{
                // let them play
                if (findNextActivePlayer()){
                    cur_hand = 0;
                    currentHand.check();

                    // update form
                    form_controls.movePanelPlayerButtons();
                    form_controls.update_player();
                }
                // no active players, finish the round
                else { 
                    finishRound();    
                }
            //}
        }


        // complete the round of play with the dealer
        public void finishRound() {

            // set dealer as current player
            cur_player = dealer.id;
            cur_hand = 0;
            currentHand = dealer.hands[cur_hand];

            dealer.completesHand();

            // update the dealer's hand
            form_controls.update_dealer();

            // show results of player vs dealer
            foreach (BlackjackPlayer player in players){
                if (player.IsActive){
                    showResult(player);
                }
            }

            form_controls.update_finalStatus();
        }


        // check final results
        void showResult(BlackjackPlayer player) 
        {
            double win = 0;

            // compare player hands to dealer
            foreach (BlackjackHand hand in player.hands){

                // set hand to win or lose
                if (currentHand.Status > hand.Status || hand.Status == -100){
                        // player loses
                        hand.result = BlackjackHand.Result.LOSE;
                        
                        // check player balance, if < 5 game over for them
                        NumActivePlayers += player.isActive();

                        hand.label_bet.Text = "";
                }
                else if (currentHand.Status == hand.Status){
                        // push
                        hand.result = BlackjackHand.Result.PUSH;

                        // return bet to player
                        player.Balance += hand.Bet;

                        hand.label_bet.Text = "+" + hand.Bet;

                }
                else{
                        // player wins
                        hand.result = BlackjackHand.Result.WIN;

                        // pay player
                        
                        if(hand.Sum == 21){
                            // 2.5 x bet if blackjack
                            win = (hand.Bet * 2) + (hand.Bet / 2);
                            player.Balance += win;
                            hand.label_bet.Text = "+" + win;
                        }
                        else{
                            // 2 x bet if win
                            win = hand.Bet * 2;
                            player.Balance += win;
                            hand.Bet = win;
                            hand.label_bet.Text = "+" + win;
                        }
                }

                // update label of the hand with results (SUM - WIN/LOSE/PUSH)
                form_controls.displayFinalResult(player.id, hand);
            }// close foreach
        }// close function

        // add a card to a Player's hand
        public void hit()
        {
            // deal card to player
            dealer.dealCard(players[cur_player], cur_hand);
            currentHand.check();
        }

        // Player stands
        public void stand()
        {
            currentHand.check();
            gotoNextHand();
        }
			
        // Player chooses to double down
        public void doubleDown()
        {
            // subtract current bet from player balance
            players[cur_player].Balance -= currentHand.Bet;
     
            // add one card to current hand
	        hit();

            // set bet on that hand to double
            currentHand.Bet *= 2;
        }

        //
        public void setBet(int bet)
        {
            // subtract the bet from player balance
            players[cur_player].Balance -= bet;

            // set the bet of the hand
            players[cur_player].hands[cur_hand].Bet += bet;
        }


        /*
         void split(std::vector<HandOfCards>&, int);

        // 
        void split(vector<HandOfCards>& hands, int current)
        {
	        // make a new hand
	        HandOfCards newHand;

	        // set bet equal to player bet
	        newHand.bet = hands[current].bet;

	        // copy second card from current hand 
	        Card tempCard = hands[current].getCard(1);

	        // add it to the new the hand
	        newHand.addCard(tempCard);

	        // remove second card from the current hand
	        hands[current].cards.pop_back();

	        // BlackJackDealer adds a new card to each hand
	        Card temp1 = myBlackJackDealer.myDeckOfCards.dealCard();// get a card from the deck
	        Card temp2 = myBlackJackDealer.myDeckOfCards.dealCard();// get a card from the deck
	        hands[current].addCard(temp1);	// add a card to the current hand
	        newHand.addCard(temp2);	// add a card to the new hand

	        //LAST! - add new hand to the player's total hands
	        hands.push_back(newHand);
	
	        cout << "\nPlayer's New Hand (from split): "; 
	        cout << hands[current];
	        //hands[current].printHand();
	        //cout << endl;

	        // continue playing the two card hand
        }// end function 
       
          */


    }
}
