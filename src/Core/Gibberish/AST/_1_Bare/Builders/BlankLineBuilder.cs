using System.Collections.Generic;

namespace Gibberish.AST._1_Bare.Builders
{
	public class BlankLineBuilder : AstBuilder<LanguageConstruct>
	{
		public BlankLineBuilder(PossiblySpecified<int> indentationDepth)
		{
			IndentationDepth = indentationDepth;
		}

		public PossiblySpecified<int> IndentationDepth { get; }

		internal override void Build(List<LanguageConstruct> destination)
		{
			destination.Add(new BlankLine(IndentationDepth, Errors));
		}
	}
}
