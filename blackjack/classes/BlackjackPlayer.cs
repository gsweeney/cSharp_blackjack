using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blackjack.classes
{
    public class BlackjackPlayer
    {

        public List<BlackjackHand> hands = new List<BlackjackHand>();
        public int id; // Player number
        private double _balance;

        // display the bet on the hand
        //public System.Windows.Forms.Label label_balance;

        public bool IsActive { get; set; }

        // property
        public double Balance 
        {
            get { return _balance; }
            set { _balance = value; //label_balance.Text = _balance.ToString(); 
                }
        
        }
        //public bool IsActive { get; set; }

        public BlackjackPlayer()
        {
            //label_balance = new System.Windows.Forms.Label();
            BlackjackHand temp = new BlackjackHand();
            IsActive = true;
            temp.Status = -1;
            temp.x_nextOffset = 0;
            temp.y_nextOffset = 0;
            hands.Add(temp);
            Balance = BlackjackGame.INITIAL_BALANCE;
        }

        public int isActive() {

            if (Balance < 5)
            {
                IsActive = false;
                return -1;
            }
            else {
                return 0;
            }
        }

    }
}
