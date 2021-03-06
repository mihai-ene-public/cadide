// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using IxMilia.Step.Tokens;

namespace IxMilia.Step.Syntax
{
    internal class StepFileSyntax : StepSyntax
    {
        public override StepSyntaxType SyntaxType => StepSyntaxType.File;

        public StepHeaderSectionSyntax Header { get; }
        public StepDataSectionSyntax Data { get; }

        public StepFileSyntax(StepHeaderSectionSyntax header, StepDataSectionSyntax data)
            : base(header.Line, header.Column)
        {
            Header = header;
            Data = data;
        }

        public override IEnumerable<StepToken> GetTokens()
        {
            throw new NotSupportedException();
        }
    }
}
