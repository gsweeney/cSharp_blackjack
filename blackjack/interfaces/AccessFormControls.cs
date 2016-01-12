using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;
using blackjack.classes;

namespace blackjack.interfaces
{
    interface AccessFormControls
    {
        // string from numPlayers textbox
        string NumPlayers { get; set; }

        void update_player();
        void update_dealer();
        //void updateForm_showPlayButtons();
        //void updateForm_showBetButtons();
        //void updateForm_playerBalance(BlackjackPlayer player);
        //void updateForm_handLabel();

        void flipDealerCard(System.Drawing.Bitmap c);
        void movePanelPlayerButtons();

        void update_finalStatus();

        // display a card on the form
        void displayCard(Card card, int id);

        // check final status of dealer hand
        void displayFinalResult(int player_id, BlackjackHand hand);
    }
}
