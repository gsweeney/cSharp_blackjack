using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using blackjack.classes;
using blackjack.interfaces;
using System.Media;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Drawing.Text;


namespace blackjack
{

    public partial class Blackjack_Form : Form, AccessFormControls
    {
        // game elements
        BlackjackGame game;

        // for control panel movement
        int[] x_loc;
        int y_loc;

        List<Label> labels_hand = new List<Label>();
        List<Label> labels_balance = new List<Label>();
        List<FlowLayoutPanel> panels_bets = new List<FlowLayoutPanel>();
        List<GroupBox> groupboxes = new List<GroupBox>();
        List<TextBox> playernames = new List<TextBox>();

        // fonts
        PrivateFontCollection pfc = new PrivateFontCollection();

        /* properties */
        public string NumPlayers
        {
            get { return comboBox_playerSelect.Text; }
            set { comboBox_playerSelect.Text = value; }
        }

        void addFonts() {
            // specify embedded resource name
            string resource = "embedded_font.KOMTITA.TTF";

            // receive resource stream
            Stream fontStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource);

            // create an unsafe memory block for the font data
            System.IntPtr data = Marshal.AllocCoTaskMem((int)fontStream.Length);

            // create a buffer to read in to
            byte[] fontdata = new byte[fontStream.Length];

            // read the font data from the resource
            fontStream.Read(fontdata, 0, (int)fontStream.Length);

            // copy the bytes to the unsafe memory block
            Marshal.Copy(fontdata, 0, data, (int)fontStream.Length);

            // pass the font to the font collection
            pfc.AddMemoryFont(data, (int)fontStream.Length);

            // close the resource stream
            fontStream.Close();

            // free up the unsafe memory
            Marshal.FreeCoTaskMem(data);

            label_caption1.Font = new Font(pfc.Families[0], 16, FontStyle.Regular);
            label_caption2.Font = new Font(pfc.Families[0], 16, FontStyle.Regular);
        }

        /// <summary>
        /// constructor
        /// </summary>
        public Blackjack_Form()
        {
            InitializeComponent();

            //addFonts();

            game = new BlackjackGame(this);

            // set-up form controls
            x_loc = new int[4]{870, 576, 282, -12};
            y_loc = 644;

            button_reset.Enabled = false;
            display_playerControls(false, 0);

            groupboxes.Add(groupBox_p1);
            groupboxes.Add(groupBox_p2);
            groupboxes.Add(groupBox_p3);
            groupboxes.Add(groupBox_p4);
            groupboxes.Add(groupBox_dealer);

            labels_hand.Add(label_h1);
            labels_hand.Add(label_h2);
            labels_hand.Add(label_h3);
            labels_hand.Add(label_h4);
            labels_hand.Add(label_hd);

            labels_balance.Add(label_b1);
            labels_balance.Add(label_b2);
            labels_balance.Add(label_b3);
            labels_balance.Add(label_b4);

            panels_bets.Add(flowPanel_1);
            panels_bets.Add(flowPanel_2);
            panels_bets.Add(flowPanel_3);
            panels_bets.Add(flowPanel_4);

            playernames.Add(textBox1);
            playernames.Add(textBox2);
            playernames.Add(textBox3);
            playernames.Add(textBox4);



        }

        /// <summary>
        /// display a card on the form
        /// </summary>
        /// <param name="card"></param>
        /// <param name="id"></param>
        public void displayCard(Card card, int id) 
        {
            Card new_card = new Card(card.Face.ToString() +"_of_" + card.Suit.ToString());
            new_card.Location = new Point(card.x, card.y); 

            // show card front or back ?
            if (card.IsFaceDown && id == game.dealer.id){
                new_card.BackgroundImage = card.card_back;
            }
            else {
                new_card.BackgroundImage = card.card_front;
            }

            // add picture box to the correct groupBox      
            groupboxes[id].Controls.Add(new_card);
            new_card.BringToFront();
        }

