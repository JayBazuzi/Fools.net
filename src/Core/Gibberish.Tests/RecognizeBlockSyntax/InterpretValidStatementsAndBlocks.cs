﻿using System.Collections.Generic;
using Gibberish.AST;
using Gibberish.AST._1_Bare;
using Gibberish.Parsing;
using Gibberish.Tests.ZzTestHelpers;
using NUnit.Framework;

namespace Gibberish.Tests.RecognizeBlockSyntax
{
	[TestFixture]
	public class InterpretValidStatementsAndBlocks
	{
		[Test, TestCaseSource(nameof(valid_recognitions))]
		public void should_recognize_as(string input, BasicAst.Builder expected)
		{
			var subject = new RecognizeBlocks();
			var result = subject.GetMatch(input, subject.WholeFile);

			result.Should()
				.BeRecognizedAs(expected);
		}

		public static IEnumerable<IEnumerable<object>> valid_recognitions { get; } = new[]
		{
			new object[]
			{
				"",
				BasicAst.BlankLine(0)
					.WithError(ParseError.MissingNewlineAtEndOfFile())
			},
			new object[]
			{
				" ",
				BasicAst.BlankLine(0)
					.WithError(ParseError.IllegalWhitespaceOnBlankLine(" "))
					.WithError(ParseError.MissingNewlineAtEndOfFile())
			},
			new object[]
			{
				"\t",
				BasicAst.BlankLine(1)
					.WithError(ParseError.MissingNewlineAtEndOfFile())
			},
			new object[]
			{
				"\r\n",
				BasicAst.BlankLine(0)
			},
			new object[]
			{
				"\t\t\r\n",
				BasicAst.BlankLine(2)
			},
			new object[]
			{
				"\t \t\r\n",
				BasicAst.BlankLine(1)
					.WithError(ParseError.IllegalWhitespaceOnBlankLine(" \t"))
			},
			new object[]
			{
				"arbitrary statement\r\n",
				BasicAst.Statement("arbitrary statement")
			},
			new object[]
			{
				"arbitrary statement",
				BasicAst.Statement("arbitrary statement")
					.WithError(ParseError.MissingNewlineAtEndOfFile())
			},
			new object[]
			{
				"arbitrary statement \r\n",
				BasicAst.Statement("arbitrary statement")
					.WithError(ParseError.IllegalWhitespaceAtEnd(" "))
			},
			new object[]
			{
				"arbitrary statement\t\r\n",
				BasicAst.Statement("arbitrary statement")
					.WithError(ParseError.IllegalWhitespaceAtEnd("\t"))
			},
			new object[]
			{
				"arbitrary\tstatement\r\n",
				BasicAst.Statement("arbitrary\tstatement")
					.WithError(ParseError.IllegalTabInLine())
			},
			new object[]
			{
				"arbitrary statement\t\t#[2]\r\n",
				BasicAst.Statement("arbitrary statement")
					.WithCommentRefs(2)
			},
			new object[]
			{
				"arbitrary statement\t\t#[2], [42], [3]\r\n",
				BasicAst.Statement("arbitrary statement")
					.WithCommentRefs(2, 42, 3)
			},
			new object[]
			{
				"arbitrary statement\t\t#[2],[4]\r\n",
				BasicAst.Statement("arbitrary statement")
					.WithError(ParseError.IncorrectCommentFormat("[2],[4]"))
			},
			new object[]
			{
				"arbitrary statement\t\t#[2a4]\r\n",
				BasicAst.Statement("arbitrary statement")
					.WithError(ParseError.IncorrectCommentFormat("[2a4]"))
			},
			new object[]
			{
				"arbitrary statement\t\t\t#[2]\r\n",
				BasicAst.Statement("arbitrary statement")
					.WithCommentRefs(2)
					.WithError(ParseError.IncorrectCommentSeparator("\t\t\t#"))
			},
			new object[]
			{
				"arbitrary block:\r\n\tpass\r\n",
				BasicAst.Block("arbitrary block")
					.WithBody(b => b.AddStatement("pass"))
			},
			new object[]
			{
				"arbitrary block:",
				BasicAst.Block("arbitrary block", p => p.WithError(ParseError.MissingNewlineAtEndOfFile()))
			},
			new object[]
			{
				"arbitrary block:\r\n\tfirst\r\n\tsecond block:\r\n\t\tpass\r\n\tlast\r\n",
				BasicAst.Block("arbitrary block")
					.WithBody(
						b =>
						{
							b.AddStatement("first");
							b.AddBlock("second block")
								.WithBody(inner => inner.AddStatement("pass"));
							b.AddStatement("last");
						})
			},
			new object[]
			{
				"arbitrary block:\t\t#[3]\r\n\tpass\r\n",
				BasicAst.Block("arbitrary block", p => p.WithCommentRefs(3))
					.WithBody(b => b.AddStatement("pass"))
			},
			new object[]
			{
				"arbitrary block: \t\t#[3]\r\n\tpass\r\n",
				BasicAst.Block(
					"arbitrary block",
					p => p.WithCommentRefs(3)
						.WithError(ParseError.IncorrectCommentSeparator(" \t\t#")))
					.WithBody(b => b.AddStatement("pass"))
			},
			new object[]
			{
				"arbitrary block:\r\n\tpass \r\n",
				BasicAst.Block("arbitrary block")
					.WithBody(
						b => b.AddStatement("pass")
							.WithError(ParseError.IllegalWhitespaceAtEnd(" ")))
			},
			new object[]
			{
				"arbitrary block: \r\n\tpass\r\n",
				BasicAst.Block("arbitrary block", p => p.WithError(ParseError.IllegalWhitespaceAtEnd(" ")))
					.WithBody(b => b.AddStatement("pass"))
			},
			new object[]
			{
				"arbitrary block:\t\t#[4] \r\n\tpass\r\n",
				BasicAst.Block(
					"arbitrary block",
					p => p.WithCommentRefs(4)
						.WithError(ParseError.IllegalWhitespaceAtEnd(" ")))
					.WithBody(b => b.AddStatement("pass"))
			},
			new object[]
			{
				"arbitrary block:\t\r\n\tpass\r\n",
				BasicAst.Block("arbitrary block", p => p.WithError(ParseError.IllegalWhitespaceAtEnd("\t")))
					.WithBody(b => b.AddStatement("pass"))
			},
			new object[]
			{
				"arbitrary block\t:\r\n\tpass\r\n",
				BasicAst.Block("arbitrary block\t", p => p.WithError(ParseError.IllegalTabInLine()))
					.WithBody(b => b.AddStatement("pass"))
			},
			new object[]
			{
				"arbitrary\tblock:\r\n\tpass\r\n",
				BasicAst.Block("arbitrary\tblock", p => p.WithError(ParseError.IllegalTabInLine()))
					.WithBody(b => b.AddStatement("pass"))
			},
			new object[]
			{
				"#[2]: comment content\r\n",
				BasicAst.CommentDefinition(2, "comment content")
			},
			new object[]
			{
				"#[22]: comment content",
				BasicAst.CommentDefinition(22, "comment content").WithError(ParseError.MissingNewlineAtEndOfFile())
			},
			new object[]
			{
				"#[9]:\tcomment content\r\n",
				BasicAst.CommentDefinition(9, "comment content")
					.WithError(ParseError.IncorrectCommentDefinitionSeparator("\t"))
			},
			new object[]
			{
				"# just a bare comment\r\n",
				BasicAst.CommentDefinition(0, "just a bare comment")
					.WithError(ParseError.MissingIdInCommentDefinition("just a b"))
			},
			new object[]
			{
				"#[3a3]: almost right\r\n",
				BasicAst.CommentDefinition(0, "[3a3]: almost right")
					.WithError(ParseError.MissingIdInCommentDefinition("[3a3]: a"))
			},
			new object[]
			{
				"#[8]: \"\"\"\r\n\"\"\"\r\n",
				BasicAst.CommentDefinition(8, "\r\n")
			},
			new object[]
			{
				@"#[9]: """"""first

more
""""""
",
				BasicAst.CommentDefinition(9, @"first

more
")
			},
			new object[]
			{
				"#[16]: \"\"\"\r\n#[12]: hi\r\n",
				BasicAst.CommentDefinition(16, "\r\n#[12]: hi\r\n")
					.WithError(ParseError.MultilineCommentWithoutEnd())
			},
			new object[]
			{
				"#[13]: \"\"\"\r\n \"\"\"\r\n",
				BasicAst.CommentDefinition(13, "\r\n")
					.WithError(ParseError.ErrorAtEndOfMultilineComment(" \"\"\""))
			},
			new object[]
			{
				"#[17]: \"\"\"\r\n\"\"\" \r\n",
				BasicAst.CommentDefinition(17, "\r\n")
					.WithError(ParseError.ErrorAtEndOfMultilineComment("\"\"\" "))
			}
		};
	}
}
