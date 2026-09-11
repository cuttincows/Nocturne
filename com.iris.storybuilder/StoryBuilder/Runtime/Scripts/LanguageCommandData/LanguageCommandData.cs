using System.Collections.Generic;
using System.Text;

public abstract class LanguageCommandData
{
    public int Indent;

    public abstract string CommandName { get; }
    
    public abstract bool Deserialize(List<string> lines, ref int lineIndex);

    public string Serialize()
    {
        var result = new StringBuilder();
        for (int i = 0; i < Indent; i++)
        {
            result.Append('\t');
        }

        result.Append(SerializeInternal());
        
        return result.ToString();
    }

    public abstract void ReplaceVars(LanguageInterpreter interpreter);

    protected abstract string SerializeInternal();
}
