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
            private partial class Content
            {
                private class Block : IDisposable
                {
                    private Content m_Content;

                    public Block(Content content)
                    {
                        this.m_Content = content;
                        this.m_Content.m_Tabulation = new Tabulation(this.m_Content.m_Tabulation.Pattern, this.m_Content.m_Tabulation.Level + 1);
                    }

                    void IDisposable.Dispose()
                    {
                        this.m_Content.m_Tabulation = new Tabulation(this.m_Content.m_Tabulation.Pattern, this.m_Content.m_Tabulation.Level - 1);
                    }
                }
            }
        }
    }
}
