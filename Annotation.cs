using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Binairo
{
    public class Annotation : Label
    {
        private int nbrZero = 0;
        private int nbrUn = 0;

        public int NbrZero { get => nbrZero; set => nbrZero = value; }
        public int NbrUn { get => nbrUn; set => nbrUn = value; }

        public Annotation()
        {
        }

        public Annotation(int x, int y)
        {
            Top = y; Left = x;
            ForeColor = Color.Black;
            Font = new Font(Font.FontFamily, 12, FontStyle.Italic);
            Text = "0/0";
            Width = 40;
            nbrUn = 0;
            nbrZero = 0;
        }

        public void MAJ()
        {
            this.Text = nbrZero.ToString() + "/" + nbrUn.ToString();
            return;
        }
    }
}