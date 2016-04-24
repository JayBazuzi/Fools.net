using System.Collections.Generic;
using Gibberish.AST._1_Bare;
using JetBrains.Annotations;

namespace Gibberish.AST._2_Fasm.Builders
{
	public class UnknownStatementBuilder : AstBuilder<FasmOrLanguageConstruct>
	{
		public UnknownStatementBuilder(string unparsedContent, int indentationDepth)
		{
			UnparsedContent = unparsedContent;
			_indentationDepth = indentationDepth;
		}

		[NotNull]
		public List<int> Comments { get; } = new List<int>();
		[NotNull]
		public string UnparsedContent { get; }

		[NotNull]
		public UnknownStatementBuilder WithCommentRefs([NotNull] params int[] indices)
		{
			Comments.AddRange(indices);
			return this;
		}

		internal override void Build(List<FasmOrLanguageConstruct> destination)
		{
			destination.Add(new UnknownStatement(PossiblySpecified<int>.WithValue(_indentationDepth), UnparsedContent, Comments, Errors));
		}

		private readonly int _indentationDepth;
	}
}
