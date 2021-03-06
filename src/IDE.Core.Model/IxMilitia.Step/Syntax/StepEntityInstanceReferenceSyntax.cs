// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Generic;
using IxMilia.Step.Tokens;

namespace IxMilia.Step.Syntax
{
    internal class StepEntityInstanceReferenceSyntax : StepSyntax
    {
        public override StepSyntaxType SyntaxType => StepSyntaxType.EntityInstanceReference;

        public int Id { get; }

        public StepEntityInstanceReferenceSyntax(int id)
            : base(-1, -1)
        {
            Id = id;
        }

        public StepEntityInstanceReferenceSyntax(StepEntityInstanceToken itemInstance)
            : base(itemInstance.Line, itemInstance.Column)
        {
            Id = itemInstance.Id;
        }

        public override IEnumerable<StepToken> GetTokens()
        {
            yield return new StepEntityInstanceToken(Id, -1, -1);
        }
    }
}
