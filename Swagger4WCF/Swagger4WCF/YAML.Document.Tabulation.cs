using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swagger4WCF
{
    static public partial class YAML
    {
        public partial class Document
        {
            private class Tabulation
            {
                static public implicit operator string(Tabulation tabulation)
                {
                    return tabulation == null ? string.Empty : tabulation.ToString();
                }

                private string m_Pattern;
                private int m_Level;

                public Tabulation(string pattern, int level)
                {
                    this.m_Pattern = pattern;
                    this.m_Level = level;
                }

                public string Pattern
                {
                    get { return this.m_Pattern; }
                }

                public int Level
                {
                    get { return this.m_Level; }
                }

                override public string ToString()
                {
                    var _builder = new StringBuilder();
                    for (var _index = 0; _index < this.m_Level; _index++) { _builder.Append(this.m_Pattern); }
                    return _builder.ToString();
                }
            }
        }
    }
}
