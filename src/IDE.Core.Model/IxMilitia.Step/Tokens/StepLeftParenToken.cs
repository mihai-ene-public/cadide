// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace IxMilia.Step.Tokens
{
    internal class StepLeftParenToken : StepToken
    {
        public override StepTokenKind Kind => StepTokenKind.LeftParen;

        public StepLeftParenToken(int line, int column)
            : base(line, column)
        {
        }

        public override string ToString()
        {
            return "(";
        }

        public static StepLeftParenToken Instance { get; } = new StepLeftParenToken(-1, -1);
    }
}
