using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Gibberish.AST._1_Bare
{
	public class UnknownStatement : LanguageConstruct
	{
		public UnknownStatement(string content, IEnumerable<int> comments, IEnumerable<ParseError> errors) : base(errors)
		{
			Content = content;
			Comments = comments.ToArray();
		}

		[NotNull]
		public string Content { get; }
		[NotNull]
		public int[] Comments { get; }
	}
}
