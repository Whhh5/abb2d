using System.Collections.Generic;
using UnityEngine;

public sealed class AICommonModule : AIModule
{
    private List<EnEntityCmd> _CmdList = new();
    public override void Execute()
    {

    }

    public override void Finish()
    {

    }

    public override bool IsBreak()
    {
        return false;
    }

    public override bool IsExecute()
    {
        return false;
    }

    public override bool IsNextModule()
    {
        return false;
    }

    public override void PreExecute()
    {

    }

    public override void Reexecute()
    {

    }
}
