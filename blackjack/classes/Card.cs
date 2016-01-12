using blackjack.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace blackjack.classes
{
    // enums
    public enum Suit{ clubs, diamonds, hearts, spades };
    public enum Face{ace=1, two, three, four, five, six, seven, eight, nine, ten, jack, queen, king};

    public class Card : PictureBox
    {

        // image for card 
        public readonly Bitmap card_back = blackjack.Properties.Resources.cardback;
        public readonly Bitmap card_front;

        // constructor
        public Card(string face) {
            ResourceManager rm = Resources.ResourceManager;
            Bitmap myImage = (Bitmap)rm.GetObject(face);
            card_front = myImage;

            // display variables
            Width = 60;
            Height = 95;
            BackgroundImageLayout = ImageLayout.Stretch;
            BackgroundImage = card_front;
            Location = new Point(x, y); 

        }

        /* backing fields */
        private Suit _suit;
        private Face _face;
        private int _value;

        /* properties */
        public int x { get; set; }
        public int y { get; set; }
        public bool IsFaceDown { get; set; }
        public Suit Suit
        {
            get { return _suit; }
            set { _suit = value; }
        }
        public Face Face
        {
            get { return _face; }
            set { _face = value; }
        }
        
        public int Value
        {
            get { return _value; }
            set { _value = value; }
        }
    }
}
