using System;

namespace Data;

public class NotifyStateService
{
    public event EventHandler? UserChange;

    public void NotifyUserChange(object sender)
    {
        if (UserChange == null) return;
        UserChange(sender, EventArgs.Empty);
    }
}