        /// <summary>
        ///  display available buttons to player
        /// </summary>
        /// <param name="dd"></param>
        private void displayPossibleActions(int dd)
        {
            // set visibility of player buttons for hand
            display_playerControls(true, 1);

            // set buttons as enabled or not
            button_stand.Enabled = true;
            button_hit.Enabled = true;
            if (dd == 0)
            {
                button_double.Enabled = false;
            }
            else
            {
                button_double.Enabled = true;
                // TO DO: give option to split if card values are the same
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void display_betButtons()
        {

            //label_caption1.ForeColor = System.Drawing.Color.DarkBlue;
            //label_caption2.ForeColor = System.Drawing.Color.DarkBlue;
            label_caption1.Text = "MAKE YOUR BET";
            label_caption2.Text = playernames[game.cur_player].Text.ToUpper();
            label_caption1.Visible = true;
            label_caption2.Visible = true;
            pictureBox_caption.Visible = true;

            movePanelPlayerButtons();

            // display betting controls under player currently betting
            display_playerControls(true, 0);

            // add label_bet to player's betting groupBox
            Label temp = game.players[game.cur_player].hands[game.cur_hand].label_bet;

            panels_bets[game.cur_player].Controls.Add(temp);
            panel_playerButtons.Focus();
        }

        /// <summary>
        ///  set visibility for buttons in player control panel
        /// </summary>
        /// <param name="b"></param>
        /// <param name="c"></param>
        void display_playerControls(bool b, int c)
        {
            panel_playerButtons.Visible = b;

            switch(c){
                case 1:
                    button_stand.Visible = b;
                    button_hit.Visible = b;
                    button_double.Visible = b;
                    button_split.Visible = b;
                    button_bet100.Visible = !b;
                    button_bet25.Visible = !b;
                    button_bet5.Visible = !b;
                    button_betALL.Visible = !b;
                    break;
                case 0:
                    button_stand.Visible = !b;
                    button_hit.Visible = !b;
                    button_double.Visible = !b;
                    button_split.Visible = !b;
                    button_bet100.Visible = b;
                    button_bet25.Visible = b;
                    button_bet5.Visible = b;
                    button_betALL.Visible = b;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        ///  display final result of given hand
        /// </summary>
        /// <param name="player_id"></param>
        /// <param name="hand"></param>
        public void displayFinalResult(int player_id, BlackjackHand hand)
        {

            labels_hand[player_id].Text = hand.Sum + " - " + hand.result.ToString();

            if (hand.result == BlackjackHand.Result.WIN)
            {
                labels_hand[player_id].ForeColor = System.Drawing.Color.GreenYellow;
            }
            else
            {
                labels_hand[player_id].ForeColor = System.Drawing.Color.White;
            }
        }

        /// <summary>
        /// add or remove highlight to a hand of cards
        /// </summary>
        /// <param name="Y_N"></param>
        void highlight(int Y_N)
        {
            // use generic var to store collection of PictureBox objects
            var pbs = groupboxes[game.cur_player].Controls.OfType<PictureBox>();

            if (Y_N == 0)
            {
                foreach (PictureBox pb in pbs)
                {
                    pb.BorderStyle = System.Windows.Forms.BorderStyle.None;
                }
            }
            else
            {
                foreach (PictureBox pb in pbs)
                {
                    pb.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
                }
            }
        }

        /// <summary>
        /// change location of player control panel
        /// </summary>
        public void movePanelPlayerButtons() {
            int x = x_loc[game.cur_player];
            int y = y_loc;
            panel_playerButtons.Location = new Point(x, y);
        }

        /// <summary>
        /// show card face of dealer's face down card
        /// </summary>
        /// <param name="c"></param>
        public void flipDealerCard(System.Drawing.Bitmap c)
        {
            // flip dealer card face up
            groupBox_dealer.Controls[1].BackgroundImage = c;
        }
        

        /// <summary>
        /// 
        /// </summary>
        public void display_playButtons()
        {
            label_hd.Text = "";
            movePanelPlayerButtons();

            // display betting controls under player currently betting
            display_playerControls(true, 1);
            panel_playerButtons.Focus();
        }

        /// <summary>
        /// try to go to next player to place bet
        /// </summary>
        private void nextBet()
        {
            int total_players = game.players.Count - 1;

            if (game.findNextActivePlayer()){

                movePanelPlayerButtons();

                // add label_bet to player's betting groupBox
                Label temp = game.players[game.cur_player].hands[game.cur_hand].label_bet;
                panels_bets[game.cur_player].Controls.Add(temp);

                //label_caption1.Text = "MAKE YOUR BET,";
                label_caption2.Text = playernames[game.cur_player].Text.ToUpper();
             }
             else{
                label_caption1.Text = "";
                label_caption1.Visible = false;
                label_caption2.Visible = false;
                pictureBox_caption.Visible = false;

                game.startNewRound();
                display_playButtons();
                update_player();
             }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="status"></param>
        private void update_Hand(string status) {
            labels_hand[game.cur_player].Text += status;
            labels_hand[game.cur_player].BackColor = System.Drawing.Color.Transparent;
            labels_hand[game.cur_player].ForeColor = System.Drawing.Color.Black;
        }

        /// <summary>
        /// 
        /// </summary>
        public void update_dealer()
        {
            display_playerControls(false, 1);

            labels_hand[game.cur_player].Text = game.currentHand.Sum.ToString();

            if (game.currentHand.Status == -100){
                labels_hand[game.cur_player].Text += " - BUST";
            }
            if (game.currentHand.Status == 100){
                labels_hand[game.cur_player].Text += " - BLACKJACK";
            }
        }

        /// <summary>
        /// *********update the form with the status of a hand of cards (bust, blackjack, neither)
        /// </summary>
        public void update_player()
        {
            BlackjackHand hand = game.currentHand;
            highlight(1);

            labels_hand[game.cur_player].Text = hand.Sum.ToString();

                switch (hand.Status){
                    case -100:// BUST (-100)
                        update_Hand(" - BUST");
                        highlight(0);
                        game.gotoNextHand();
                        break;
                    case 100:// BLACKJACK (100)
                        update_Hand("  - BLACKJACK");
                        highlight(0);
                        game.gotoNextHand();
                        break;
                    case 0:// hand is new 
                        displayPossibleActions(1);
                        break;
                    default:// hand is not new one
                        displayPossibleActions(0);// (stand, hit)
                        break;
                }
        }


        /// <summary>
        /// 
        /// </summary>
        public void update_finalStatus()
        {


            if (game.NumActivePlayers == 0){
                // GAME IS OVER
                updatePlayerBalances();
                pictureBox_caption.Visible = true;
                label_caption1.Visible = true;
                label_caption2.Visible = true;
                label_caption1.Text = "MEOW MEOW";
                label_caption2.Text = "GAME OVER";
                button_continue.Visible = false;
            }
            else{
                // GAME IS NOT OVER
                button_continue.Visible = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void resetGame()
        {
            // manage form controls
            button_reset.Enabled = false;
            button_continue.Visible = false;
            comboBox_playerSelect.Enabled = true;
            button_start.Visible = true;
            NumPlayers = "1";
            pictureBox_caption.Visible = true;
            label_caption1.Text = "SET YOUR PLAYERS";
            label_caption2.Text = "SET YOUR NAME";
            label_caption1.Visible = true;
            label_caption2.Visible = true;

            display_playerControls(false, 1);
            clearCards();
            clearLabelsAboveHands();
            clearPlayerBets();
            resetPlayerNames();
            resetPlayerBalances();

            // manage game elements
            game.reset(true);
        }

        /// <summary>
        /// 
        /// </summary>
        void resetPlayerBalances()
        {
            // update balance labels
            foreach (Label label in labels_balance)
            {
                label.Text = "0";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        void resetPlayerNames() 
        {
            foreach (TextBox textbox in playernames){
                textbox.Text = "Player " + textbox.Tag;
                textbox.ReadOnly = false;
                textbox.BackColor = System.Drawing.Color.DarkSlateGray;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        void clearCards()
        {
            // clear groupboxes
            foreach (GroupBox gb in groupboxes){
                gb.Controls.Clear();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        void clearLabelsAboveHands()
        {
            // clear form labels
            foreach (Label label in labels_hand){
                label.Text = "        ";
                label.BackColor = System.Drawing.Color.Transparent;
                label.ForeColor = System.Drawing.Color.White;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        void clearPlayerBets()
        {
            // clear bets
            foreach (FlowLayoutPanel panel in panels_bets){
                panel.Controls.Clear();
            }
        }


        void updatePlayerBalances()
        {

            foreach (BlackjackPlayer player in game.players)
            {
                if (!player.IsActive)
                {
                    labels_balance[player.id].Text = "GAME OVER";
                }
                else
                {
                    labels_balance[player.id].Text = player.Balance.ToString();
                }
            }
        }

        /* BUTTON CLICKS */

        // add a card to the current hand
        private void button_hit_Click(object sender, EventArgs e)
        {
            game.hit();
            update_player();

            Stream str = Properties.Resources.bark;
            SoundPlayer snd = new SoundPlayer(str);
            snd.Play();
        }

        // end the turn of the current hand
        private void button_stand_Click(object sender, EventArgs e)
        {
            // manage form controls
            highlight(0);
            labels_hand[game.cur_player].BackColor = System.Drawing.Color.Transparent;
            labels_hand[game.cur_player].ForeColor = System.Drawing.Color.Black;
            // manage game elements
            game.stand();
        }

        //
        private void button_start_Click(object sender, EventArgs e)
        {
            // manage form controls
            comboBox_playerSelect.Enabled = false;
            button_start.Visible = false;
            button_reset.Enabled = true;
            label_caption1.Visible = true;
            

            // lock player names
            foreach (TextBox textbox in playernames) {
                textbox.ReadOnly = true;
                textbox.BackColor = System.Drawing.Color.SeaGreen;
            }
            // manage game elements
            game.startNewGame();

            //updateForm_showBetButtons();
            updatePlayerBalances();
            display_betButtons();
        }


        //
        private void button_reset_Click(object sender, EventArgs e)
        {
            resetGame();
        }


        //
        private void button_continue_Click(object sender, EventArgs e)
        {
            // manage form controls
            button_continue.Visible = false;
            clearCards();
            clearLabelsAboveHands();
            clearPlayerBets();
            panel_playerButtons.Focus();
            updatePlayerBalances();

            // manage game elements
            game.continuePlaying();

            label_caption1.Visible = true;
            display_betButtons();
        }

        private void button_bet_Click(object sender, EventArgs e)
        {
            Button b = (Button)sender;
            int value = int.Parse(b.Tag.ToString());

            if (value == 999)
            {
                game.setBet((int)game.players[game.cur_player].Balance);
            }
            else {
                game.setBet(value);
            }
            updatePlayerBalances();
            nextBet();
        }


        //
        private void button_exit_Click(object sender, EventArgs e)
        {
            System.Environment.Exit(0);
        }

        private void button_double_Click(object sender, EventArgs e)
        {
            labels_hand[game.cur_player].ForeColor = System.Drawing.Color.Black;
            game.doubleDown();
            updatePlayerBalances();
            // update that hands label
            labels_hand[game.cur_player].Text = game.currentHand.Sum.ToString();
            // go to next hand
            game.gotoNextHand();
        }


        private void comboBox_playerSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = comboBox_playerSelect.SelectedIndex;
            switch (index) {
                case 3:
                    player4.Visible = true;
                    player3.Visible = true;
                    player2.Visible = true;
                    break;
                case 2:
                    player4.Visible = false;
                    player3.Visible = true;
                    player2.Visible = true;
                    break;
                case 1:
                    player4.Visible = false;
                    player3.Visible = false;
                    player2.Visible = true;
                    break;
                case 0:
                    player4.Visible = false;
                    player3.Visible = false;
                    player2.Visible = false;
                    break;

            }
        }
    }// class
}// namespace
