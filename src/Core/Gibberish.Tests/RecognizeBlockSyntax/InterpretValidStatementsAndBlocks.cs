﻿using System;
using System.Collections.Generic;
using ApprovalTests;
using ApprovalTests.Reporters;
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
		[Test, TestCaseSource(nameof(valid_recognitions)), UseReporter(typeof (QuietReporter))]
		public void should_recognize_as(string input, BasicAst.Builder expected)
		{
			var subject = new RecognizeBlocks();
			var result = subject.GetMatch(input, subject.WholeFile);
			try
			{
				result.Should()
					.BeRecognizedAs(expected);
			}
			catch (Exception) {
				Approvals.VerifyJson(result.PrettyPrint());
			}
		}

		public static IEnumerable<IEnumerable<object>> valid_recognitions { get; } = new[]
		{
			new object[]
			{
				"arbitrary statement\r\n",
				BasicAst.Statement("arbitrary statement")
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
					.WithError(ParseError.IncorrectCommentSeparator("\t\t\t"))
			},
			new object[]
			{
				"arbitrary block:\r\n\tpass\r\n",
				BasicAst.Block("arbitrary block")
					.WithBody(b => b.AddStatement("pass"))
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
						.WithError(ParseError.IncorrectCommentSeparator(" \t\t")))
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
			}
		};
	}
}
