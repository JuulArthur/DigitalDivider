using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DigitalDivider
{
    class Highscore : IComparable<Highscore>
    {
        private String name;
        private int score;
        private DateTime time;

        public Highscore(String name, int score, DateTime time)
        {
            this.name = (name==null?"":name);
            this.score = score;
            this.time = time;
        }

        public String getName()
        {
            return name;
        }

        public int getScore()
        {
            return score;
        }

        public DateTime getTime()
        {
            return time;
        }

        public override String ToString()
        {
            return name.ToString() + ":" + score.ToString();
        }

        public int CompareTo(Highscore otherHighScore)
        {
            return otherHighScore.getScore() - this.getScore();
			//større highscore skal komme høyere opp på lista
        }
    }
}
